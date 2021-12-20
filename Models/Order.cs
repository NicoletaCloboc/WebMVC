using System;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models
{
    [Serializable]
    public class Order
    {
        [Required]
        public string CustomerName{ get; set; }

        [EmailAddress]
        public string CustomerEmail{ get; set; }


        private short quantityField;

        public short Quantity
        {
            get { return quantityField; }
            set
            {
                if (value % 1000 == 0)
                    quantityField = value;
                else
                    throw new Exception(value.ToString() + " is invalid quantity");
            }
        }

        public string Notes{ get; set; }

        private float sizeField;
        public float Size
        {
            get { return sizeField; }
            set
            {
                if (value >= 11 && value <= 15 && value % 0.5 == 0)
                    sizeField = value;
                else
                    throw new Exception(value.ToString() + " is invalid size");
            }
        }

        private System.DateTime dateRequiredField;
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public DateTime DateRequired{
            get { return dateRequiredField; }
            set
            {
                if (value > DateTime.Now.AddDays(12) && GetBusinessDays(DateTime.Now, value) >= 10)
                    dateRequiredField = value;
                else
                    throw new Exception(value.ToShortDateString() + " invalid date");
            }
        }

        private static int GetBusinessDays(DateTime start, DateTime end)
        {
            if (start.DayOfWeek == DayOfWeek.Saturday)
            {
                start = start.AddDays(2);
            }
            else if (start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = start.AddDays(1);
            }

            if (end.DayOfWeek == DayOfWeek.Saturday)
            {
                end = end.AddDays(-1);
            }
            else if (end.DayOfWeek == DayOfWeek.Sunday)
            {
                end = end.AddDays(-2);
            }

            int diff = (int)end.Subtract(start).TotalDays;

            int result = diff / 7 * 5 + diff % 7;

            if (end.DayOfWeek < start.DayOfWeek)
            {
                return result - 2;
            }
            else
            {
                return result;
            }
        }
    }
}
