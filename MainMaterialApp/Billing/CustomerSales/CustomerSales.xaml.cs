using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainMaterialApp.Billing.CustomerSales
{
    /// <summary>
    /// Interaction logic for CustomerSales.xaml
    /// </summary>
    public partial class CustomerSales : UserControl
    {
        public CustomerSales()
        {
            InitializeComponent();

            ObservableCollection<DataGridItems> datagriditems = new ObservableCollection<DataGridItems>();

            InsideDatagrid.ItemsSource = datagriditems;

            /*------------------dummy data ---------------*/
            for (int i = 0; i < 3; i++)
            {
                datagriditems.Add(new DataGridItems()
                {
                    Amount = "12",
                    Barcode = "3223",
                    Cgst = "3232",
                    CgstDetails = "2332",
                    Salesman = "ansar mon",
                    Sgst = "2121",
                    SgstDetails = "3434",
                    Details = "Shirt Details",
                    Discount = "132",
                    Id = "23",
                    DiscountDetails = "422",
                    Item = "Shirt",
                    Mrp = "232",
                    No = "32",
                    Qty = "4343"

                });
            }
        }
    }
}
