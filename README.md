# QUẢN LÝ BÁN HÀNG TẠI SIÊU THỊ (SUPERMARKET MANAGEMENT SYSTEM)

Dự án là một ứng dụng Desktop (Windows) được xây dựng để hỗ trợ tối ưu hóa quy trình quản lý bán hàng, kho bãi và nhân sự cho các siêu thị. Ứng dụng tập trung vào tính trực quan, hiệu quả và hiện đại.

## 📋 Giới thiệu chung
* **Môn học:** Lập trình trực quan (IT008)
* **Giảng viên hướng dẫn:** Cô Nguyễn Thị Xuân Hương
* **Nhóm thực hiện:**
    1. Lê Hoàng Quân - 24521432
    2. Phạm Hoàng Sơn - 24521536
    3. Mai Phú Tân - 24521574
* **Đơn vị:** Khoa Công nghệ Phần mềm - Trường Đại học Công nghệ Thông tin (UIT)

## 💻 Công nghệ và Kiến trúc
Ứng dụng được phát triển dựa trên các nền tảng và thư viện hiện đại:
* **Framework:** .NET Framework / .NET Core.
* **Ngôn ngữ:** C# (C-Sharp).
* **Giao diện:** **WPF (Windows Presentation Foundation)** với thư viện **Material Design in XAML** giúp giao diện chuyên nghiệp.
* **Cơ sở dữ liệu:** **Microsoft SQL Server**.
* **Mô hình thiết kế:** **MVVM (Model - View - ViewModel)** giúp tách biệt rõ ràng giữa logic xử lý và giao diện người dùng.
* **Thư viện hỗ trợ:** * `LiveCharts`: Hiển thị biểu đồ thống kê doanh thu.
    * `MaterialDesignThemes`: Cung cấp các UI components theo phong cách Google.

## ✨ Các tính năng chính
Hệ thống cung cấp đầy đủ các nghiệp vụ quản lý siêu thị bao gồm:
- **Quản lý Đăng nhập & Phân quyền:** Bảo mật hệ thống với các vai trò Admin và Nhân viên.
- **Quản lý Sản phẩm:** Theo dõi danh mục, thông tin hàng hóa, giá bán và giá nhập.
- **Quản lý Kho hàng:** Quản lý nhập hàng, xuất hàng, theo dõi tồn kho và hạn sử dụng sản phẩm.
- **Quản lý Bán hàng:** Tạo hóa đơn nhanh chóng, tính toán tổng tiền, chiết khấu và in hóa đơn cho khách hàng.
- **Quản lý Khách hàng & Đối tác:** Lưu trữ thông tin khách hàng thân thiết và các nhà cung cấp uy tín.
- **Quản lý Khuyến mãi:** Thiết lập và quản lý các chương trình ưu đãi, giảm giá.
- **Báo cáo & Thống kê:** Hệ thống biểu đồ trực quan về doanh thu, lợi nhuận theo ngày/tháng/năm.

## 🗄️ Cấu trúc Cơ sở dữ liệu
Hệ thống sử dụng SQL Server với các bảng thực thể chính:
- `NHANVIEN`, `TAIKHOAN`: Quản lý nhân sự và quyền truy cập.
- `SANPHAM`, `LOAISANPHAM`: Danh mục hàng hóa.
- `HOADON`, `CHITIETHOADON`: Thông tin giao dịch bán hàng.
- `NHAPHANG`, `CHITIETNHAPHANG`, `NHACUNGCAP`: Quy trình nhập hàng từ đối tác.

## 🚀 Hướng phát triển tương lai
- **Tích hợp AI:** Phân tích dữ liệu mua sắm để dự báo xu hướng tiêu dùng.
- **Đa nền tảng:** Phát triển phiên bản Web và Mobile (Android/iOS) để quản lý từ xa.
- **Bảo mật nâng cao:** Tích hợp xác thực 2 lớp (2FA) và mã hóa dữ liệu.
- **Thanh toán điện tử:** Kết nối với các cổng thanh toán (MoMo, VNPay, Ngân hàng).

## 🛠️ Hướng dẫn cài đặt
1. **Yêu cầu:** Đã cài đặt Visual Studio 2022 và SQL Server.
2. **Cài đặt Database:** Chạy file script SQL (đính kèm trong folder Database) để tạo cấu trúc bảng và dữ liệu mẫu.
3. **Cấu hình:** Thay đổi `ConnectionString` trong file `App.config` hoặc lớp kết nối để trỏ đúng về SQL Server cục bộ.
4. **Chạy ứng dụng:** Mở file `.sln` bằng Visual Studio, nhấn `F5` hoặc `Start` để bắt đầu.

---
© 2026 - Nhóm Đồ án IT008 - UIT
