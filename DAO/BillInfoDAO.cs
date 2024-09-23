using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1.DAO
{
    internal class BillInfoDAO
    {
        private static BillInfoDAO instance;
        private BillInfoDAO() { }
        public static BillInfoDAO Instance { 
            get { 
                if (instance == null) instance = new BillInfoDAO(); 
                return instance; 
            }
            private set { instance = value; }
        }

        public List<BillInfo> GetListBillInfo(int id)
        {
            List<BillInfo> billInfo = new List<BillInfo>();
            string query = $"SELECT * FROM dbo.BIllInfo WHERE IDBill = {id}";
            DataTable dt = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow dr in dt.Rows)
            {
                BillInfo info = new BillInfo(dr);
                billInfo.Add(info);
            }
            return billInfo;
        }

        public void InsertBillInfo(int iDBill, int iDFood, int count)
        {
            string query = "USP_InsertBillInfo @idBill , @idFood , @count";
            DataProvider.Instance.ExecuteNumQuery(query, new object[] {iDBill, iDFood, count});
        }

        public void DeleteBillInfoByIdFood(int id)
        {
            string query = $"DELETE dbo.BillInfo WHERE IDFood = {id}";
            DataProvider.Instance.ExecuteQuery(query);
        }
    }
}
