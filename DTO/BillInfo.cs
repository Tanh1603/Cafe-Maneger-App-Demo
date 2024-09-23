using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    internal class BillInfo
    {
        private int iD;
        private int iDBill;
        private int iDFood;
        private int count;
        
        public int ID { get => iD; set => iD = value; }
        public int IDBill { get => iDBill; set => iDBill = value; }
        public int IDFood { get => iDFood; set => iDFood = value; }
        public int Count { get => count; set => count = value; }

        public BillInfo() { }
        public BillInfo(int iD, int idBill, int iDFood, int count)
        {
            ID = iD;
            IDBill = idBill;
            IDFood = iDFood;
            Count = count;
        }
        public BillInfo(DataRow dr)
        {
            ID = (int)dr["ID"];
            IDBill = (int)dr["IDBill"];
            IDFood = (int)dr["IDFood"];
            Count = (int)dr["Count"];
        }
    }
}
