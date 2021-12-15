using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MainMaterialApp.Masters.SalesB2C.Models
{
    public class TableDataStructure : INotifyPropertyChanged, IDataErrorInfo
    {
       
        public event PropertyChangedEventHandler PropertyChanged;
        


        private string id { get; set; }
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
                
            }
        }
        private string no;
        public string No
        {
            get { return no; }
            set
            {
                no = value;
                OnPropertyChanged("No");
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
        private string qty ;
        public string Qty
        {
            get { return qty; }
            set
            {
                qty = value;          
                OnPropertyChanged("Qty");
                //OnPropertyChanged("Amount");
                //OnPropertyChanged("CgstDetails");
                //OnPropertyChanged("TotalQuantity");
                //OnPropertyChanged("TotalAmount");
                //OnPropertyChanged("TotalMrp");

            }
        }

        private string mrp ;
        public string Mrp
        {
            get { return mrp; }
            set
            {
                mrp = value;
                OnPropertyChanged("Mrp");
            }
        }
        private string discount ;
        public string Discount
        {
            get { return discount; }
            set
            {
                discount = value;
                OnPropertyChanged("Discount");
            }
        }

        private string cgst ;
        public string Cgst
        {
            get { return cgst; }
            set
            {
                cgst = value;
                OnPropertyChanged("Cgst");
            }
        }

        private string sgst ;
        public string Sgst
        {
            get { return sgst; }
            set
            {
                sgst = value;
                OnPropertyChanged("Sgst");
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

        private string salesman ;
        public string Salesman
        {
            get { return salesman; }
            set
            {
                salesman = value;
                OnPropertyChanged("Salesman");
            }
        }

      

        public bool datachanged = false;
        protected void OnPropertyChanged(string name)
        {
            datachanged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }




        /*----------------------------Error Validation---------------------------------*/

        public bool InvalidBarcode = false;
       

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
                    if (String.IsNullOrWhiteSpace(Barcode) && datachanged)
                    {
                        return "Barcode is empty";
                    } 
                    else if(InvalidBarcode == true)
                    {
                        InvalidBarcode = false;
                        return "Invalid Barcode";
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "InvalidBarcode":
                    if (InvalidBarcode==true)
                    {
                        return "Barcode is Invalid";
                    }
                    else
                    {
                        return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

    }
}

