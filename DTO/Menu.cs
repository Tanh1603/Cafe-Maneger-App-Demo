using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    internal class MENU
    {
        private int count;
        private string foodName;
        private float price;
        private float totalPrice;

        public int Count { get => count; set => count = value; }
        public string FoodName { get => foodName; set => foodName = value; }
        public float Price { get => price; set => price = value; }
        public float TotalPrice { get => totalPrice; set => totalPrice = value; }

        public MENU() { }
        public MENU(int count, string foodName, float price, float totalPrice = 0)
        {
            Count = count;
            FoodName = foodName;
            Price = price;
            TotalPrice = totalPrice;
        }
        public MENU(DataRow row)
        {
            FoodName = row["Name"].ToString();
            Count = (int)row["Count"];
            Price = (float)Convert.ToDouble(row["Price"].ToString());
            TotalPrice = (float)Convert.ToDouble(row["TotalPrice"].ToString());
        }
    }
}
