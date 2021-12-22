using Newtonsoft.Json.Linq;
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
using System.Windows.Shapes;

namespace MainMaterialApp.Masters.SalesB2C.SubWindows
{
    /// <summary>
    /// Interaction logic for OfferSection.xaml
    /// </summary>
    public partial class OfferSection : Window
    {
        private JArray response;
        ObservableCollection<Offers> offerlist = new ObservableCollection<Offers>();
        public OfferSection(JArray res1)
        {
            InitializeComponent();
            this.response = res1;
            OffersListBox.ItemsSource = offerlist;
            
            foreach (var item in res1)
            {
                //offerslist.add(item[0]["id"]
                offerlist.Add(new Offers() { id = item["id"].ToString(), name = item["name"].ToString()});
            }
        }
    }


    class Offers
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
