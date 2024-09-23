using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DAO
{
    internal class DataProvider
    {
        private string connectionSTR = @"Data Source=TANH\TANH;Initial Catalog=QuanLyQuanCafe;Integrated Security=True;";

        // Design patern Singleton
        private static DataProvider instance; // Ctrl + R + E
        private DataProvider() { }
        public static DataProvider Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DataProvider();
                }
                return instance;
            }
            private set
            {
                DataProvider.instance = value;
            }
        }
        // End Design patern Singleton

        // Hàm thực hiện excute data
        public DataTable ExecuteQuery(string query, object[] parameters = null)
        {

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(this.connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                // Add N parameters
                if(parameters != null)
                {
                    string[] listParameters = query.Split(' ');
                    int i = 0;
                    foreach (string param in listParameters)
                    {
                        if(param.Contains("@"))
                        {
                            command.Parameters.AddWithValue(param, parameters[i++]);
                        }
                    }
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();

            }
            return dt;
        }
        // Hàm đếm số lượng được insert vào
        public int ExecuteNumQuery(string query, object[] parameters = null)
        {

            int data = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                // Add N parameters
                if (parameters != null)
                {
                    string[] listParameters = query.Split(' ');
                    int i = 0;
                    foreach (string param in listParameters)
                    {
                        if (param.Contains("@"))
                        {
                            command.Parameters.AddWithValue(param, parameters[i++]);
                        }
                    }
                }
                data = command.ExecuteNonQuery();
                connection.Close();

            }
            return data;
        }

        //
        public object ExecuteScalar(string query, object[] parameters = null)
        {

            object data = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                // Add N parameters
                if (parameters != null)
                {
                    string[] listParameters = query.Split(' ');
                    int i = 0;
                    foreach (string param in listParameters)
                    {
                        if (param.Contains("@"))
                        {
                            command.Parameters.AddWithValue(param, parameters[i++]);
                        }
                    }
                }
                data = command.ExecuteScalar();
                connection.Close();

            }
            return data;
        }
    }
}
