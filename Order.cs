using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseKinematic
{
    public class Order
    {
        private string date;
        public string Date
        {
            get { return date; }
            set
            {
                date = value;

            }
        }

        private string number;
        public string Number
        {
            get { return number; }
            set
            {
                number = value;

            }
        }

        private string supplier;
        public string Supplier
        {
            get { return supplier; }
            set
            {
                supplier = value;

            }
        }

        private string purchaser;
        public string Purchaser
        {
            get { return purchaser; }
            set
            {
                purchaser = value;

            }
        }
        private int quantity;
        public string Quantity
        {
            get { return quantity.ToString(); }
            set { quantity = int.Parse(value); }
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;

            }
        }

        private StatusEnumType status1 = StatusEnumType.Painting;
        public StatusEnumType Status1
        {

            get { return status1; }
            set
            {
                status1 = value;
            }
        }


    }
}
