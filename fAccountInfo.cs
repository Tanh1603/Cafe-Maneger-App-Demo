using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1
{
    public partial class fAccountInfo : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get => loginAccount;
            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount);
            }
        }
        public fAccountInfo(Account acc)
        {
            InitializeComponent();
            LoginAccount = acc;
        }
        void ChangeAccount(Account acc)
        {
            txbUserName.Text = acc.UserName;
            txbDisplayName.Text = acc.DisplayName;
        }
        void UpdateAccount()
        {
            string displayName = txbDisplayName.Text;
            string passWord = txbPassWord.Text;
            string newPass = txbNewPass.Text;
            string reEnterPass = txbReEnterPass.Text;
            string userName = txbUserName.Text;

            if( !newPass.Equals(reEnterPass))
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu đúng với mật khẩu mới!");
            }
            else
            {
                if(AccountDAO.Instance.UpdateAccountPassWord(userName, displayName, passWord, newPass))
                {
                    MessageBox.Show("Cập nhật thành công");
                    if(eventUpdateAccount != null)
                    {
                        eventUpdateAccount(this, new AccountEvent(AccountDAO.Instance.GetAccountByUserName(userName)));
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng điền đúng mật khẩu");
                }
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccount();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private event EventHandler<AccountEvent> eventUpdateAccount;
        public event EventHandler<AccountEvent> EventUpdateAccount
        {
            add
            {
                eventUpdateAccount += value;
            }
            remove
            {
                eventUpdateAccount -= value;
            }
        }

        // event cấp 2
        public class AccountEvent: EventArgs
        {
            private Account account;

            public Account Account { get => account; set => account = value; }

            public AccountEvent(Account account)
            {
                Account = account;
            }
        }
    }
}
