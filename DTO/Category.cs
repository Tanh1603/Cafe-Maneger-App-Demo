using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    internal class Category
    {
        private string name;
        private int iD;


        public string Name { get => name; set => name = value; }
        public int ID { get => iD; set => iD = value; }
        public Category() { }
        public Category(string name, int id)
        {
            Name = name;
            ID = id;
        }
        public Category(DataRow row)
        {
            Name = row["name"].ToString();
            ID = (int)row["id"];
        }
    }
}
