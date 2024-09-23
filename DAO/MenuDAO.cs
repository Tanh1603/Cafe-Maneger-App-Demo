using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1.DAO
{
    internal class MENUDAO
    {
        private static MENUDAO instance;
        private MENUDAO() { }
        public static MENUDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MENUDAO();
                }
                return instance;
            }
            set { instance = value; }
        }

        public List<MENU> GetListMENUByTableId(int id)
        {
            List<MENU> listMENU = new List<MENU>();
            string query = $"SELECT Food.Name, BillInfo.Count, Food.Price, Food.Price * BillInfo.Count AS TotalPrice FROM dbo.BIllInfo, dbo.Bill, dbo.Food  \r\nWHERE BillInfo.IDBill = Bill.ID AND BillInfo.IDFood = Food.id AND Bill.IDTable = {id} AND Bill.Status = 0";
            DataTable dt = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow dr in dt.Rows)
            {
                listMENU.Add(new MENU(dr));

            }
            return listMENU;
        }
    }
}
