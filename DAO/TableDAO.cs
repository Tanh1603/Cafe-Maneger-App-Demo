using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1.DAO
{
    internal class TableDAO
    {
        private static TableDAO instance;
        public static int tableWidth = 90;
        public static int tableHeight = 90;
        private TableDAO() { }
        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return instance; }
            private set { instance = value; }
        }

        // Hàm
        public List<Table> LoadTableList()
        {
            List<Table> listTable = new List<Table>();

            DataTable dt = DataProvider.Instance.ExecuteQuery("USP_GetTableList");
            foreach (DataRow dr in dt.Rows)
            {
                Table t = new Table(dr);
                listTable.Add(t);
            }
            return listTable;
        }
        public void SwitchTable(int id1, int id2)
        {
            string query = "USP_SwitchTable @iDTable1 , @iDTable2 ";
            DataProvider.Instance.ExecuteQuery(query, new object[] { id1, id2 });
        }

        // Change db (add, delete, update, show)

        public bool InsertTable(string name, string status = "Trống")
        {
            string query = $"INSERT INTO dbo.TableInfo(Name, Status) VALUES(N'{name}', N'{status}')";
            int res = DataProvider.Instance.ExecuteNumQuery(query);
            return res > 0;
        }

        public bool UpdateTable(string name, string status, int id)
        {
            string query = $"UPDATE dbo.TableInfo SET Name = N'{name}', Status = N'{status}' WHERE ID = {id}";
            int res = DataProvider.Instance.ExecuteNumQuery(query);
            return res > 0;
        }

        public bool DeleteTable(int id)
        {
            string query = $"DELETE FROM dbo.TableInfo WHERE ID = {id}";
            int res = DataProvider.Instance.ExecuteNumQuery(query) ; 
            return res > 0;
        }

    }
}
