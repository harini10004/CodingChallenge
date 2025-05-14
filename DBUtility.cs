using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement
{
    public class DBUtility
    {
        private const string connectionString = @"Data Source=HARINI;Initial Catalog=OrderManagement;Integrated Security=True;MultipleActiveResultSets=true;";

        // Method to get and open SQL connection
        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Opening the Connection: {e.Message}");
                return null;
            }
        }
    }
}
