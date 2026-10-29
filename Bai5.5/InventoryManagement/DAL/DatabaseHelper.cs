using System.Configuration;
using System.Data.SqlClient;

namespace InventoryManagement.DAL
{
    public static class DatabaseHelper
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["InventoryDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
