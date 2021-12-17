using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace MainMaterialApp.Masters.SalesB2B.Models
{
    public class TableDataStructure : INotifyPropertyChanged , IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string itemid { get; set; }
        public string ItemId
        {
            get { return itemid; }
            set
            {
                itemid = value;
                OnPropertyChanged();
            }
        }
        private string variantid { get; set; }
        public string VariantId
        {
            get { return variantid; }
            set
            {
                variantid = value;
                OnPropertyChanged();
            }
        }
        private string no;
        public string No
        {
            get { return no; }
            set
            {
                no = value;
                OnPropertyChanged();
            }
        }


        private string details;
        public string Details
        {
            get { return details; }
            set
            {
                details = value;
                OnPropertyChanged("Details");
            }
        }

        private string cgstdetails;
        public string CgstDetails
        {
            get { return cgstdetails; }
            set
            {
                cgstdetails = value;
                OnPropertyChanged("CgstDetails");
            }
        }

        private string sgstdetails;
        public string SgstDetails
        {
            get { return sgstdetails; }
            set
            {
                sgstdetails = value;
                OnPropertyChanged("SgstDetails");
            }
        }

        private string igstdetails;
        public string IgstDetails
        {
            get { return igstdetails; }
            set
            {
                igstdetails = value;
                OnPropertyChanged("IgstDetails");
            }
        }

        private string discountdetails;
        public string DiscountDetails
        {
            get { return discountdetails; }
            set
            {
                discountdetails = value;
                OnPropertyChanged("DiscountDetails");
            }
        }
        private string barcode;
        public string Barcode
        {
            get { return barcode; }
            set
            {
                barcode = value;
             
                OnPropertyChanged("Barcode");
            }
        }

        private string hsn;
        public string HSN
        {
            get { return hsn; }
            set
            {
                hsn = value;

                OnPropertyChanged("HSN");
            }
        }

        private string itemname;
        public string ItemName
        {
            get { return itemname; }
            set
            {
                itemname = value;
                OnPropertyChanged("ItemName");
            }
        }
        private string qty;
        public string Qty
        {
            get { return qty; }
            set
            {
                qty = value;
            
                OnPropertyChanged("Qty");
            }
        }

        private string mrp;
        public string Mrp
        {
            get { return mrp; }
            set
            {
                mrp = value;
                OnPropertyChanged("Mrp");
            }
        }

        private string price;
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }

        private string cgst;
        public string Cgst
        {
            get { return cgst; }
            set
            {
                cgst = value;
                OnPropertyChanged("Cgst");
            }
        }

        private string sgst;
        public string Sgst
        {
            get { return sgst; }
            set
            {
                sgst = value;
                OnPropertyChanged("Sgst");
            }
        }
        private string igst;
        public string Igst
        {
            get { return igst; }
            set
            {
                igst = value;
                OnPropertyChanged("Igst");
            }
        }

        private string amount;
        public string Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged("Amount");
            }
        }
        private string salesman;
        public string Salesman
        {
            get { return salesman; }
            set
            {
                salesman = value;
                OnPropertyChanged();
            }
        }
      

        private bool dataChanged = false;
        protected void OnPropertyChanged(string name = null)
        {
            dataChanged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /*----------------------------Error Validation---------------------------------*/

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                return GetErrorForProperty(propertyName);
            }
        }

        private string GetErrorForProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Barcode":
                    if (String.IsNullOrWhiteSpace(Barcode) && dataChanged)
                        return "Barcode required";
                  
                    else
                    {
                        return string.Empty;
                    }
                case "Qty":
                    if (Qty!=null && !Qty.All(c => char.IsDigit(c)))
                    {
                        return "Qty only numbers";

                    }
                    else
                    {
                        return string.Empty;
                    }

                //    if (String.IsNullOrWhiteSpace(Qty) && dataChanged)
                //        return "Qty required";

                //    else if (!double.TryParse(Qty, out double i) && dataChanged)
                //    {
                //        return "only characters";
                //    }

                //    else
                //    {
                //        return string.Empty;
                //    }



                default:
                    return string.Empty;
            }
        }
    }
}
