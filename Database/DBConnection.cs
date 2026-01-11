using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Page_Navigation_App.Database
{
    public static class DBConnection
    {
        // CHỈNH SỬA TẠI ĐÂY: Dán chuỗi kết nối của bạn vào đây
        // Đây là cách an toàn nhất cho .NET 6 khi App.config không nhận
        private static readonly string _connectionString = @"Data Source=HP_DEVICE;Initial Catalog=QLST;User ID=User4;Password=123";

        /// <summary>
        /// Thực thi truy vấn SELECT và trả về DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string sql, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ExecuteQuery: " + ex.Message, "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dt;
        }

        /// <summary>
        /// Thực thi INSERT, UPDATE, DELETE
        /// </summary>
        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ExecuteNonQuery: " + ex.Message, "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }
    }
}