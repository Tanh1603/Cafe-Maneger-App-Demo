using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1.DAO
{
    internal class CategoryDAO
    {
        private static CategoryDAO instance;
        private CategoryDAO() { }
        public static CategoryDAO Instance
        {
            get { if (instance == null) instance = new CategoryDAO(); return instance; }
            private set { instance = value; }
        }

        // Hàm
        public List<Category> GetListCategories()
        {
            List<Category> listCategories = new List<Category>();
            string query = "SELECT * FROM dbo.FoodCategory";
            DataTable dataTable = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow row in dataTable.Rows)
            {
                listCategories.Add(new Category(row));
            }
            return listCategories;
        }

        public Category GetCategoryById(int id)
        {
            string query = $"SELECT * FROM dbo.FoodCategory WHERE ID = {id}";
            DataTable data  = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                return new Category(row);
            }
            return null;
        }
        public bool isHasCategoryName(string name)
        {
            string query = $"SELECT * FROM dbo.FoodCategory WHERE Name =N'{name}'";
            object res = DataProvider.Instance.ExecuteScalar(query);
            return res != null;


        }
        // fn change db category

        public bool InsertCategory(string name)
        {
            if(this.isHasCategoryName(name)) return false;
            string query = $"INSERT INTO dbo.FoodCategory(Name) VALUES (N'{name}')";
            int res = DataProvider.Instance.ExecuteNumQuery(query);
            return res > 0;
        }

        public bool UpdateCategory(int id, string name)
        {
            if (this.isHasCategoryName(name)) return false;
            string query = $"UPDATE dbo.FoodCategory SET Name =  N'{name}' WHERE ID = {id}";
            int res = DataProvider.Instance.ExecuteNumQuery(query);
            return res > 0;
        }

        public bool DeleteCategory(int id, string name)
        {
            FoodDAO.Instance.DeleteFoodByIdCategory(id);
            string query = $"DELETE FROM dbo.FoodCategory WHERE Name = N'{name}' AND ID = {id}";
            int res = DataProvider.Instance.ExecuteNumQuery(query);
            return res > 0;
        }
    }
}
