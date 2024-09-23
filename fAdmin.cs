using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource tableList = new BindingSource();
        public Account loginAccount;

        public fAdmin()
        {
            InitializeComponent();
            LOAD();
        }
        //Hàm load tất cả tài nguyên cho trang
        void LOAD()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            dtgvCategory.DataSource = categoryList;
            dtgvTable.DataSource = tableList;

            LoadDateTimePicker();
            LoadListBillByDate(dtpStart.Value, dtpEnd.Value);
            
            AddFoodBinding();
            LoadListFood();

            AddAccountBinding();
            LoadListAccount();

            AddCategoryBinding();
            LoadListCategory();

            AddTableBinding();
            LoadListTableFood();
        }
        #region METHOD
        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);
            return listFood;   
        }
        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();

            if (!dtgvFood.Columns.Contains("CategoryName"))
            {
                dtgvFood.Columns.Add("CategoryName", "Tên danh mục");
            }
            dtgvFood.Columns["ID"].DisplayIndex = 0;
            dtgvFood.Columns["CategoryName"].DisplayIndex = 1;
            dtgvFood.Columns["Name"].DisplayIndex = 2;


            dtgvFood.Columns["ID"].HeaderText = "ID";
            dtgvFood.Columns["IDCategory"].Visible = false;
            dtgvFood.Columns["Name"].HeaderText = "Tên món";
            dtgvFood.Columns["Price"].HeaderText = "Giá";

            dtgvFood.DataBindingComplete += DtgvFood_DataBindingComplete;
        }

        private void DtgvFood_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dtgvFood.Rows)
            {
                if (row.Cells["IDCategory"].Value != null)
                {
                    int idCategory = Convert.ToInt32(row.Cells["IDCategory"].Value);
                    string categoryName = CategoryDAO.Instance.GetCategoryById(idCategory).Name;
                    row.Cells["CategoryName"].Value = categoryName;
                }
            }
        }

        void LoadListAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();

            dtgvAccount.Columns["UserName"].HeaderText = "Tên tài khoản";
            dtgvAccount.Columns["DisplayName"].HeaderText = "Tên hiển thị";
            dtgvAccount.Columns["Type"].HeaderText = "Loại tài khoản";

            
        }
        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategories();

            dtgvCategory.Columns["ID"].DisplayIndex = 0;

            dtgvCategory.Columns["Name"].HeaderText = "Tên danh mục";


        }
        void LoadListTableFood()
        {
            tableList.DataSource = TableDAO.Instance.LoadTableList();
        }

        void LoadDateTimePicker()
        {
            DateTime dateTime = DateTime.Now;
            dtpStart.Value = new DateTime(dateTime.Year, dateTime.Month, 1);
            dtpEnd.Value = dtpStart.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvRevenue.DataSource =  BillDAO.Instance.GetBillListByDate(checkIn, checkOut);

        }
        void LoadCategoryIntoComboBox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategories();
            cb.DisplayMember = "Name";
        }
        // Fn add binding
        void AddFoodBinding()
        {
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            nmPriceFood.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));

            LoadCategoryIntoComboBox(cbFoodIDCategory);

        }
        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));

            cbAccountType.DataSource = new List<string> { "Admin", "Staff" };
        }
        void AddCategoryBinding()
        {
            txbCategoryID.DataBindings.Add("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never);
            txbNameCategory.DataBindings.Add("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never);
        }
        void AddTableBinding()
        {
            txbTableID.DataBindings.Add("Text", dtgvTable.DataSource, "ID",true, DataSourceUpdateMode.Never);
            txbTableName.DataBindings.Add("Text", dtgvTable.DataSource, "Name",true, DataSourceUpdateMode.Never);
            txbStatusTable.DataBindings.Add("Text", dtgvTable.DataSource, "Status",true, DataSourceUpdateMode.Never);

        }
        // End add binding
        // Fn Change Account (add, delete, reset)
        void AddAccount(string userName, string displayName, int type)
        {
            if(AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }
            LoadListAccount();
        }
        void UpdateAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }
            LoadListAccount();
        }
        void DeleteAccount(string userName)
        {
            if(loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Vui lòng đừng xóa chính bạn");
                return;
            }

            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }
            LoadListAccount();
        }
        void ResetPassWord(string userName)
        {
            if (AccountDAO.Instance.ResetPassWord(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công");
            }else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
        }
        // End change account

        #endregion

        #region EVENT
        private void button3_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpStart.Value, dtpEnd.Value);
        }
        private void dtgvRevenue_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtgvRevenue.Columns[e.ColumnIndex].Name == "Giảm giá")
            {
                // Kiểm tra nếu giá trị không phải null
                if (e.Value != null)
                {
                    // Chuyển đổi giá trị thành kiểu số và thêm dấu %
                    e.Value = e.Value.ToString() + " %";
                    e.FormattingApplied = true; // Đánh dấu là đã áp dụng định dạng
                }
            }
        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    object obj = dtgvFood.SelectedCells[0].OwningRow.Cells["IDCategory"].Value;
                    if (obj == null) return;
                    int id = (int)obj;
                    Category category = CategoryDAO.Instance.GetCategoryById(id);
                    cbFoodIDCategory.SelectedItem = category;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodIDCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbFoodIDCategory.SelectedIndex = index;
                }
            } catch { }  
        }

        private void txbUserName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvAccount.SelectedCells.Count > 0)
                {
                    // Lấy giá trị của cột "Type"
                    object obj = dtgvAccount.SelectedCells[0].OwningRow.Cells["Type"].Value;
                    if (obj == null) return;

                    // Chuyển đổi giá trị sang int
                    int typeValue = Convert.ToInt32(obj);
                    cbAccountType.SelectedIndex = typeValue == 1 ? 0 : 1;
                }

            }
            catch
            {

            }
        }

        #region Food event click
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int iDCategory = (cbFoodIDCategory.SelectedItem as Category).ID;
            float price = (float)nmPriceFood.Value;

            if(FoodDAO.Instance.InsertFood(name, iDCategory, price))
            {
                MessageBox.Show("Thêm món thành công!");
                LoadListFood();
                if (insertFood != null)
                {
                    insertFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm");
            }
        }
        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int iDCategory = (cbFoodIDCategory.SelectedItem as Category).ID;
            float price = (float)nmPriceFood.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(name, iDCategory, price, id))
            {
                MessageBox.Show("Sửa món thành công!");
                LoadListFood();
                if (updateFood != null)
                {
                    updateFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa");
            }
        }
        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);
            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công!");
                LoadListFood();
                if(deleteFood != null)
                {
                    deleteFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa");
            }
        }
        private void btnShow_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }
        #endregion

        // Sự kiện tự thiết lập
        #region Event Handler
        // Food
        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add
            {
                insertFood += value;
            }
            remove
            {
                insertFood -= value;
            }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add
            {
                deleteFood += value;
            }
            remove
            {
                deleteFood -= value;
            }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add
            {
                updateFood += value;
            }
            remove
            {
                updateFood -= value;
            }
        }
        // End food
        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add
            {
                insertCategory += value;
            }
            remove
            {
                insertCategory -= value;
            }
        }

        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add
            {
                deleteCategory += value;
            }
            remove
            {
                deleteCategory -= value;
            }
        }

        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add
            {
                updateCategory += value;
            }
            remove
            {
                updateCategory -= value;
            }
        }

        // Table info
        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add
            {
                insertTable += value;
            }
            remove
            {
                insertTable -= value;
            }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add
            {
                deleteTable += value;
            }
            remove
            {
                deleteTable -= value;
            }
        }

        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add
            {
                updateTable += value;
            }
            remove
            {
                updateTable -= value;
            }
        }
        #endregion

        #region Account event
        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource =  SearchFoodByName(txbSearchFoodName.Text);

        }
        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadListAccount();
        }
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = cbAccountType.SelectedValue.Equals("Admin") ? 1 : 0;

            AddAccount(userName, displayName, type);
        }
        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            DeleteAccount(userName);
        }
        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = cbAccountType.SelectedValue.Equals("Admin") ? 1 : 0;

            UpdateAccount(userName, displayName, type);
        }
        private void btnResetPass_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            ResetPassWord(userName);
        }
        #endregion

        #region Table event
        private void btnShowTable_Click(object sender, EventArgs e)
        {
            LoadListTableFood();
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            string status = txbStatusTable.Text;
            int id = Convert.ToInt32(txbTableID.Text);

            if(TableDAO.Instance.InsertTable(name, status))
            {
                MessageBox.Show("Thêm bàn thành công");
                if (insertTable != null)
                {
                    insertTable(this, EventArgs.Empty);
                }

            }
            else
            {
                MessageBox.Show("Thêm bàn thất bại");
            }
            LoadListTableFood();
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbTableID.Text);

            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Xóa bàn thành công");
                if(deleteTable != null)
                {
                    deleteTable(this, EventArgs.Empty);
                }
            }
            else
            {
                MessageBox.Show("Xóa bàn thất bại");
            }
            LoadListTableFood();
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            string status = txbStatusTable.Text;
            int id = Convert.ToInt32(txbTableID.Text);

            if (TableDAO.Instance.UpdateTable(name, status, id))
            {
                MessageBox.Show("Cập nhật bàn thành công");
                if (updateTable != null)
                {
                    updateTable(this, EventArgs.Empty);
                }
            }
            else
            {
                MessageBox.Show("Cập nhật bàn thất bại");
            }
            LoadListTableFood();

        }
        #endregion

        #region Category event
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryID.Text);
            string name = txbNameCategory.Text.ToString();
            if (CategoryDAO.Instance.InsertCategory(name))
            {
                MessageBox.Show("Thêm danh mục món ăn thành công");
                if (insertCategory != null)
                {
                    insertCategory(this, EventArgs.Empty);  // Kích hoạt sự kiện
                }
            }
            else
            {
                MessageBox.Show("Thêm danh mục món ăn thất bại");
            }
            LoadListCategory();
            LoadListFood();
            LoadCategoryIntoComboBox(cbFoodIDCategory);
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryID.Text);
            string name = txbNameCategory.Text.ToString();

            if(CategoryDAO.Instance.DeleteCategory(id, name))
            {
                MessageBox.Show("Xóa danh mục món ăn thành công");
                if (deleteCategory != null)
                {
                    deleteCategory(this, EventArgs.Empty);  // Kích hoạt sự kiện
                }
            }
            else
            {
                MessageBox.Show("Xóa danh mục món ăn thất bại");
            }
            LoadListCategory();
            LoadListFood();
            LoadCategoryIntoComboBox(cbFoodIDCategory);
        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryID.Text);
            string name = txbNameCategory.Text.ToString();

            if(CategoryDAO.Instance.UpdateCategory(id, name))
            {
                MessageBox.Show("Cập nhật danh mục món ăn thành công");
                if (updateCategory != null)
                {
                    updateCategory(this, EventArgs.Empty);  // Kích hoạt sự kiện
                }
            }
            else{
                MessageBox.Show("Cập nhật danh mục món ăn thành công");
            }
            LoadListCategory();
            LoadListFood();
            LoadCategoryIntoComboBox(cbFoodIDCategory);
        }

        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            LoadListCategory();
        }
        #endregion

        #endregion
    }
}
