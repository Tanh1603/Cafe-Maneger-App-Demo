using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    internal class Food
    {
        private float price;
        private int iD;
        private string name;
        private int iDCategory;

        public float Price { get => price; set => price = value; }
        public int ID { get => iD; set => iD = value; }
        public string Name { get => name; set => name = value; }
        public int IDCategory { get => iDCategory; set => iDCategory = value; }

        public Food() { }
        public Food(float price, int iD, string name, int iDCategory)
        {
            Price = price;
            ID = iD;
            Name = name;
            IDCategory = iDCategory;
                

        }
        public Food(DataRow row)
        {
            ID = (int)row["ID"];
            IDCategory = (int)row["IDCategory"];
            Name = (string)row["Name"];
            Price = (float)Convert.ToDouble(row["Price"].ToString());
        }
    }
}
