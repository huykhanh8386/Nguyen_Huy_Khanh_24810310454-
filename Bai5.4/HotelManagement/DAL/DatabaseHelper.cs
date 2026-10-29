using System.Configuration;
using System.Data.SqlClient;

namespace HotelManagement.DAL
{
    public static class DatabaseHelper
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
