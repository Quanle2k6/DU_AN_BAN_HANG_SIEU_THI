CREATE TABLE NHANVIEN (
MaNV char(8) CONSTRAINT NHANVIEN_MaNV_PK PRIMARY KEY,
TenNV varchar(100),
GioiTinh tinyint,
NgSinh smalldatetime,
Email char(100),
CCCD char(12),
MaLNV char(6)
)

CREATE PHONGBAN (
MaPB char(6) CONSTRAINT p