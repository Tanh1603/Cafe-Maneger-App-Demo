using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DTO
{
    public class Account
    {
        private string userName;
        private string displayName;
        private string password;
        private int type;

        public string UserName { get => userName; set => userName = value; }
        public string DisplayName { get => displayName; set => displayName = value; }
        public string Password { get => password; set => password = value; }
        public int Type { get => type; set => type = value; }
        public Account() { }
        public Account(string userName, string displayName, int type, string pass = null)
        {
            UserName = userName;
            DisplayName = displayName;
            Password = pass;
            Type = type;
        }
        public Account(DataRow row)
        {
            UserName = row["UserName"].ToString();
            DisplayName = row["DisplayName"].ToString();
            Password = row["PassWord"].ToString();
            Type = (int)row["Type"];

        }
    }
}
