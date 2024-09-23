using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1.DAO
{
    internal class BillDAO
    {
        private static BillDAO instance;
        private BillDAO() { }
        public static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return instance; }
            private set { instance = value; }
        }

        public int GetUnCheckBillByTableID(int id)
        {
            string query = $"SELECT * FROM dbo.Bill WHERE IDTable = {id} AND Status = 0";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            if(data.Rows.Count > 0 )
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            else
            {
                return -1;
            }
        }

        public void InsertBill(int id)
        {
            string query = "USP_InsertBill @idTable";
            DataProvider.Instance.ExecuteNumQuery(query, new object[] {id});
        }

        public int GetMaxIdBill()
        {
            string query = "SELECT MAX(ID) FROM dbo.Bill";
            return (int)DataProvider.Instance.ExecuteScalar(query);
        }

        public void CheckOut(int id, int discount, float totalPrice)
        {
            string query = $"UPDATE dbo.Bill SET Status = 1, TotalPrice = {totalPrice}, Discount = {discount}, DateCheckOut = GETDATE()  WHERE ID = {id}";
            DataProvider.Instance.ExecuteNumQuery(query);
        }

        public DataTable GetBillListByDate(DateTime checkIn, DateTime checOut)
        {
            string query = $"USP_GetlistBillByDate @checkIn , @checkOut";
            DataTable dt = DataProvider.Instance.ExecuteQuery(query, new object[] {checkIn, checOut});
            return dt;
        }
    }
}
