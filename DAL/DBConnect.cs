using System.Configuration;
using System.Data.SqlClient;

namespace LibraryManagement.DAL
{
    public class DBConnect
    {
        protected static string connectionString =
            ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}