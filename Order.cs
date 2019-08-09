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

        public Order(string date, string orderNumber, string quantity, string supplier, string purchaser)
        {
            Date = date;
            OrderNumber = orderNumber;
            Quantity = quantity;
            Supplier = supplier;
            Purchaser = purchaser;
        }
        public string Date
        {
            get { return date; }
            set
            {
                date = value;

            }
        }

        private string _orderNumber;
        public string OrderNumber
        {
            get { return _orderNumber; }
            set
            {
                _orderNumber = value;

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
        private string quantity;
        public string Quantity
        {
            get { return quantity; }
            set { quantity=value; }
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
