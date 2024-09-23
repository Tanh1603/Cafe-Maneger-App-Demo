using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WindowsFormsApp1.DTO;

namespace WindowsFormsApp1.DAO
{
    internal class FoodDAO
    {
        private static FoodDAO instance;

        private FoodDAO() { }
        public static FoodDAO Instance {
            get 
            {
                if(instance == null) instance = new FoodDAO();
                return instance;
            }
            private set => instance = value; 
        }

        public List<Food> GetFoodCategoryById(int id)
        {
            List<Food> listFood = new List<Food>();
            string query = $"SELECT * FROM dbo.Food WHERE IDCategory = {id}";
            DataTable dataTable = DataProvider.Instance.ExecuteQuery(query);
            foreach(DataRow row in dataTable.Rows )
            {
                listFood.Add(new Food(row));
            }
            return listFood;
        }

        public List<Food> GetListFood()
        {
            List<Food> listFood = new List<Food>();
            string query = $"SELECT ID, IDCategory, Name, Price FROM dbo.Food ";
            DataTable dataTable = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow row in dataTable.Rows)
            {
                listFood.Add(new Food(row));
            }
            return listFood;
        }

        public List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = new List<Food>();

            string query = $"SELECT * FROM dbo.Food WHERE dbo.utf8ConvertSQL(Name) LIKE '%' + dbo.utf8ConvertSQL(N'{name}') + '%'";
            DataTable dataTable = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow row in dataTable.Rows)
            {
                listFood.Add(new Food(row));
            }
            return listFood;
        }
        public void DeleteFoodByIdCategory(int id)
        {
            string query = $"DELETE FROM dbo.Food WHERE IDCategory = {id}";
            DataProvider.Instance.ExecuteQuery(query);
        }
        // Thêm sửa xóa
        public bool InsertFood(string name, int iDCategory, float price)
        {
            string query = $"INSERT INTO dbo.Food (Name, IDCategory, Price) VALUES (N'{name}', {iDCategory}, {price})";
            int res = DataProvider.Instance.ExecuteNumQuery(query);

            return res > 0;
        }
        public bool UpdateFood(string name, int iDCategory, float price, int id)
        {
            string query = $"UPDATE dbo.Food SET Name = N'{name}' , IDCategory = {iDCategory}, Price = {price} WHERE ID = {id}";
            int res = DataProvider.Instance.ExecuteNumQuery(query);

            return res > 0;
        }
        public bool DeleteFood(int id)
        {
            BillInfoDAO.Instance.DeleteBillInfoByIdFood(id);
            string query = $"DELETE dbo.Food WHERE ID = {id}";
            int res = DataProvider.Instance.ExecuteNumQuery(query);

            return res > 0;
        } 

    }
}
