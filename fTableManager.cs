using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.DTO;
using static WindowsFormsApp1.fAccountInfo;

namespace WindowsFormsApp1
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount 
        { 
            get => loginAccount;
            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount.Type);
            } 
        }

        public fTableManager() { }
        public fTableManager(Account acc)
        {
            InitializeComponent();
            LoginAccount = acc;

            LoadTable();
            LoadCategory();
            LoadTableInCbSwitchTable();
        }

        #region Method
        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tb = TableDAO.Instance.LoadTableList();
            foreach (Table t in tb)
            {
                Button btn = new Button();
                btn.Width = TableDAO.tableWidth;
                btn.Height = TableDAO.tableHeight;
                btn.Text = t.Name + "\n" + t.Status;
                btn.Click += Btn_Click;
                btn.Tag = t;

                switch (t.Status)
                {
                    case "Có người":
                        btn.BackColor = Color.DarkSlateGray;
                        btn.ForeColor = Color.Wheat;
                        break;
                    case "Bàn đặt":
                        btn.BackColor = Color.Gray;
                        btn.ForeColor = Color.DarkGray;
                        btn.Enabled = false;
                        break;
                    default:
                        btn.BackColor = Color.DarkTurquoise;
                        break;
                }
                flpTable.Controls.Add(btn);
            }

        }
        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategories();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }
        void LoadFoodListByCategoryId(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodCategoryById(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";

        }
        void LoadTableInCbSwitchTable()
        {
            List<Table> listTable = TableDAO.Instance.LoadTableList();
            cbSwitchTable.DataSource = listTable;
            cbSwitchTable.DisplayMember = "Name";
        }
        void ShowBill(int id)
        {
            try
            {
                lsvBill.Items.Clear();
                float totalPrice = 0;
                List<MENU> ListMenu = MENUDAO.Instance.GetListMENUByTableId(id);
                foreach (MENU menu in ListMenu)
                {
                    ListViewItem listViewItem = new ListViewItem(menu.FoodName.ToString());
                    listViewItem.SubItems.Add(menu.Count.ToString());
                    listViewItem.SubItems.Add(menu.Price.ToString());
                    listViewItem.SubItems.Add(menu.TotalPrice.ToString());

                    lsvBill.Items.Add(listViewItem);
                    totalPrice += menu.TotalPrice;
                }
                CultureInfo culture = new CultureInfo("vi-VN");
                txbTotalPrice.Text = totalPrice.ToString("c", culture);
                LoadTable();
            }catch
            {
                return;
            }
        }

        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += $" ({loginAccount.DisplayName})";
        }
        #endregion

        #region Events
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTínCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountInfo f = new fAccountInfo(loginAccount);
            f.EventUpdateAccount += F_eventUpdateAccount;
            f.ShowDialog();

        }
        // Chuyển thông tin ngược về form cha
        private void F_eventUpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = $"Thông tin tài khoản ({e.Account.DisplayName})";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin  f = new fAdmin();
            f.loginAccount = loginAccount;
            f.InsertFood += F_InsertFood;
            f.UpdateFood += F_UpdateFood;
            f.DeleteFood += F_DeleteFood;

            f.InsertCategory += F_InsertCategory;
            f.UpdateCategory += F_UpdateCategory;
            f.DeleteCategory += F_DeleteCategory;

            f.InsertTable += F_InsertTable;
            f.UpdateTable += F_UpdateTable;
            f.DeleteTable += F_DeleteTable;
            f.ShowDialog();
        }

        private void F_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void F_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void F_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void F_DeleteCategory(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryId((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);

            LoadTable();
        }

        private void F_UpdateCategory(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryId((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void F_InsertCategory(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryId((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            int idTable = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(idTable);   
        }
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null) return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryId(id);
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            if(table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            }
            int idBill = BillDAO.Instance.GetUnCheckBillByTableID(table.ID);
            if(cbFood.SelectedItem == null)
            {
                MessageBox.Show("Danh mục này chưa có món vui lòng chọn lại danh mục khác");
                return;
            }
            int iDFood = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFood.Value;

            if(idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                int maxID = BillDAO.Instance.GetMaxIdBill();
                BillInfoDAO.Instance.InsertBillInfo(maxID, iDFood, count);
            }else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, iDFood, count);

            }
            ShowBill(table.ID);
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int iDBill = BillDAO.Instance.GetUnCheckBillByTableID(table.ID);
            int discount = (int)nmDiscount.Value;

            string totalPriceText = txbTotalPrice.Text.Split(',')[0]; // Lấy giá trị trước dấu phẩy
            double totalPrice = double.Parse(totalPriceText, NumberStyles.Currency, CultureInfo.GetCultureInfo("vi-VN"));
            double finalToTalPrice = totalPrice - (totalPrice/100)*discount;

            if(iDBill != -1)
            {
                if(MessageBox.Show($"Bạn có muốn thanh toán cho bàn {table.Name}", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(iDBill, discount, (float)finalToTalPrice);
                    ShowBill(table.ID);
                    LoadTable();
                }
            }

        }
        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            ComboBox cb = (cbSwitchTable as ComboBox);
            int idTable1 = (lsvBill.Tag as Table).ID;
            int idTable2 = (cb.SelectedItem as Table).ID;
            if (idTable1 == idTable2) return;

            TableDAO.Instance.SwitchTable(idTable1 , idTable2);
            LoadTable();
            ShowBill(idTable1);

        }

        private void F_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryId((cbCategory.SelectedItem as Category).ID);
            if(lsvBill.Tag != null) 
                ShowBill((lsvBill.Tag as Table).ID);

            LoadTable();
        }

        private void F_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryId((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null) 
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void F_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryId((cbCategory.SelectedItem as Category).ID);
            if(lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }
        #endregion

    }
}
