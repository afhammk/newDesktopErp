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
using System.Linq;

namespace MainMaterialApp.Masters.SalesB2C
{


    public partial class SalesB2C : UserControl
    {
        string branchid = "1";
        string counterid = "1";
        string salesId = "0";
        string status = "0";

        ObservableCollection<string> obj2 = new ObservableCollection<string>();
        ObservableCollection<Models.TableDataStructure> datagriditems = new ObservableCollection<Models.TableDataStructure>();
        Models.BasicInfo basicinfo = new Models.BasicInfo();
        QueryHandler.QueryHandler queryHandler = new QueryHandler.QueryHandler();
        ManualValidationHandler.ManualValidationHandler err = new ManualValidationHandler.ManualValidationHandler();
        Models.TableTotal totals = new Models.TableTotal();
        ObservableCollection<Models.Offers> offerlist = new ObservableCollection<Models.Offers>();

             
        public SalesB2C()
        {
            
            InitializeComponent();

            InsideDatagrid.ItemsSource = datagriditems;
            MobileTextBox.DataContext = basicinfo;
            NameTextBox.DataContext = basicinfo;
            PlaceTextBox.DataContext = basicinfo;
            TotalsGrid.DataContext = totals;

            obj2.Add("afham");
            obj2.Add("sanooj");
            obj2.Add("shameel");
            DgSalesmanColumn.ItemsSource = obj2;

            datagriditems.Add(new Models.TableDataStructure());

        }
        private void Screen_Loaded(object sender, RoutedEventArgs e)
        {
            //setInvoiceNo();
            //void setInvoiceNo()
            //{
            //    var invoiceNoResponse = queryHandler.HandleQuery("SELECT MAX(invoiceno + 1) as invno FROM public.salesinvoices WHERE invoicetype = 'BB' LIMIT 1", "select");
            //    if (invoiceNoResponse != null && invoiceNoResponse.HasValues)
            //    {
            //        status = "1";
            //        basicinfo.InvoiceNo = Convert.ToDouble(invoiceNoResponse[0]["invno"].ToString());
            //    }
            //}

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

                if (response!=null && response.HasValues)
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
            var row = datagriditems[e.Row.GetIndex()];
            if (e.Column.Header.ToString() == "Barcode")
            {
                var editedTextbox = e.EditingElement as TextBox;
                var editedText = editedTextbox.Text;

                Mouse.OverrideCursor = Cursors.Wait;
                var res = queryHandler.HandleQuery($"SELECT pd.barcode,pd.itemid,pd.variantid,pd.mrp,CONCAT(i.name,' ',iv.name) as item,i.categoryid,i.brandid,p.supplierid, CASE WHEN td.cgstrate IS NULL THEN 0 ELSE td.cgstrate END, CASE WHEN td.sgstrate IS NULL THEN 0 ELSE td.sgstrate END FROM public.purchaseitemdetails pd LEFT JOIN purchases p ON pd.purchaseid = p.id LEFT JOIN items i ON pd.itemid = i.id LEFT JOIN itemvariants iv ON pd.variantid = iv.id LEFT JOIN taxes t ON i.taxid = t.id LEFT JOIN taxdetails td ON t.id = td.taxid AND td.type= 'E' AND (pd.mrp between td.ratefrom AND td.rateto ) WHERE pd.barcode='{editedText}'", "select");
                Mouse.OverrideCursor = null;
    
                if (res!=null && res.HasValues)
                {

                    row.No = e.Row.GetIndex().ToString();
                    row.ItemName = res[0]["item"].ToString();
                    row.Barcode = res[0]["barcode"].ToString();
                    row.ItemId = res[0]["itemid"].ToString();
                    row.VariantId = res[0]["variantid"].ToString();
                    row.Mrp = String.IsNullOrEmpty(res[0]["mrp"].ToString()) ? "0" : res[0]["mrp"].ToString();
                    row.Cgst = "0";
                    row.Sgst = "0";
                    row.Discount = "0";
                    row.CgstDetails = res[0]["cgstrate"].ToString();
                    row.SgstDetails = res[0]["sgstrate"].ToString();
                    row.BrandId = res[0]["brandid"].ToString();
                    row.CategoryId = res[0]["categoryid"].ToString();

                    var getOffersQuery = queryHandler.HandleQuery($"SELECT og.id, og.name FROM public.offer_groups og LEFT JOIN offer_on_barcodes ob ON og.id = ob.offer_group_id WHERE og.deletedat IS NULL AND ob.barcode='{editedText}';", "select");
                    if(getOffersQuery!=null && getOffersQuery.HasValues) // for barcode based offers
                    {
                        offerlist.Clear();
                        foreach (var item in getOffersQuery)
                        {

                            offerlist.Add(new Models.Offers() { id = item["id"].ToString(), name = item["name"].ToString(), isSelected = false });
                        }
                        SubWindows.OfferSection offersection = new SubWindows.OfferSection(offerlist, (id) => getOfferConditions(id));
                        offersection.ShowDialog();

                        void getOfferConditions(string id){
                            
                            var listofconditions = new List<JArray>();
                            var offerConditionsQuery = new JArray();
                           
                            var res = queryHandler.HandleQuery($"SELECT o.id, o.group_id, o.value , gd.value as offvalue, gd.applying_mrp, gd.applicable_quantity, gd.max_discount_amount,CASE WHEN o.type = 's' THEN 'SHOP FOR' ELSE 'BUY' END as type, CASE WHEN o.get_type = 'f' THEN 'FLAT' WHEN o.get_type = 'p' THEN 'PERCENTAGE' ELSE 'BARCODE' END as get_type,(SELECT json_agg(t) FROM(select oo.id, (select array( SELECT ib.barcode FROM offer_cart_include_barcodes ib WHERE ib.offer_id = oo.id )) as barcodes,( select array( SELECT ibd.brand_id FROM offer_cart_include_brands ibd WHERE ibd.offer_id = oo.id )) as brands,(select array( SELECT ic.category_id FROM offer_cart_include_categories ic WHERE ic.offer_id = oo.id)) as categories,(select array( SELECT iv.vendor_id FROM offer_cart_include_vendors iv WHERE iv.offer_id = oo.id )) as vendors,( SELECT json_agg(m) FROM(SELECT im.mrp_from, im.mrp_to FROM offer_cart_include_mrp im WHERE im.offer_id = oo.id) m) as mrp from offers oo WHERE oo.id = o.id) t) as conditions FROM public.offers o LEFT JOIN offer_groups og ON o.group_id = og.id LEFT JOIN offer_get_discount gd ON o.id = gd.offer_id WHERE o.deletedat IS NULL AND og.deletedat IS NULL AND og.id = '{id}' ORDER BY o.value DESC;", "jsonInsideJArray");
                            offerConditionsQuery = res;
                            if (res != null && res.HasValues)
                            {
                                foreach (var conditions in offerConditionsQuery)
                                {
                                    listofconditions.Add(JArray.Parse(conditions["conditions"].ToString()));
                                }
                                //"conditions": "[{\"id\":46,\"barcodes\":[],\"brands\":[],\"categories\":[],\"vendors\":[],\"mrp\":[{\"mrp_from\":7000,\"mrp_to\":12000}]}]"
                            }
                            row.OfferId = id;
                            row.OfferConditions = offerConditionsQuery;
                            row.OfferConditionsDetails = listofconditions;
                            
                        }
                        //InsideDatagrid.CurrentCell = new DataGridCellInfo(InsideDatagrid.Items[e.Row.GetIndex()], InsideDatagrid.Columns[3]);
                        //InsideDatagrid.BeginEdit();

                    }
                    
                    else // for non barcode based offers ( brand or category)
                    {
                        row.OfferId = "";
                        return;
                    }

         
                    //InsideDatagrid.CurrentCell = new DataGridCellInfo(InsideDatagrid.Items[e.Row.GetIndex()], InsideDatagrid.Columns[3]);
                    //InsideDatagrid.BeginEdit();
                    //editedTextbox.Text = editedTextbox.Text;

                }
                else
                {
                    datagriditems[e.Row.GetIndex()].InvalidBarcode = true;
                    editedTextbox.Text = editedText;
                    e.Cancel = true;
                }
               
            }
            else if (e.Column.Header.ToString() == "Qty")
            {
                var editedTextbox = e.EditingElement as TextBox;
                var editedText = editedTextbox.Text;
                if (string.IsNullOrWhiteSpace(editedText))
                {
                    datagriditems[e.Row.GetIndex()].Qty = "";
                    e.Cancel = true;
                }
                else if (!editedText.All(c => char.IsDigit(c)))
                {
                    e.Cancel = true;
                }
                else
                {
                    row.Qty = Math.Round(Convert.ToDouble(editedText)).ToString();
                    row.Cgst = Math.Round((Convert.ToDouble(row.Qty) * Convert.ToDouble(row.Mrp) * (Convert.ToDouble(row.CgstDetails) / 100)), 2).ToString();
                    row.Sgst = Math.Round((Convert.ToDouble(row.Qty) * Convert.ToDouble(row.Mrp) * (Convert.ToDouble(row.SgstDetails) / 100)), 2).ToString();
                    row.Amount = Math.Round((Convert.ToDouble(row.Qty) * Convert.ToDouble(row.Mrp) + Convert.ToDouble(row.Cgst) + Convert.ToDouble(row.Sgst)), 2).ToString();
                    CalculateTotals();

                    if (datagriditems.Count == e.Row.GetIndex() + 1)
                    {
                        datagriditems.Add(new Models.TableDataStructure());
                    }
                  
                }
                CheckDiscountConditions();
            }
            //else if (e.Column.Header.ToString() == "Salesman")
            //{
            //    //InsideDatagrid.CurrentCell = new DataGridCellInfo(InsideDatagrid.Items[datagriditems.Count - 1], InsideDatagrid.Columns[1]);
            //    //InsideDatagrid.BeginEdit();
            //}

        }

        private void CheckDiscountConditions()
        {
            for (int i = 0; i < datagriditems.Count; i++)
            {
                var currentItem = datagriditems[i];
                if (String.IsNullOrWhiteSpace(currentItem.OfferId)==false)
                {
                    if (currentItem.OfferConditions[0]["get_type"].ToString() == "FLAT")
                    {
                        bool containsSameOfferId = false;
                        double totalItemAmount = Convert.ToDouble(currentItem.Amount);
                        double totalCount = Convert.ToDouble(currentItem.Qty);
                        for (int k = 0; k < datagriditems.Count; k++)  //loop to check if same offer id exist in datagrid items
                        {
                            if (i == k)
                            {
                                continue;
                            }
                            else
                            {
                                if (currentItem.OfferId == datagriditems[k].OfferId)
                                {
                                    containsSameOfferId = true;
                                    totalItemAmount = Convert.ToDouble(datagriditems[k].Amount);
                                    totalCount= Convert.ToDouble(datagriditems[k].Qty);
                                }
                            }
                        }
                        if (containsSameOfferId == true)
                        {
                            dynamic selectedOfferConditionIndex = null;
                            for (int j = 0; j < currentItem.OfferConditions.Count; j++)
                            {
                                var currentOffer = currentItem.OfferConditions[j];
                                
                                if (totalItemAmount > currentOffer["value"].ToObject<double>())
                                {
                                    selectedOfferConditionIndex = j;
                                    break;
                                }
                                
                            }
                            if (selectedOfferConditionIndex != null) // if offer exist 
                            {
                                var selectedOfferDetails = currentItem.OfferConditionsDetails[selectedOfferConditionIndex];
                                var selectedOffer =  currentItem.OfferConditions as JArray;
                                var categories = selectedOfferDetails[0]["categories"] as JArray;
                                var barcodes = selectedOfferDetails[0]["barcodes"] as JArray;
                                var brands = selectedOfferDetails[0]["brands"] as JArray;
                                var vendors = selectedOfferDetails[0]["vendors"] as JArray;

                                var dict = new Dictionary<string, JArray>();
                                if (categories.HasValues) dict.Add("categories", categories);
                                if (barcodes.HasValues) dict.Add("barcodes", barcodes);
                                if (brands.HasValues) dict.Add("brands", brands);
                                if (vendors.HasValues) dict.Add("vendors", vendors);

                                bool isValid = true;

                                if (dict.Count == 0) // no conditions present in offerconditiondetails
                                {
                                   isValid = true; 
                                }
                                else
                                {
                                    foreach (var dictitem in dict)
                                    {
                                        foreach (var value in dictitem.Value)
                                        {
                                            foreach (var item in datagriditems)
                                            {
                                                if (dictitem.Key == "barcodes")
                                                {
                                                    if (value.ToString() != item.Barcode)
                                                    {
                                                        isValid = false;

                                                    }
                                                }
                                                else if (dictitem.Key == "categories")
                                                {
                                                    if (value.ToString() != item.CategoryId)
                                                    {
                                                        isValid = false;
                                                    }
                                                }
                                                else if (dictitem.Key == "brands")
                                                {
                                                    if (value.ToString() != item.BrandId)
                                                    {
                                                        isValid = false;
                                                    }
                                                }
                                                else //dictitem.Key == "vendors"
                                                {
                                                    if (value.ToString() != item.SupplierId)
                                                    {
                                                        isValid = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                              
                                if (isValid == true)
                                {
                                    currentItem.Discount =  (selectedOffer[selectedOfferConditionIndex]["offvalue"].ToObject<double>() / totalCount).ToString(); // set discount
                                    MessageBox.Show("discount valid");
                                }
                                else
                                {
                                    MessageBox.Show("discount not valid");
                                }

                            }
                        }
                        else // containsSameOfferId == false
                        {
                            dynamic selectedOfferConditionIndex = null;
                            for (int j = 0; j < currentItem.OfferConditions.Count; j++)
                            {
                                var currentOffer = currentItem.OfferConditions[j];

                                if ( Convert.ToDouble(currentItem.Amount) > currentOffer["value"].ToObject<double>())
                                {
                                    selectedOfferConditionIndex = j;
                                    break;
                                }

                            }
                            if (selectedOfferConditionIndex != null) // if offer exist 
                            {
                                var selectedOfferDetails = currentItem.OfferConditionsDetails[selectedOfferConditionIndex];
                                var selectedOffer = currentItem.OfferConditions as JArray;
                                var categories = selectedOfferDetails[0]["categories"] as JArray;
                                var barcodes = selectedOfferDetails[0]["barcodes"] as JArray;
                                var brands = selectedOfferDetails[0]["brands"] as JArray;
                                var vendors = selectedOfferDetails[0]["vendors"] as JArray;

                                var dict = new Dictionary<string, JArray>();
                                if (categories.HasValues) dict.Add("categories", categories);
                                if (barcodes.HasValues) dict.Add("barcodes", barcodes);
                                if (brands.HasValues) dict.Add("brands", brands);
                                if (vendors.HasValues) dict.Add("vendors", vendors);

                                bool isValid = false;
                                int count = 0 ;

                                if (dict.Count == 0) // no conditions present in offerconditiondetails
                                {
                                    isValid = true;
                                }
                                else
                                {
                                    foreach (var dictitem in dict)
                                    {
                                        if (dictitem.Key == "barcodes")
                                        {
                                            count = 0;
                                            foreach (var value in dictitem.Value)
                                            {
                                                foreach(var item in datagriditems)
                                                {
                                                    if (value.ToString() == item.Barcode)
                                                    {
                                                        count++;
                                                        break;
                                                    }
                                                   
                                                }
                                            }
                                            if (dictitem.Value.Count == count)
                                            {
                                                isValid = true;
                                            }
                                            else
                                            {
                                                isValid = false;
                                                break;
                                            }
                                        }
                                        else if (dictitem.Key == "categories")
                                        {
                                            count = 0;
                                            foreach (var value in dictitem.Value)
                                            {
                                                foreach (var item in datagriditems)
                                                {
                                                    if (value.ToString() == item.CategoryId)
                                                    {
                                                        count++;
                                                        break;
                                                    }

                                                }
                                            }
                                            if (dictitem.Value.Count == count)
                                            {
                                                isValid = true;
                                            }
                                            else
                                            {
                                                isValid = false;
                                                break;
                                            }
                                        }
                                        else if (dictitem.Key == "brands")
                                        {
                                            count = 0;
                                            foreach (var value in dictitem.Value)
                                            {
                                                foreach (var item in datagriditems)
                                                {
                                                    if (value.ToString() == item.BrandId)
                                                    {
                                                        count++;
                                                        break;
                                                    }

                                                }
                                            }
                                            if (dictitem.Value.Count == count)
                                            {
                                                isValid = true;
                                            }
                                            else
                                            {
                                                isValid = false;
                                                break;
                                            }
                                        }
                                        else //dictitem.Key == "vendors"
                                        {
                                            count = 0;
                                            foreach (var value in dictitem.Value)
                                            {
                                                foreach (var item in datagriditems)
                                                {
                                                    if (value.ToString() == item.SupplierId)
                                                    {
                                                        count++;
                                                        break;
                                                    }

                                                }
                                            }
                                            if (dictitem.Value.Count == count)
                                            {
                                                isValid = true;
                                            }
                                            else
                                            {
                                                isValid = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (isValid == true)
                                {
                                    currentItem.Discount = (selectedOffer[selectedOfferConditionIndex]["offvalue"].ToString()); // set discount
                                    MessageBox.Show("discount valid");
                                }
                                else
                                {
                                    MessageBox.Show("discount not valid");
                                }

                            }
                        }
                    }
                    else if(currentItem.OfferConditions[0]["get_type"].ToString() == "PERCENTAGE")
                    {

                    }
                    else // currentItem.OfferConditions[0]["get_type"].ToString() == "BARCODE"
                    {

                    }               
                }
                else // if offerid not present
                {
                    currentItem.Discount = "0";
                }
            }
            
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
            var x = datagriditems;
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
