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
    class TableDataStructure : INotifyPropertyChanged , IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string id { get; set; }
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
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
                OnPropertyChanged();
            }
        }

        private string cgstdetails;
        public string CgstDetails
        {
            get { return cgstdetails; }
            set
            {
                cgstdetails = value;
                OnPropertyChanged();
            }
        }

        private string sgstdetails;
        public string SgstDetails
        {
            get { return sgstdetails; }
            set
            {
                sgstdetails = value;
                OnPropertyChanged();
            }
        }

        private string discountdetails;
        public string DiscountDetails
        {
            get { return discountdetails; }
            set
            {
                discountdetails = value;
                OnPropertyChanged();
            }
        }
        private string barcode;
        public string Barcode
        {
            get { return barcode; }
            set
            {
                barcode = value;
             
                OnPropertyChanged();
            }
        }

        private string hsn;
        public string HSN
        {
            get { return hsn; }
            set
            {
                hsn = value;

                OnPropertyChanged();
            }
        }

        private string item;
        public string ItemName
        {
            get { return item; }
            set
            {
                item = value;
                OnPropertyChanged();
            }
        }
        private string qty;
        public string Qty
        {
            get { return qty; }
            set
            {
                qty = value;
            
                OnPropertyChanged();
            }
        }

        private string mrp;
        public string Mrp
        {
            get { return mrp; }
            set
            {
                mrp = value;
                OnPropertyChanged();
            }
        }
        private string discount;
        public string Discount
        {
            get { return discount; }
            set
            {
                discount = value;
                OnPropertyChanged();
            }
        }

        private string cgst;
        public string Cgst
        {
            get { return cgst; }
            set
            {
                cgst = value;
                OnPropertyChanged();
            }
        }

        private string sgst;
        public string Sgst
        {
            get { return sgst; }
            set
            {
                sgst = value;
                OnPropertyChanged();
            }
        }


        private string amount;
        public string Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged();
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
        private ObservableCollection<Models.SalesmanOptionsDataStructure> salesmanoptions;
        public ObservableCollection<Models.SalesmanOptionsDataStructure> SalesmanOptions
        {
            get { return salesmanoptions; }
            set
            {
                salesmanoptions = value;
                OnPropertyChanged();
            }
        }

        private bool dataChanged = false;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
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
                        return "Barcode is empty";
                  
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
