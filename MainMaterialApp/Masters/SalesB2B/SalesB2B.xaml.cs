using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace MainMaterialApp.Masters.SalesB2B
{
    /// <summary>
    /// Interaction logic for SalesB2B.xaml
    /// </summary>
    
    public partial class SalesB2B : UserControl
    {
        QueryHandler.QueryHandler queryHandler = new QueryHandler.QueryHandler();
        ManualValidationHandler.ManualValidationHandler err = new ManualValidationHandler.ManualValidationHandler();

        ObservableCollection<Models.TableDataStructure> datagriditems = new ObservableCollection<Models.TableDataStructure>();
        ObservableCollection<Models.SalesmanOptionsDataStructure> salesmanOptions = new ObservableCollection<Models.SalesmanOptionsDataStructure>();



        public SalesB2B()
        {
            InitializeComponent();
            B2BDatagrid.ItemsSource = datagriditems;
           
            
            salesmanOptions.Add(new Models.SalesmanOptionsDataStructure() { id = "1", name = "Afham" });
            salesmanOptions.Add(new Models.SalesmanOptionsDataStructure() { id = "2", name = "sanooj" });
      

            datagriditems.Add(new Models.TableDataStructure());
            datagriditems[0].SalesmanOptions = salesmanOptions;
            datagriditems[0].Amount = "23";



            //LoadingGrid.Content = new Loading();

            ///*------------------dummy data ---------------*/
            //for (int i = 0; i < 3; i++)
            //{
            //    datagriditems.Add(new Models.TableDataStructure()
            //    {
            //        Amount = "12",
            //        Barcode = "3223",
            //        Cgst = "3232",
            //        CgstDetails = "2332",
            //        Salesman = "ansar mon",
            //        Sgst = "2121",
            //        SgstDetails = "3434",
            //        Details = "Shirt Details",
            //        Discount = "132",
            //        Id = "23",
            //        DiscountDetails = "422",
            //        Item = "Shirt",
            //        Mrp = "232",
            //        No = "32",
            //        Qty = "4343"

            //    });
            //}

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
          
            //queryHandler.HandleQuery("INSERT NTO public.\"BarcodeItems\"(\"Barcode\", \"Item\", \"Mrp\", \"Discount\", \"Cgst\", \"Sgst\", \"Amount\") VALUES (10021,'banyan', 33, 23, 43, 34, 45)","insert");

        }

        private void B2BDatagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
            //if (e.Column.Header.ToString() == "Barcode")
            //{
            //    var editedTextbox = e.EditingElement as TextBox;
            //    var editedText = editedTextbox.Text;

            //    Mouse.OverrideCursor = Cursors.Wait;
            //    var res = queryHandler.HandleQuery($"select * from public.\"BarcodeItems\" where barcode='{editedText}'","select");
            //    Mouse.OverrideCursor = null;
            //    if (res.HasValues)
            //    {

            //        datagriditems[e.Row.GetIndex()].ItemName = res[0]["item"].ToString();
            //        datagriditems[e.Row.GetIndex()].Barcode = res[0]["barcode"].ToString();
            //        datagriditems[e.Row.GetIndex()].HSN = res[0]["hsn"].ToString();
            //        datagriditems[e.Row.GetIndex()].Mrp = res[0]["mrp"].ToString();
            //        datagriditems[e.Row.GetIndex()].Discount = res[0]["discount"].ToString();
            //        datagriditems[e.Row.GetIndex()].Cgst = res[0]["cgst"].ToString();
            //        datagriditems[e.Row.GetIndex()].Sgst = res[0]["sgst"].ToString();
            //        datagriditems[e.Row.GetIndex()].Amount = res[0]["amount"].ToString();


            //        B2BDatagrid.CurrentCell = new DataGridCellInfo(B2BDatagrid.Items[e.Row.GetIndex()], B2BDatagrid.Columns[4]);
            //        B2BDatagrid.BeginEdit();
            //        editedTextbox.Text = editedTextbox.Text;

            //        if (datagriditems.Count == e.Row.GetIndex() + 1)
            //        {
            //            datagriditems.Add(new Models.TableDataStructure());
            //        }
            //    }
            //    else
            //    {

            //        e.Cancel = true;

            //        err.Trigger(editedTextbox , "Barcode not found");
            //    }

            //}
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
                    
            //        B2BDatagrid.CurrentCell = new DataGridCellInfo(B2BDatagrid.Items[e.Row.GetIndex()], B2BDatagrid.Columns[9]);
            //        B2BDatagrid.BeginEdit();
            //        //editedTextbox.Text = editedTextbox.Text;

            //    }

            //}
 
        }

        private void B2BDatagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                B2BDatagrid.CommitEdit();
                e.Handled = true;
            }
        }

        private void InvoiceNoNumericField_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            //Task.Delay(1000).Wait();
            //var res = queryHandler.HandleQuery($"select* from public.\"BarcodeItems\" where invoiceno = '{e.NewValue.ToString()}'","select");
            //Mouse.OverrideCursor = null;
            //datagriditems.Clear();
            
            //if (res.HasValues)
            //{
            //    try
            //    {
            //        foreach (var item in res)
            //        {
            //            datagriditems.Add(new Models.TableDataStructure() { 
            //                Id="2", 
            //                ItemName = item["item"].ToString() ,
            //                Barcode = item["barcode"].ToString() , 
            //                HSN = item["hsn"].ToString() ,
            //                Mrp = item["mrp"].ToString() ,
            //                Discount = item["discount"].ToString() ,
            //                Cgst = item["cgst"].ToString() ,
            //                Sgst = item["sgst"].ToString() ,
            //                Amount = item["amount"].ToString()
            //            });
                      
            //        }
            //    }
            //    catch(Exception error)
            //    {
            //        MessageBox.Show(error.Message);
            //    }
               
            //}
            //datagriditems.Add(new Models.TableDataStructure());


        }


        private void DgCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = e.OriginalSource as ComboBox;
            var y = x.SelectedItem as ComboBoxItem;
            datagriditems[B2BDatagrid.SelectedIndex].Salesman = y.Content.ToString();
            B2BDatagrid.CommitEdit();
        }

       
    }

}
