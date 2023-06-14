using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPManagementSystem.Class
{
    public class DatabaseConnection
    {
        private string connectionString;
        private SqlConnection connection;

        public DatabaseConnection()
        {
            connectionString = Properties.Resources.connectionString;
            connection = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }

        public object ExecuteScalar(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();
                return result;
            }
        }
        public SqlDataReader ExecuteReader(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
        }

        public void ExecuteNonQuery(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void AddParameter(string parameterName, object value)
        {
            connection.Open();
            SqlParameter parameter = new SqlParameter(parameterName, value);
            connection.Close();
        }
    }
}
