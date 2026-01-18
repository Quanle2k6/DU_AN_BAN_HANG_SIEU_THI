using Page_Navigation_App.Model;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace Page_Navigation_App.ViewModel
{
    public class SettingVM : Utilities.ViewModelBase
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        public ObservableCollection<NhanVienModel> DanhSachNhanVien { get; set; }

        private NhanVienModel _selectedNhanVien;
        public NhanVienModel SelectedNhanVien
        {
            get => _selectedNhanVien;
            set
            {
                _selectedNhanVien = value;
                OnPropertyChanged();
            }
        }

        public SettingVM()
        {
            DanhSachNhanVien = new ObservableCollection<NhanVienModel>();
            LoadNhanVien();
        }

        public void LoadNhanVien()
        {
            DanhSachNhanVien.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT * FROM NHANVIEN";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    DanhSachNhanVien.Add(new NhanVienModel
                    {
                        MaNV = rd["MaNV"].ToString(),
                        TenNV = rd["TenNV"].ToString(),
                        GioiTinh = rd["GioiTinh"]?.ToString(),
                        NgSinh = rd["NgSinh"] == DBNull.Value ? null : (DateTime?)rd["NgSinh"],
                        NgBDLamViec = rd["NgBDLamViec"] == DBNull.Value ? null : (DateTime?)rd["NgBDLamViec"],
                        Luong = rd["Luong"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["Luong"])
                    });
                }
            }

            OnPropertyChanged(nameof(DanhSachNhanVien));
        }

        public void SaveNhanVien()
        {
            if (SelectedNhanVien == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                UPDATE NHANVIEN
                SET TenNV = @TenNV,
                    GioiTinh = @GioiTinh,
                    NgSinh = @NgSinh,
                    NgBDLamViec = @NgBD,
                    Luong = @Luong
                WHERE MaNV = @MaNV";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNV", SelectedNhanVien.MaNV);
                cmd.Parameters.AddWithValue("@TenNV", SelectedNhanVien.TenNV);
                cmd.Parameters.AddWithValue("@GioiTinh", SelectedNhanVien.GioiTinh ?? "");
                cmd.Parameters.AddWithValue("@NgSinh", (object?)SelectedNhanVien.NgSinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgBD", (object?)SelectedNhanVien.NgBDLamViec ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Luong", SelectedNhanVien.Luong);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Lưu thông tin thành công");
            LoadNhanVien();
        }

        public void ChangePassword(string newPassword)
        {
            if (SelectedNhanVien == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "UPDATE DANGNHAP SET MatKhau = @MK WHERE MaNV = @MaNV";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNV", SelectedNhanVien.MaNV);
                cmd.Parameters.AddWithValue("@MK", newPassword);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    MessageBox.Show("Nhân viên chưa có tài khoản đăng nhập");
                    return;
                }
            }

            MessageBox.Show("Đổi mật khẩu thành công");
        }

        public void Logout()
        {
            MessageBox.Show("Đã đăng xuất");
        }
    }
}
