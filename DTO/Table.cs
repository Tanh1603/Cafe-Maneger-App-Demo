using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    public class Table
    {
        // Attribute
        private int iD;
        private string name;
        private string status;
        // Method get/set
        public int ID { get => iD; set => iD = value; }
        public string Name { get => name; set => name = value; }
        public string Status { get => status; set => status = value; }
        // Contructor
        public Table() { }
        public Table(int id, string name, string status)
        {
            this.iD = id;
            this.name = name;
            this.status = status;
        }

        //
        public Table(DataRow row)
        {
            this.iD = (int)row["ID"];
            this.name = row["Name"].ToString();
            this.status = row["Status"].ToString();

        } 

        public void GetListTable()
        {

        }
    }
}
