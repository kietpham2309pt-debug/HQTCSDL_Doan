USE master;
GO

IF DB_ID('DL_OTO') IS NULL
BEGIN
    CREATE DATABASE DL_OTO;
END
GO

USE DL_OTO;
GO

-- Drop views first
IF OBJECT_ID('vw_DanhSachNhanVien', 'V') IS NOT NULL DROP VIEW vw_DanhSachNhanVien;
IF OBJECT_ID('vw_TonKho', 'V') IS NOT NULL DROP VIEW vw_TonKho;
IF OBJECT_ID('vw_XeBanChay', 'V') IS NOT NULL DROP VIEW vw_XeBanChay;
GO

-- Drop tables in FK order: child tables first
IF OBJECT_ID('ChiTietDH', 'U') IS NOT NULL DROP TABLE ChiTietDH;
IF OBJECT_ID('DonHang', 'U') IS NOT NULL DROP TABLE DonHang;
IF OBJECT_ID('HoaDon', 'U') IS NOT NULL DROP TABLE HoaDon;
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL DROP TABLE TaiKhoan;
IF OBJECT_ID('Xe', 'U') IS NOT NULL DROP TABLE Xe;
IF OBJECT_ID('Car', 'U') IS NOT NULL DROP TABLE Car;
IF OBJECT_ID('DichVuPhuTung', 'U') IS NOT NULL DROP TABLE DichVuPhuTung;
IF OBJECT_ID('NhanVien', 'U') IS NOT NULL DROP TABLE NhanVien;
IF OBJECT_ID('KhachHang', 'U') IS NOT NULL DROP TABLE KhachHang;
IF OBJECT_ID('KHACHHANG', 'U') IS NOT NULL DROP TABLE KHACHHANG;
IF OBJECT_ID('HangXe', 'U') IS NOT NULL DROP TABLE HangXe;
GO

CREATE TABLE HangXe (
    MaHang VARCHAR(20) PRIMARY KEY,
    TenHang NVARCHAR(100) NOT NULL,
    QuocGia NVARCHAR(50),
    LogoPath NVARCHAR(500)
);
GO

CREATE TABLE Xe (
    MaXe VARCHAR(20) PRIMARY KEY,
    TenXe NVARCHAR(100) NOT NULL,
    LoaiXe NVARCHAR(50),
    NamSX INT,
    GiaBan DECIMAL(18,2) CHECK (GiaBan > 0),
    MauSac NVARCHAR(30),
    MoTa NVARCHAR(MAX),
    HinhAnh NVARCHAR(500),
    MaHang VARCHAR(20),
    SoLuongTon INT CHECK (SoLuongTon >= 0),
    CONSTRAINT FK_Xe_HangXe FOREIGN KEY (MaHang) REFERENCES HangXe(MaHang)
);
GO

CREATE TABLE KhachHang (
    MaKH VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SDT VARCHAR(15),
    CCCD VARCHAR(20),
    Email VARCHAR(100),
    DiaChi NVARCHAR(255),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    AnhCaNhan NVARCHAR(500)
);
GO

CREATE TABLE NhanVien (
    MaNV VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50),
    NgayVaoLam DATE DEFAULT GETDATE(),
    SDT VARCHAR(15),
    TrangThai NVARCHAR(50) DEFAULT N'Đang làm việc'
);
GO

CREATE TABLE TaiKhoan (
    Username VARCHAR(50) PRIMARY KEY,
    Password VARCHAR(255) NOT NULL,
    Role NVARCHAR(50),
    MaNV VARCHAR(20),
    CONSTRAINT FK_TaiKhoan_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);
GO

CREATE TABLE DichVuPhuTung (
    MaPT VARCHAR(20) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    Gia DECIMAL(18,2) CHECK (Gia >= 0),
    TonKho INT CHECK (TonKho >= 0)
);
GO

CREATE TABLE HoaDon (
    MaHD VARCHAR(20) PRIMARY KEY,
    NgayLap DATETIME DEFAULT GETDATE(),
    MaNV VARCHAR(20),
    MaKH VARCHAR(20),
    TenDV_SP NVARCHAR(255),
    SoLuong INT CHECK (SoLuong > 0),
    ThanhTien DECIMAL(18,2) CHECK (ThanhTien > 0),
    PhuongThucThanhToan NVARCHAR(50),
    TrangThai NVARCHAR(50) DEFAULT N'Đã xác nhận',
    CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
GO

-- HÃNG XE Ô TÔ
INSERT INTO HangXe VALUES
('HX01', N'Toyota',     N'Nhật Bản', 'https://www.carlogos.org/car-logos/toyota-logo.png'),
('HX02', N'Honda',      N'Nhật Bản', 'https://www.carlogos.org/car-logos/honda-logo.png'),
('HX03', N'Mazda',      N'Nhật Bản', 'https://www.carlogos.org/car-logos/mazda-logo.png'),
('HX04', N'Hyundai',    N'Hàn Quốc', 'https://www.carlogos.org/car-logos/hyundai-logo.png'),
('HX05', N'Kia',        N'Hàn Quốc', 'https://www.carlogos.org/car-logos/kia-logo.png'),
('HX06', N'Ford',       N'Mỹ',       'https://www.carlogos.org/car-logos/ford-logo.png'),
('HX07', N'Mercedes-Benz', N'Đức',   'https://www.carlogos.org/car-logos/mercedes-benz-logo.png'),
('HX08', N'BMW',        N'Đức',      'https://www.carlogos.org/car-logos/bmw-logo.png'),
('HX09', N'VinFast',    N'Việt Nam', 'https://www.carlogos.org/car-logos/vinfast-logo.png'),
('HX10', N'Tesla',      N'Mỹ',       'https://www.carlogos.org/car-logos/tesla-logo.png');
GO

-- XE Ô TÔ
INSERT INTO Xe VALUES
('XE01', N'Toyota Camry 2.5Q 2024',     N'Sedan',     2024, 1495000000, N'Đen',         N'Sedan hạng D động cơ Dynamic Force 2.5L, hộp số 8AT, an toàn Toyota Safety Sense.',     'https://www.toyota.com.vn/uploads/products/camry/Camry%202024/Camry_2024_2.5Q-1.png', 'HX01', 8),
('XE02', N'Toyota Vios E 2024',          N'Sedan',     2024, 488000000,  N'Bạc',         N'Sedan hạng B bán chạy nhất Việt Nam, động cơ 1.5L tiết kiệm xăng.',                       'https://www.toyota.com.vn/uploads/products/vios/Vios%202024/Vios_2024_E_Bac.png',       'HX01', 22),
('XE03', N'Honda Civic RS 2024',         N'Sedan',     2024, 875000000,  N'Đỏ',          N'Sedan thể thao động cơ VTEC TURBO 1.5L, hộp số CVT, Honda SENSING tiêu chuẩn.',           'https://hondaotofan.com/wp-content/uploads/2023/05/Honda-CIVIC-RS-do.png',              'HX02', 11),
('XE04', N'Honda CR-V L 2024',           N'SUV',       2024, 1289000000, N'Trắng Ngọc',  N'SUV 7 chỗ động cơ 1.5L Turbo, AWD, Honda SENSING.',                                       'https://hondaotofan.com/wp-content/uploads/2023/11/CR-V-2024-L-trang-Pearl.png',        'HX02', 9),
('XE05', N'Mazda CX-5 Premium 2024',     N'SUV',       2024, 879000000,  N'Đỏ Pha Lê',   N'SUV 5 chỗ Mazda CX-5 với Skyactiv-G 2.0L, GVC Plus, i-Activsense.',                       'https://www.mazda.com.vn/uploads/cars/cx5/cx5-red.png',                                  'HX03', 14),
('XE06', N'Hyundai Accent AT 2024',      N'Sedan',     2024, 539000000,  N'Xanh Dương',  N'Sedan hạng B Hyundai Accent thế hệ mới, động cơ 1.4L MPI, hộp số AT.',                    'https://www.hyundai.com.vn/wp-content/uploads/2023/12/Accent-2024-Xanh.png',            'HX04', 18),
('XE07', N'Kia Sorento Signature 2024',  N'SUV',       2024, 1359000000, N'Xám Titan',   N'SUV 7 chỗ Kia Sorento bản full option, động cơ Smartstream 2.5L.',                        'https://kia.com.vn/wp-content/uploads/2023/10/Sorento-2024-Xam.png',                    'HX05', 6),
('XE08', N'Ford Everest Titanium 2024',  N'SUV',       2024, 1452000000, N'Đen Nhám',    N'SUV 7 chỗ off-road Ford Everest, Bi-Turbo 2.0L, hộp số 10AT, 4x4.',                       'https://ford.com.vn/wp-content/uploads/2023/06/Everest-Titanium-Den.png',              'HX06', 5),
('XE09', N'Mercedes-Benz C300 AMG 2024', N'Sedan',     2024, 2099000000, N'Bạc Polar',   N'Sedan hạng sang Mercedes C300 AMG, gói AMG Line, MBUX.',                                  'https://www.mercedes-benz.com.vn/img/c-class/c300-amg-bac.png',                          'HX07', 3),
('XE10', N'BMW X5 xDrive40i 2024',       N'SUV',       2024, 4519000000, N'Đen Carbon',  N'SUV hạng sang BMW X5 với máy xăng 3.0L Twin-Power Turbo, xDrive AWD.',                    'https://www.bmw.com.vn/content/dam/bmw/common/all-models/x-series/x5/2023/onepager/bmw-x5-onepager-mg-01.png', 'HX08', 2),
('XE11', N'VinFast VF8 Plus 2024',       N'SUV điện',  2024, 1310000000, N'Xanh Lá',     N'SUV điện 5 chỗ VinFast VF8 Plus, động cơ kép 402hp, pin 87.7kWh, ADAS.',                  'https://vinfastauto.com/sites/default/files/2022-09/VF8-Green.png',                     'HX09', 7),
('XE12', N'Tesla Model 3 Long Range 2024', N'Sedan điện', 2024, 1690000000, N'Trắng Pearl', N'Sedan điện Tesla Model 3 LR, AWD, tăng tốc 0-100 trong 4.4s, Autopilot.',               'https://www.tesla.com/configurator/m3-white.png',                                       'HX10', 4);
GO

-- KHÁCH HÀNG
INSERT INTO KhachHang (MaKH, HoTen, SDT, CCCD, Email, DiaChi) VALUES
('KH001', N'Nguyễn Minh Tuấn',    '0931845672', '079091234571', 'nguyenminhtuan@gmail.com',    N'123 Nguyễn Huệ, Bến Nghé, Quận 1, TP.HCM'),
('KH002', N'Trần Thị Thanh Hà',   '0768234591', '079185234582', 'tranthithanhha@gmail.com',    N'45 Lê Lợi, Bến Thành, Quận 3, TP.HCM'),
('KH003', N'Lê Quốc Cường',       '0853712946', '079094537893', 'lequoccuong@gmail.com',       N'78 Xô Viết Nghệ Tĩnh, P.24, Bình Thạnh, TP.HCM'),
('KH004', N'Phạm Thị Lan Anh',    '0912638457', '079185812604', 'phamthilananh@gmail.com',     N'12 Phan Văn Trị, P.7, Gò Vấp, TP.HCM'),
('KH005', N'Hoàng Trọng Nghĩa',   '0375824193', '079095634215', 'hoangtrongnghia@gmail.com',   N'56 Võ Văn Ngân, Linh Chiểu, Thủ Đức, TP.HCM'),
('KH006', N'Vũ Thị Mỹ Hạnh',      '0896314725', '079185417326', 'vuthimyhanh@gmail.com',       N'34 Bình Giã, P.13, Quận 10, TP.HCM'),
('KH007', N'Đặng Văn Hùng',       '0582947361', '079093248537', 'dangvanhung@gmail.com',       N'89 Trường Chinh, P.14, Tân Bình, TP.HCM'),
('KH008', N'Bùi Ngọc Hương',      '0703581924', '079180539148', 'buingochuong@gmail.com',      N'21 Nguyễn Thị Thập, Tân Phú, Quận 7, TP.HCM'),
('KH009', N'Đỗ Thanh Khoa',       '0846275319', '079095624759', 'dothankhoa@gmail.com',        N'67 Nguyễn Tất Thành, P.13, Quận 4, TP.HCM'),
('KH010', N'Ngô Thị Phương Linh', '0563748291', '079181234860', 'ngothiphuonglinh@gmail.com',  N'15 Kinh Dương Vương, An Lạc, Bình Tân, TP.HCM');
GO

-- NHÂN VIÊN
INSERT INTO NhanVien VALUES
('NV001', N'Huỳnh Thanh Phong',     N'Quản lý',              '2019-03-01', '0938145762', N'Đang làm việc'),
('NV002', N'Lê Minh Khang',         N'Nhân viên bán hàng',   '2021-06-15', '0764823915', N'Đang làm việc'),
('NV003', N'Trần Thị Ngọc Lan',     N'Nhân viên bán hàng',   '2022-08-10', '0852917364', N'Đang làm việc'),
('NV004', N'Nguyễn Hữu Tài',        N'Kỹ thuật viên',        '2020-11-20', '0915284736', N'Đang làm việc'),
('NV005', N'Phan Đình Phúc',        N'Kỹ thuật viên',        '2021-04-05', '0376924158', N'Đang làm việc'),
('NV006', N'Cao Thị Mỹ Linh',       N'Kế toán',              '2020-01-10', '0895471362', N'Đang làm việc'),
('NV007', N'Võ Trung Kiên',         N'Bảo vệ',               '2022-02-28', '0584619273', N'Đang làm việc'),
('NV008', N'Đinh Tuấn Anh',         N'Kỹ thuật viên',        '2023-07-01', '0702381945', N'Đang làm việc'),
('NV009', N'Lương Thị Bích Huyền',  N'Chăm sóc khách hàng', '2023-11-15', '0849627153', N'Đang làm việc'),
('NV010', N'Phan Văn Triều',        N'Nhân viên bán hàng',   '2020-09-12', '0561384972', N'Tạm nghỉ');
GO

-- TÀI KHOẢN
INSERT INTO TaiKhoan VALUES
('admin',      '123456', N'Quản lý',              'NV001'),
('nv_khang',   '123456', N'Bán hàng',             'NV002'),
('nv_lan',     '123456', N'Bán hàng',             'NV003'),
('kt_tai',     '123456', N'Kỹ thuật',             'NV004'),
('kt_phuc',    '123456', N'Kỹ thuật',             'NV005'),
('kt_linh',    '123456', N'Kế toán',              'NV006'),
('bv_kien',    '123456', N'Bảo vệ',               'NV007'),
('kt_anh',     '123456', N'Kỹ thuật',             'NV008'),
('cskh_huyen', '123456', N'Chăm sóc khách hàng', 'NV009'),
('nv_trieu',   '123456', N'Bán hàng',             'NV010');
GO

-- DỊCH VỤ & PHỤ TÙNG Ô TÔ
INSERT INTO DichVuPhuTung VALUES
('DV001', N'Bảo dưỡng định kỳ 10.000km',      1500000,   0),
('DV002', N'Rửa xe + dọn nội thất cao cấp',    250000,   0),
('DV003', N'Thay nhớt động cơ + lọc nhớt',     650000,   0),
('DV004', N'Đại tu động cơ',                 12000000,   0),
('DV005', N'Cân chỉnh thước lái + cân mâm',    450000,   0),
('DV006', N'Kiểm tra hệ thống điện ECU',       800000,   0),
('PT001', N'Nhớt Mobil 1 ESP 5W-30 4L',        850000,  40),
('PT002', N'Lọc gió động cơ K&N',             1200000,  25),
('PT003', N'Bugi NGK Iridium IX',              280000, 100),
('PT004', N'Lốp Michelin Pilot Sport 4 225/45R17', 5800000, 20),
('PT005', N'Ắc quy Varta Silver Dynamic 70Ah', 2950000,  15),
('PT006', N'Phanh đĩa Brembo trước',          3500000,  18),
('PT007', N'Camera hành trình Vietmap C61',   2400000,  35),
('PT008', N'Phim cách nhiệt 3M Crystalline',  9500000,   8);
GO

-- HÓA ĐƠN
INSERT INTO HoaDon (MaHD, NgayLap, MaNV, MaKH, TenDV_SP, SoLuong, ThanhTien, PhuongThucThanhToan) VALUES
('HD001', '2025-01-08 09:15:00', 'NV002', 'KH001', N'Toyota Camry 2.5Q 2024',           1, 1495000000, N'Trả góp'),
('HD002', '2025-01-15 10:30:00', 'NV003', 'KH002', N'Toyota Vios E 2024',                1, 488000000,  N'Tiền mặt'),
('HD003', '2025-02-03 08:45:00', 'NV004', 'KH003', N'Bảo dưỡng định kỳ 10.000km',       1, 1500000,    N'Tiền mặt'),
('HD004', '2025-02-18 14:00:00', 'NV005', 'KH004', N'Lốp Michelin Pilot Sport 4 225/45R17', 4, 23200000, N'Chuyển khoản'),
('HD005', '2025-03-05 11:00:00', 'NV002', 'KH005', N'Honda Civic RS 2024',               1, 875000000,  N'Trả góp'),
('HD006', '2025-03-20 13:30:00', 'NV003', 'KH006', N'Mazda CX-5 Premium 2024',           1, 879000000,  N'Chuyển khoản'),
('HD007', '2025-04-02 09:00:00', 'NV004', 'KH007', N'Rửa xe + dọn nội thất cao cấp',    1, 250000,     N'Tiền mặt'),
('HD008', '2025-04-10 15:00:00', 'NV002', 'KH008', N'Hyundai Accent AT 2024',            1, 539000000,  N'Trả góp'),
('HD009', '2025-04-25 10:00:00', 'NV003', 'KH009', N'BMW X5 xDrive40i 2024',             1, 4519000000, N'Trả góp'),
('HD010', '2025-05-07 08:30:00', 'NV002', 'KH010', N'Camera hành trình Vietmap C61',    1, 2400000,    N'Tiền mặt');
GO

PRINT N'DL_OTO đã được tạo lại theo schema mới.';
GO
