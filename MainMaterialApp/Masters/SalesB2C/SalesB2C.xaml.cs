using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Printing;
using System.Windows.Documents;
using IPrint;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System;

namespace MainMaterialApp.Masters.SalesB2C
{

    public partial class SalesB2C : UserControl
    {
        
        ObservableCollection<string> obj2 = new ObservableCollection<string>();
        ObservableCollection<Models.TableDataStructure> datagriditems = new ObservableCollection<Models.TableDataStructure>();
        Models.BasicInfo basicinfo = new Models.BasicInfo();
        QueryHandler.QueryHandler queryHandler = new QueryHandler.QueryHandler();
        ManualValidationHandler.ManualValidationHandler err = new ManualValidationHandler.ManualValidationHandler();
        Models.TableTotal totals = new Models.TableTotal();


             
        public SalesB2C()
        {
            
            InitializeComponent();
            InsideDatagrid.ItemsSource = datagriditems;
            
            datagriditems.Add(new Models.TableDataStructure());
            MobileTextBox.DataContext = basicinfo;
            NameTextBox.DataContext = basicinfo;
            PlaceTextBox.DataContext = basicinfo;

            TotalQuantity.DataContext = totals;
            TotalAmount.DataContext = totals;
            TotalMrp.DataContext = totals;
            TotalCgst.DataContext = totals;
            TotalSgst.DataContext = totals;
            TotalDiscount.DataContext = totals;


            obj2.Add("afham");
            obj2.Add("sanooj");
            obj2.Add("shameel");
            DgSalesmanColumn.ItemsSource = obj2;


        }
      

        private void PayNow_Click(object sender, RoutedEventArgs e)
        {
            var paymentWindow = new SubWindows.PaymentWindow(() => Blur.Radius = 0);
            Blur.Radius = 5;
            paymentWindow.ShowDialog();
           
        }

       

        private void MobileTextBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
    
               var response =queryHandler.HandleQuery($"SELECT c.id,c.name,c.mobile,c.cityid, ct.name as place FROM public.customers" +
                   $" c LEFT JOIN cities ct ON c.cityid = ct.id WHERE c.deletedat IS " +
                   $"NULL and c.mobile = '{basicinfo.Mobile}' LIMIT 1", "select");

                if (response.HasValues)
                {
                    basicinfo.Name = response[0]["name"].ToString();
                    basicinfo.Place = response[0]["place"].ToString();
                }
                else
                {
                    basicinfo.Name = "";
                    basicinfo.Place = "";
                }
            }
     
        }

        private void InsideDatagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (e.Column.Header.ToString() == "Barcode")
            {
                var editedTextbox = e.EditingElement as TextBox;
                var editedText = editedTextbox.Text;

               
                var res = queryHandler.HandleQuery($"SELECT pd.barcode,pd.variantid,pd.mrp,CONCAT(i.name,' ',iv.name) as item, CASE WHEN td.cgstrate IS NULL THEN 0 ELSE td.cgstrate END, CASE WHEN td.sgstrate IS NULL THEN 0 ELSE td.sgstrate END FROM public.purchaseitemdetails pd LEFT JOIN items i ON pd.itemid = i.id LEFT JOIN itemvariants iv ON pd.variantid = iv.id LEFT JOIN taxes t ON i.taxid = t.id LEFT JOIN taxdetails td ON t.id = td.taxid AND td.type= 'I' AND (pd.mrp between td.ratefrom AND td.rateto ) WHERE pd.barcode='C98562'", "select");
               
           

                    if (res.HasValues)
                    {
                        datagriditems[e.Row.GetIndex()].Id = res[0]["variantid"].ToString();
                        datagriditems[e.Row.GetIndex()].ItemName = res[0]["item"].ToString();
                        datagriditems[e.Row.GetIndex()].Barcode = res[0]["barcode"].ToString();
                        datagriditems[e.Row.GetIndex()].Mrp = res[0]["mrp"].ToString();
                        //datagriditems[e.Row.GetIndex()].Discount = res[0]["discount"].ToString();
                        datagriditems[e.Row.GetIndex()].Cgst = res[0]["cgstrate"].ToString();
                        datagriditems[e.Row.GetIndex()].Sgst = res[0]["sgstrate"].ToString();
                        //datagriditems[e.Row.GetIndex()].Details = res[0]["item"].ToString();
                        //datagriditems[e.Row.GetIndex()].SgstDetails = res[0]["sgst"].ToString();
                        //datagriditems[e.Row.GetIndex()].DiscountDetails = res[0]["discount"].ToString();
                        //datagriditems[e.Row.GetIndex()].Salesman = "shameel";

                        CalculateTotals();

                        InsideDatagrid.CurrentCell = new DataGridCellInfo(InsideDatagrid.Items[e.Row.GetIndex()], InsideDatagrid.Columns[3]);
                        InsideDatagrid.BeginEdit();
                        editedTextbox.Text = editedTextbox.Text;


                    }
                    else
                    {
                        e.Cancel = true;
                        datagriditems[e.Row.GetIndex()].InvalidBarcode = true;
                        InsideDatagrid.BeginEdit();
                        editedTextbox.Text = editedTextbox.Text;

                        //InsideDatagrid.BeginEdit();
                        //var row = (DataGridRow)InsideDatagrid.ItemContainerGenerator.ContainerFromItem(InsideDatagrid.CurrentItem);
                        //var txt = InsideDatagrid.Columns[1].GetCellContent(row) as TextBox;
                        //err.Trigger(txt, "Invalid Barcode");
                    }
               
            }
            //else if (e.Column.Header.ToString() == "Qty")
            //{
            //    var editedTextbox = e.EditingElement as TextBox;
            //    var editedText = editedTextbox.Text;
            //    if (string.IsNullOrWhiteSpace(editedText) || string.IsNullOrEmpty(datagriditems[e.Row.GetIndex()].Barcode))
            //    {
            //        e.Cancel = true;
            //    }
            //    else
            //    {
            //        if (datagriditems.Count == e.Row.GetIndex() + 1)
            //        {
            //            datagriditems.Add(new Models.TableDataStructure());
            //        }
            //        CalculateTotals();
            //        InsideDatagrid.CurrentCell = new DataGridCellInfo(InsideDatagrid.Items[e.Row.GetIndex()], InsideDatagrid.Columns[9]);
            //        InsideDatagrid.BeginEdit();
            //    }

            //}
            //else if (e.Column.Header.ToString() == "Salesman")
            //{
            //    //InsideDatagrid.CurrentCell = new DataGridCellInfo(InsideDatagrid.Items[datagriditems.Count - 1], InsideDatagrid.Columns[1]);
            //    //InsideDatagrid.BeginEdit();
            //}

        }



        private void ManualBillToggle_Click(object sender, RoutedEventArgs e)
        {
            if (ManualBillToggle.IsChecked == true)
            {
                if (MessageBox.Show("Are you sure to ENABLE Manual Bill ? ", "Comfirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ManualBillToggle.IsChecked = true;
                    Card.Background = Brushes.AliceBlue;
                }
                else
                {
                    ManualBillToggle.IsChecked = false;
                    Card.Background = Brushes.White;

                }
            }
            else if (ManualBillToggle.IsChecked == false)
            {
                if (MessageBox.Show("Are you sure to DISABLE Manual Bill ? ", "Comfirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ManualBillToggle.IsChecked = false;
                    Card.Background = Brushes.White;

                }
                else
                {
                    ManualBillToggle.IsChecked = true;
                    Card.Background = Brushes.AliceBlue;

                }
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

            FlowDocument doc = new FlowDocument();

            //Paragraph p = new Paragraph(new Run("Hello, world!"));
            //p.FontSize = 36;
            //doc.Blocks.Add(p);

            //p = new Paragraph(new Run("The ultimate programming greeting!"));
            //p.FontSize = 14;
            //p.FontStyle = FontStyles.Italic;
            //p.TextAlignment = TextAlignment.Left;
            //p.Foreground = Brushes.Gray;
            //doc.Blocks.Add(p);
           
            //IPrintDialog.PreviewDocument(doc);

        }

        private void InsideDatagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InsideDatagrid.CommitEdit();
                e.Handled = true;
            }
        }

        public void isLoading(bool value) {
            if (value == true)
            {
                Blur.Radius = 5;
                B2CUserControl.IsHitTestVisible = false;
                Loading.Visibility = Visibility.Visible;
                Spinner.Visibility = Visibility.Visible;
                Spinner.IsIndeterminate = true;
            }
            else
            {
                Blur.Radius = 0;
                B2CUserControl.IsHitTestVisible = true;
                Loading.Visibility = Visibility.Hidden;
                Spinner.Visibility = Visibility.Hidden;
                Spinner.IsIndeterminate = false;

            }
        }

        public void CalculateTotals()
        {
            double totalquantity = 0;
            double totalmrp = 0;
            double totaldiscount = 0;
            double totalcgst = 0;
            double totalsgst = 0;
            double totalamount = 0;
            foreach (var items in datagriditems)
            {
                totalquantity += Convert.ToDouble(string.IsNullOrEmpty(items.Qty) ? 0 : items.Qty);
                totalmrp += Convert.ToDouble(string.IsNullOrEmpty(items.Mrp) ? 0 : items.Mrp);
                totaldiscount += Convert.ToDouble(string.IsNullOrEmpty(items.Discount) ? 0 : items.Discount);
                totalcgst += Convert.ToDouble(string.IsNullOrEmpty(items.Cgst) ? 0 : items.Cgst);
                totalsgst += Convert.ToDouble(string.IsNullOrEmpty(items.Sgst) ? 0 : items.Sgst);
                totalamount += Convert.ToDouble(string.IsNullOrEmpty(items.Amount) ? 0 : items.Amount);
                
            }
            totals.TotalQuantity = totalquantity.ToString();
            totals.TotalMrp = totalmrp.ToString();
            totals.TotalDiscount = totaldiscount.ToString();
            totals.TotalCgst = totalcgst.ToString();
            totals.TotalSgst = totalsgst.ToString();
            totals.TotalAmount = totalamount.ToString();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(DataGridCell))
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }

        private void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            TotalScroll.ScrollToHorizontalOffset(e.NewValue);
        }
    }
}
