using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReverseKinematic
{
    public class Part:ViewModelBase
    {
        private bool isSelected = false;
        public string fullName;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        public Part()
        {

        }
        private ObservableCollection<Order> orderList = new ObservableCollection<Order>();

        public ObservableCollection<Order> OrderList
        {
            get { return orderList; }
            set
            {
                orderList = value;
                OnPropertyChanged(nameof(OrderList));
            }
        }

        public Part(string fullName)
        {
            this.fullName = fullName;
            var stringsList=fullName.Split(new[]{"_!#!_"}, StringSplitOptions.None);
            mass=Int32.Parse(stringsList[0])/1000; //Convert from mg to g
            MaterialEnumType.TryParse(stringsList[1],out material);
            name=stringsList[2];
            Surface = SurfaceEnumType.AnthracitePaint;

            //var o=new Order();
            //o.OrderNumber = "PK/233/219";
            //o.Date = "01.06.2019";
            //o.Quantity = "10";
            //o.Supplier = "CNC Polska";
            //o.Purchaser = "Piotr";


            //OrderList.Add(o);
        }


        
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private MaterialEnumType material;

        public MaterialEnumType Material
        {
            get { return material; }
            set { material = value; }
        }

        private SurfaceEnumType surface;

        public SurfaceEnumType Surface
        {
            get { return surface; }
            set { surface = value; }
        }

        private int mass;
        public int Mass
        {
            get { return mass; }
            set
            {
                mass = value;
                OnPropertyChanged(nameof(mass));
            }
        }
    }
}
