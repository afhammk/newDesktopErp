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

    
    
    public partial class SalesB2B : UserControl
    {
        int branchid = 1;
        int counterid = 1;

        QueryHandler.QueryHandler queryHandler = new QueryHandler.QueryHandler();
        ManualValidationHandler.ManualValidationHandler err = new ManualValidationHandler.ManualValidationHandler();
        ObservableCollection<Models.TableDataStructure> datagriditems = new ObservableCollection<Models.TableDataStructure>();
        ObservableCollection<Models.SalesmanOptionsDataStructure> salesmanOptions = new ObservableCollection<Models.SalesmanOptionsDataStructure>();  
        ObservableCollection<Models.PartyModel> partiesOptions = new ObservableCollection<Models.PartyModel>();
        Models.TableTotal totals = new Models.TableTotal();
        Models.BasicInfo basicinfo = new Models.BasicInfo();


        public SalesB2B()
        {
            InitializeComponent();

            B2BDatagrid.ItemsSource = datagriditems;
            TotalsGrid.DataContext = totals;
            PartySelectbox.ItemsSource = partiesOptions;
            InvoiceDatePicker.DataContext = basicinfo;
            PartySelectbox.DataContext = basicinfo;

            var invoiceNoResponse = queryHandler.HandleQuery("SELECT MAX(invoiceno + 1) as invno FROM public.salesinvoices WHERE invoicetype = 'BB' LIMIT 1", "select");

            if(invoiceNoResponse.HasValues)
                InvoiceNoNumericField.Value = Convert.ToDouble(invoiceNoResponse[0]["invno"].ToString());
            
            var response = queryHandler.HandleQuery("SELECT id,name,stateid FROM public.suppliers WHERE deletedat IS NULL", "select");

            if (response.HasValues)
            {
                foreach (var item in response)
                {
                    partiesOptions.Add(new Models.PartyModel() { Id = item["id"].ToString(), Name = item["name"].ToString(), StateId = item["stateid"].ToString() });

                }
            }
            
            datagriditems.Add(new Models.TableDataStructure());
        }


        private void B2BDatagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var row = datagriditems[e.Row.GetIndex()];
            if (e.Column.Header.ToString() == "Barcode")
            {
                var editedTextbox = e.EditingElement as TextBox;
                var editedText = editedTextbox.Text;

                Mouse.OverrideCursor = Cursors.Wait;
                var res = queryHandler.HandleQuery($"SELECT pd.barcode,pd.itemid,pd.variantid,pd.mrp,pd.cost as price,i.hsn,CONCAT(i.name,' ',iv.name) as item, CASE WHEN td.cgstrate IS NULL THEN 0 ELSE td.cgstrate END, CASE WHEN td.sgstrate IS NULL THEN 0 ELSE td.sgstrate END, CASE WHEN td.igstrate IS NULL THEN 0 ELSE td.igstrate END FROM public.purchaseitemdetails pd LEFT JOIN items i ON pd.itemid = i.id LEFT JOIN itemvariants iv ON pd.variantid = iv.id LEFT JOIN taxes t ON i.taxid = t.id LEFT JOIN taxdetails td ON t.id = td.taxid AND td.type= 'E' AND (pd.mrp between td.ratefrom AND td.rateto ) WHERE pd.barcode='C98562' ", "select");
                Mouse.OverrideCursor = null;
                
                if (res.HasValues)
                {

                   row.ItemName = res[0]["item"].ToString();
                   row.Barcode = res[0]["barcode"].ToString();
                   row.ItemId = res[0]["itemid"].ToString();
                   row.VariantId = res[0]["variantid"].ToString();
                   row.HSN = String.IsNullOrEmpty(res[0]["hsn"].ToString()) ? "0" : res[0]["hsn"].ToString();
                   row.Mrp = String.IsNullOrEmpty(res[0]["mrp"].ToString()) ? "0" : res[0]["mrp"].ToString();
                   row.Price = String.IsNullOrEmpty(res[0]["price"].ToString()) ? "0" : res[0]["price"].ToString();
                   row.Cgst = "0";
                   row.Sgst = "0";
                   row.Igst = "0";
                   row.CgstDetails = res[0]["cgstrate"].ToString();
                   row.SgstDetails = res[0]["sgstrate"].ToString();
                   row.IgstDetails = res[0]["igstrate"].ToString();
                    //datagriditems[e.Row.GetIndex()].Amount = res[0]["amount"].ToString();


                    B2BDatagrid.CurrentCell = new DataGridCellInfo(B2BDatagrid.Items[e.Row.GetIndex()], B2BDatagrid.Columns[4]);
                    B2BDatagrid.BeginEdit();
                    editedTextbox.Text = editedTextbox.Text;

                    if (datagriditems.Count == e.Row.GetIndex() + 1)
                    {
                        datagriditems.Add(new Models.TableDataStructure());
                    }
                }
                else
                {

                    e.Cancel = true;
                    err.Trigger(editedTextbox, "Barcode not found");
                }

            }
            else if (e.Column.Header.ToString() == "Qty")
            {
                var editedTextbox = e.EditingElement as TextBox;
                var editedText = editedTextbox.Text;
                if (string.IsNullOrWhiteSpace(editedText) || string.IsNullOrEmpty(datagriditems[e.Row.GetIndex()].Barcode))
                {
                    e.Cancel = true;
                }
                else
                {
                    row.Cgst = Math.Round((Convert.ToDouble(row.Qty) * Convert.ToDouble(row.Mrp) * (Convert.ToDouble(row.CgstDetails) / 100)),2).ToString(); 
                    row.Sgst = Math.Round((Convert.ToDouble(row.Qty) * Convert.ToDouble(row.Mrp) * (Convert.ToDouble(row.SgstDetails) / 100)),2).ToString(); 
                    row.Igst = Math.Round((Convert.ToDouble(row.Qty) * Convert.ToDouble(row.Mrp) * (Convert.ToDouble(row.IgstDetails) / 100)),2).ToString(); 
                    row.Amount = Math.Round((Convert.ToDouble(row.Qty) * Convert.ToDouble(row.Mrp) + Convert.ToDouble(row.Cgst) + Convert.ToDouble(row.Sgst)),2).ToString();
                    CalculateTotals();
                    //B2BDatagrid.CurrentCell = new DataGridCellInfo(B2BDatagrid.Items[e.Row.GetIndex()+1], B2BDatagrid.Columns[1]);
                    //B2BDatagrid.BeginEdit();
                    //editedTextbox.Text = editedTextbox.Text;

                }

            }

        }

        public void CalculateTotals()
        {
            double totalquantity = 0;
            double totalmrp = 0;
            double totalhsn = 0;
            double totalcgst = 0;
            double totalsgst = 0;
            double totalamount = 0;
            double totalprice = 0;
            foreach (var items in datagriditems)
            {
                totalquantity += Convert.ToDouble(string.IsNullOrEmpty(items.Qty) ? 0 : items.Qty);
                totalmrp += Convert.ToDouble(string.IsNullOrEmpty(items.Mrp) ? 0 : items.Mrp);
                totalprice += Convert.ToDouble(string.IsNullOrEmpty(items.Price) ? 0 : items.Price);
                totalhsn += Convert.ToDouble(string.IsNullOrEmpty(items.HSN) ? 0 : items.HSN);
                totalcgst += Convert.ToDouble(string.IsNullOrEmpty(items.Cgst) ? 0 : items.Cgst);
                totalsgst += Convert.ToDouble(string.IsNullOrEmpty(items.Sgst) ? 0 : items.Sgst);
                totalamount += Convert.ToDouble(string.IsNullOrEmpty(items.Amount) ? 0 : items.Amount);

            }
            totals.TotalQuantity = totalquantity.ToString();
            totals.TotalMrp = totalmrp.ToString();
            totals.TotalHsn = totalhsn.ToString();
            totals.TotalPrice = totalprice.ToString();
            totals.TotalCgst = totalcgst.ToString();
            totals.TotalSgst = totalsgst.ToString();
            totals.TotalAmount = totalamount.ToString();
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
            Mouse.OverrideCursor = Cursors.Wait;
            Task.Delay(1000).Wait();
            var res = queryHandler.HandleQuery("SELECT s.id, s.invoiceno, s.invoicedate, s.partyid, s.invoicetype, s.netamount,sl.id supplierid,sl.name suppliername,sl.stateid supplier_stateid FROM public.salesinvoices s LEFT JOIN suppliers sl ON s.partyid = sl.id WHERE s.invoiceno=27 AND s.branchid =1 AND s.counterid =1 AND s.invoicetype='BB' ", "select");
            Mouse.OverrideCursor = null;
            datagriditems.Clear();

            int index=0;
            for (int i = 0; i < partiesOptions.Count; i++)
            {
                if (partiesOptions[i].Id == res[0]["supplierid"].ToString())
                {
                    index = i;
                }
            }
            PartySelectbox.SelectedIndex = index;

            //Models.PartyModel obj = new Models.PartyModel();
            //obj.Id = res[0]["supplierid"].ToString();
            //obj.StateId = res[0]["supplier_stateid"].ToString();
            //obj.Name = res[0]["suppliername"].ToString();
            //PartySelectbox.SelectedItem = obj;
            //MessageBox.Show("success");

            //if (res.HasValues)
            //{
            //    try
            //    {
            //        foreach (var item in res)
            //        {
            //            datagriditems.Add(new Models.TableDataStructure()
            //            {

            //                ItemName = item["item"].ToString(),
            //                Barcode = item["barcode"].ToString(),
            //                HSN = item["hsn"].ToString(),
            //                Mrp = item["mrp"].ToString(),

            //                Cgst = item["cgst"].ToString(),
            //                Sgst = item["sgst"].ToString(),
            //                Amount = item["amount"].ToString()
            //            });

            //        }
            //    }
            //    catch (Exception error)
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {


            //if (basicinfo.PartySelected == null)
            //{
            //    basicinfo.PartySelected = null;
            //    return;
            //}
            //if (datagriditems.Count <= 1)
            //    return;
            //var partyid = PartySelectbox.SelectedItem as Models.PartyModel;
            //var partyidstring = partyid.Id.ToString();

            //var invDate = basicinfo.InvoiceDate.ToString("yyyy/MM/dd");

            //var res = queryHandler.HandleQuery($"INSERT INTO public.salesinvoices( invoiceno, invoicedate, partyid, invoicetype, netamount, branchid, counterid) VALUES((SELECT MAX(invoiceno + 1) as invno FROM public.salesinvoices WHERE invoicetype = 'BB' LIMIT 1), '{invDate}', '{partyidstring}', 'BB', '{totals.TotalAmount}', '{branchid}', '{counterid}') RETURNING id;", "insert");
            //if (res.HasValues == true)
            //{
            //    var query = getInsertQuery();
            //    var insertquery = queryHandler.HandleQuery(query, "insert");
            //    if (insertquery.HasValues)
            //        MessageBox.Show("success");
            //    else
            //        MessageBox.Show("error saving");
            //}
            //else
            //{
            //    MessageBox.Show("failure fetching id");

            //}
        }

        private string getInsertQuery()
        {
            var query = "INSERT INTO public.salesinvoicedetails(salesid, itemid, variantid, quantity, mrp, cost, rate, igstpercentage, igst, cgstpercentage, cgst, sgstpercentage, sgst, amount, barcode) VALUES";
            for (int i = 0; i < datagriditems.Count; i++)
            {
                var item = datagriditems[i];
                var str = $"(42,'{item.ItemId}','{item.VariantId}','{item.Qty}','{item.Mrp}','{item.Price}','{item.Price}','{item.Igst}','{item.IgstDetails}','{item.Cgst}','{item.CgstDetails}','{item.Sgst}','{item.SgstDetails}','{item.Amount}','{item.Barcode}'),";
                if (!String.IsNullOrWhiteSpace(item.Barcode) && !String.IsNullOrEmpty(item.Qty))
                {

                    query = query + str;
                }


            }
            query = query.Remove(query.Length - 1, 1);
            query = query + ";";
            return query;
        }

    }

}
