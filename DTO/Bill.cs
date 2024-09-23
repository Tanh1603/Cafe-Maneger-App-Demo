using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    internal class Bill
    {
        private int iD;
        private DateTime? dateCheckIn;
        private DateTime? dateCheckOut;
        private int status;
        private int discount;
        private float totalPrice;
        public int ID { get => iD; set => iD = value; }
        public DateTime? DateCheckIn { get => dateCheckIn; set => dateCheckIn = value; }
        public DateTime? DateCheckOut { get => dateCheckOut; set => dateCheckOut = value; }
        public int Status { get => status; set => status = value; }
        public int Discount { get => discount; set => discount = value; }
        public float TotalPrice { get => totalPrice; set => totalPrice = value; }

        public Bill() { }
        public Bill(int iD, DateTime? dateCheckIn, DateTime? dateCheckOut, int status, int discount = 0, float totalPrice = 0)
        {
            ID = iD;
            DateCheckIn = dateCheckIn;
            DateCheckOut = dateCheckOut;
            Status = status;
            Discount = discount;
            TotalPrice = totalPrice;
        }
        public Bill(DataRow row)
        {
            ID = (int)row["ID"];
            DateCheckIn = (DateTime?)row["DateCheckIn"];

            var dataCheckOutTemp = row["DateCheckOut"];
            if(dataCheckOutTemp.ToString() != "")
            {
                DateCheckOut = (DateTime?)row["DateCheckOut"];
            }else
            Status = (int)row["Status"];
            if (row["Discount"].ToString() != "")
            {
                Discount = (int)row["Discount"];
            }
            TotalPrice = (float)Convert.ToDouble(row["TotalPrice"].ToString());
        }
    }
}
