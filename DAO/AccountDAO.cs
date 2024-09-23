using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1.DAO
{
    internal class AccountDAO
    {
        // Thiết lập Singleton
        private static AccountDAO instance;
        private AccountDAO() { }
        public static AccountDAO Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new AccountDAO();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }
        // End thiết lập

        public bool Login(string username, string password)
        {
            string query = $"USP_Login @userName , @passWord";

            DataTable res = DataProvider.Instance.ExecuteQuery(query, new object[] {username, password});

            return res.Rows.Count > 0;
        }

        public Account GetAccountByUserName(string username)
        {
            string query = $"SELECT * FROM dbo.Account WHERE UserName = '{username}'";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow row in data.Rows)
            {
                return new Account(row);
            }

            return null;
        }

        public DataTable GetListAccount()
        {
            string query = $"SELECT UserName, DisplayName, Type FROM dbo.Account";
            return DataProvider.Instance.ExecuteQuery(query);
        }
        public bool UpdateAccountPassWord(string userName, string displayName, string pass, string newPass)
        {
            string query = "USP_UpdateAccount @userName , @displayName , @passWord , @newPassWord";
            int res = DataProvider.Instance.ExecuteNumQuery(query, new object[] {userName, displayName, pass, newPass});

            return res > 0; 
        }

        // Thêm sửa xóa
        public bool InsertAccount(string userName, string displayName, int type)
        {
            string query = $"INSERT INTO dbo.Account (UserName, DisplayName, Type) VALUES (N'{userName}', N'{displayName}', {type})";
            int res = DataProvider.Instance.ExecuteNumQuery(query);

            return res > 0;
        }
        public bool UpdateAccount(string userName, string displayName, int type)
        {
            string query = $"UPDATE dbo.Account SET  DisplayName = N'{displayName}', Type = {type} WHERE UserName = N'{userName}'";
            int res = DataProvider.Instance.ExecuteNumQuery(query);

            return res > 0;
        }
        public bool DeleteAccount(string userName)
        {
            string query = $"DELETE dbo.Account WHERE UserName = N'{userName}'";
            int res = DataProvider.Instance.ExecuteNumQuery(query);

            return res > 0;
        }
        public bool ResetPassWord(string userName)
        {

            string query = $"UPDATE dbo.Account SET PassWord = N'{1}' WHERE UserName = N'{userName}'";
            int res = DataProvider.Instance.ExecuteNumQuery(query);

            return res > 0;
        }

    }
}
