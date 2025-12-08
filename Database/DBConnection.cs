using System.Data.SqlClient;
using System.Configuration; // Used to read App.config
using System;
using System.Data;

namespace Page_Navigation_App.Database
{
    class DBConnection
    {
        // 1. Get connection string from App.config
        private static string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        // 2. Execute non-query commands (INSERT, UPDATE, DELETE)
        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. Execute query and return DataTable (SELECT)
        public static DataTable ExecuteQuery(string sql, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}
