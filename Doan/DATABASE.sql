/* ============================================================
   DL_OTO  -  FILE SQL DUY NHẤT
   Chạy 1 lần trong SSMS là xong:
     1. Tạo (hoặc reset) database DL_OTO
     2. Tạo bảng theo schema chuẩn
     3. Nạp dữ liệu mẫu (HangXe, Xe, KhachHang, NhanVien,
        TaiKhoan, DichVuPhuTung, HoaDon)
   ============================================================ */

USE master;
GO

IF DB_ID('DL_OTO') IS NULL
BEGIN
    CREATE DATABASE DL_OTO;
END
GO

USE DL_OTO;
GO

-- Xoá view, bảng cũ (nếu chạy lại)
IF OBJECT_ID('vw_DanhSachNhanVien', 'V') IS NOT NULL DROP VIEW vw_DanhSachNhanVien;
IF OBJECT_ID('vw_TonKho', 'V') IS NOT NULL DROP VIEW vw_TonKho;
IF OBJECT_ID('vw_XeBanChay', 'V') IS NOT NULL DROP VIEW vw_XeBanChay;
GO

IF OBJECT_ID('ChiTietDH', 'U') IS NOT NULL DROP TABLE ChiTietDH;
IF OBJECT_ID('DonHang', 'U') IS NOT NULL DROP TABLE DonHang;
IF OBJECT_ID('LichSuGiaXe', 'U') IS NOT NULL DROP TABLE LichSuGiaXe;
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

/* ============================================================
   1. TẠO BẢNG
   ============================================================ */

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

/* ============================================================
   2. NẠP DỮ LIỆU
   ============================================================ */

-- 2.1 HÃNG XE
INSERT INTO HangXe VALUES
('HX01', N'Toyota',        N'Nhật Bản', 'https://www.carlogos.org/car-logos/toyota-logo.png'),
('HX02', N'Honda',         N'Nhật Bản', 'https://www.carlogos.org/car-logos/honda-logo.png'),
('HX03', N'Mazda',         N'Nhật Bản', 'https://www.carlogos.org/car-logos/mazda-logo.png'),
('HX04', N'Hyundai',       N'Hàn Quốc', 'https://www.carlogos.org/car-logos/hyundai-logo.png'),
('HX05', N'Kia',           N'Hàn Quốc', 'https://www.carlogos.org/car-logos/kia-logo.png'),
('HX06', N'Ford',          N'Mỹ',       'https://www.carlogos.org/car-logos/ford-logo.png'),
('HX07', N'Mercedes-Benz', N'Đức',      'https://www.carlogos.org/car-logos/mercedes-benz-logo.png'),
('HX08', N'BMW',           N'Đức',      'https://www.carlogos.org/car-logos/bmw-logo.png'),
('HX09', N'VinFast',       N'Việt Nam', 'https://www.carlogos.org/car-logos/vinfast-logo.png'),
('HX10', N'Tesla',         N'Mỹ',       'https://www.carlogos.org/car-logos/tesla-logo.png');
GO

-- 2.2 XE Ô TÔ (71 mẫu, đã merge đường dẫn ảnh từ SEED_XE + FALLBACK_IMAGES + FALLBACK2)
INSERT INTO Xe (MaXe, TenXe, LoaiXe, NamSX, GiaBan, MauSac, MoTa, HinhAnh, MaHang, SoLuongTon) VALUES
-- Toyota
('XE01', N'Toyota Camry 2.5Q 2024',           N'Sedan',     2024, 1495000000, N'Đen',           N'Sedan hạng D động cơ Dynamic Force 2.5L, hộp số 8AT, an toàn Toyota Safety Sense.',     N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE01.jpg', 'HX01', 8),
('XE02', N'Toyota Vios E 2024',                N'Sedan',     2024, 488000000,  N'Bạc',           N'Sedan hạng B bán chạy nhất Việt Nam, động cơ 1.5L tiết kiệm xăng.',                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE02.jpg', 'HX01', 22),
('XE13', N'Toyota Corolla Cross V 2024',       N'SUV',       2024, 905000000,  N'Trắng Ngọc',    N'Crossover gầm cao Toyota Corolla Cross, động cơ 1.8L, Toyota Safety Sense 3.0.',         N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE13.jpg', 'HX01', 12),
('XE14', N'Toyota Innova Cross HEV 2024',      N'MPV',       2024, 990000000,  N'Bạc Ánh Kim',   N'MPV 7 chỗ Toyota Innova Cross bản Hybrid, động cơ 2.0L HEV, TSS 3.0.',                   N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE14.jpg', 'HX01', 10),
('XE15', N'Toyota Fortuner Legender 2024',     N'SUV',       2024, 1295000000, N'Đen Kim Cương', N'SUV 7 chỗ Toyota Fortuner Legender, máy dầu 2.8L, hộp số 6AT, 4x2.',                     N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE15.jpg', 'HX01', 8),
('XE16', N'Toyota Hilux Adventure 2024',       N'Bán tải',   2024, 999000000,  N'Cam Inferno',   N'Bán tải Toyota Hilux Adventure, máy dầu 2.8L, 4WD, hộp số 6AT.',                         N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE16.jpg', 'HX01', 6),
('XE17', N'Toyota Veloz Cross Top 2024',       N'MPV',       2024, 678000000,  N'Xanh Đậm',      N'MPV 7 chỗ Toyota Veloz Cross Top, máy 1.5L CVT, an toàn TSS.',                           N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE17.jpg', 'HX01', 11),
-- Honda
('XE03', N'Honda Civic RS 2024',               N'Sedan',     2024, 875000000,  N'Đỏ',            N'Sedan thể thao động cơ VTEC TURBO 1.5L, hộp số CVT, Honda SENSING tiêu chuẩn.',          N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE03.jpg', 'HX02', 11),
('XE04', N'Honda CR-V L 2024',                 N'SUV',       2024, 1289000000, N'Trắng Ngọc',    N'SUV 7 chỗ động cơ 1.5L Turbo, AWD, Honda SENSING.',                                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE04.jpg', 'HX02', 9),
('XE19', N'Honda City RS 2024',                N'Sedan',     2024, 599000000,  N'Đỏ Carnelian',  N'Sedan hạng B Honda City RS, máy 1.5L i-VTEC, hộp số CVT.',                               N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE19.jpg', 'HX02', 18),
('XE20', N'Honda Accord 1.5 TURBO 2024',       N'Sedan',     2024, 1319000000, N'Xám Bạc',       N'Sedan hạng D Honda Accord, máy VTEC TURBO 1.5L, Honda SENSING.',                         N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE20.jpg', 'HX02', 5),
('XE21', N'Honda BR-V L 2024',                 N'MPV',       2024, 705000000,  N'Xám Meteoroid', N'MPV 7 chỗ Honda BR-V L, máy 1.5L i-VTEC, Honda SENSING.',                                N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE21.jpg', 'HX02', 13),
('XE22', N'Honda HR-V L 2024',                 N'SUV',       2024, 871000000,  N'Bạc Lunar',     N'SUV đô thị Honda HR-V L, máy VTEC TURBO 1.5L, Honda SENSING.',                           N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE22.jpg', 'HX02', 9),
-- Mazda
('XE05', N'Mazda CX-5 Premium 2024',           N'SUV',       2024, 879000000,  N'Đỏ Pha Lê',     N'SUV 5 chỗ Mazda CX-5 với Skyactiv-G 2.0L, GVC Plus, i-Activsense.',                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE05.jpg', 'HX03', 14),
('XE23', N'Mazda CX-3 Premium 2024',           N'SUV',       2024, 624000000,  N'Xanh Đa Sắc',   N'SUV cỡ nhỏ Mazda CX-3, Skyactiv-G 1.5L, hộp số 6AT.',                                    N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE23.jpg', 'HX03', 10),
('XE24', N'Mazda CX-30 Premium 2024',          N'SUV',       2024, 729000000,  N'Xanh Pha Lê',   N'Crossover Mazda CX-30, Skyactiv-G 2.0L, i-Activsense.',                                  N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE24.jpg', 'HX03', 8),
('XE25', N'Mazda CX-8 Premium 2024',           N'SUV',       2024, 1099000000, N'Xám Machine',   N'SUV 7 chỗ Mazda CX-8 Premium, Skyactiv-G 2.5L, AWD.',                                    N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE25.jpg', 'HX03', 7),
('XE26', N'Mazda3 1.5L Luxury 2024',           N'Sedan',     2024, 619000000,  N'Đỏ Soul',       N'Sedan Mazda3 1.5L Luxury, hộp số 6AT, i-Activsense.',                                    N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE26.jpg', 'HX03', 12),
('XE27', N'Mazda6 2.0L Premium 2024',          N'Sedan',     2024, 854000000,  N'Xanh Eternal Blue', N'Sedan hạng D Mazda6 Premium, Skyactiv-G 2.0L.',                                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE27.jpg', 'HX03', 6),
('XE28', N'Mazda CX-9 Signature 2024',         N'SUV',       2024, 2199000000, N'Đen Jet Black', N'SUV 7 chỗ flagship Mazda CX-9, Skyactiv-G 2.5L Turbo, AWD.',                             N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE28.jpg', 'HX03', 3),
-- Hyundai
('XE06', N'Hyundai Accent AT 2024',            N'Sedan',     2024, 539000000,  N'Xanh Dương',    N'Sedan hạng B Hyundai Accent, động cơ 1.4L MPI, hộp số AT.',                              N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE06.jpg', 'HX04', 18),
('XE29', N'Hyundai Tucson 2.0 AT 2024',        N'SUV',       2024, 859000000,  N'Trắng',         N'SUV Hyundai Tucson Smartstream 2.0L, Hyundai SmartSense.',                               N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE29.jpg', 'HX04', 11),
('XE30', N'Hyundai Santa Fe Calligraphy 2024', N'SUV',       2024, 1365000000, N'Trắng Creamy',  N'SUV 7 chỗ Santa Fe Calligraphy, Smartstream 2.5L, HTRAC AWD.',                           N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE30.jpg', 'HX04', 7),
('XE31', N'Hyundai Elantra Sport 2024',        N'Sedan',     2024, 769000000,  N'Đỏ Fiery',      N'Sedan hạng C Elantra Sport 1.6 Turbo, hộp số 7DCT.',                                     N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE31.jpg', 'HX04', 10),
('XE32', N'Hyundai Creta Cao Cấp 2024',        N'SUV',       2024, 729000000,  N'Xanh Ocean',    N'SUV đô thị Hyundai Creta, Smartstream 1.5L, IVT.',                                       N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE32.jpg', 'HX04', 14),
('XE33', N'Hyundai Stargazer X 2024',          N'MPV',       2024, 575000000,  N'Vàng Bumblebee',N'MPV 7 chỗ Hyundai Stargazer X, Smartstream 1.5L, IVT.',                                  N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE33.jpg', 'HX04', 9),
('XE34', N'Hyundai Kona Electric 2024',        N'SUV điện',  2024, 750000000,  N'Xanh Surf Blue',N'SUV điện Hyundai Kona Electric 64kWh, AWD.',                                             N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE34.jpg', 'HX04', 5),
('XE35', N'Hyundai Palisade Calligraphy 2024', N'SUV',       2024, 1689000000, N'Đen Onyx',      N'SUV 8 chỗ Hyundai Palisade Calligraphy, V6 3.8L, HTRAC AWD.',                            N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE35.jpg', 'HX04', 3),
-- Kia
('XE07', N'Kia Sorento Signature 2024',        N'SUV',       2024, 1359000000, N'Xám Titan',     N'SUV 7 chỗ Kia Sorento bản full option, động cơ Smartstream 2.5L.',                       N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE07.jpg', 'HX05', 6),
('XE36', N'Kia Seltos X-Line 2024',            N'SUV',       2024, 729000000,  N'Xanh Pluton',   N'SUV đô thị Kia Seltos X-Line, Smartstream 1.6 Turbo.',                                   N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE36.jpg', 'HX05', 12),
('XE37', N'Kia Sportage Signature 2024',       N'SUV',       2024, 1019000000, N'Trắng Pure',    N'SUV Kia Sportage Signature, Smartstream 1.6T HEV, AWD.',                                 N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE37.jpg', 'HX05', 8),
('XE38', N'Kia Carnival Signature 2024',       N'MPV',       2024, 1839000000, N'Vàng Honey Bee',N'MPV 7-11 chỗ cao cấp Kia Carnival Signature, diesel 2.2L.',                              N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE38.jpg', 'HX05', 4),
('XE39', N'Kia K3 Premium 2024',               N'Sedan',     2024, 689000000,  N'Đỏ Runway',     N'Sedan hạng C Kia K3 Premium, Smartstream 1.6L IVT.',                                     N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE39.jpg', 'HX05', 13),
('XE40', N'Kia Sonet Premium 2024',            N'SUV',       2024, 624000000,  N'Cam Citrus',    N'SUV cỡ nhỏ Kia Sonet Premium, máy 1.5L Smartstream, IVT.',                               N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE40.jpg', 'HX05', 11),
('XE41', N'Kia Morning Premium 2024',          N'Hatchback', 2024, 439000000,  N'Xanh Mint',     N'Hatchback đô thị Kia Morning Premium, máy 1.2L Kappa.',                                  N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE41.jpg', 'HX05', 20),
('XE42', N'Kia EV6 GT-Line 2024',              N'SUV điện',  2024, 1559000000, N'Xanh Yacht',    N'Crossover điện Kia EV6 GT-Line, pin 77.4kWh, AWD.',                                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE42.jpg', 'HX05', 3),
-- Ford
('XE08', N'Ford Everest Titanium 2024',        N'SUV',       2024, 1452000000, N'Đen Nhám',      N'SUV 7 chỗ off-road Ford Everest, Bi-Turbo 2.0L, hộp số 10AT, 4x4.',                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE08.jpg', 'HX06', 5),
('XE43', N'Ford Ranger Wildtrak 2024',         N'Bán tải',   2024, 965000000,  N'Xanh Pháo Đài', N'Bán tải Ford Ranger Wildtrak Bi-Turbo 2.0L, 4x4, hộp số 10AT.',                          N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE43.jpg', 'HX06', 9),
('XE44', N'Ford Explorer Limited 2024',        N'SUV',       2024, 2399000000, N'Đỏ Rapid Red',  N'SUV 7 chỗ Ford Explorer Limited, EcoBoost 2.3L, hộp số 10AT, AWD.',                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE44.jpg', 'HX06', 2),
('XE45', N'Ford Territory Titanium X 2024',    N'SUV',       2024, 879000000,  N'Xanh Hồ Tiêu',  N'SUV Ford Territory Titanium X, máy EcoBoost 1.5L Turbo.',                                N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE45.jpg', 'HX06', 8),
('XE46', N'Ford Mustang GT 5.0 V8 2024',       N'Coupe',     2024, 4250000000, N'Vàng Grabber',  N'Muscle car Ford Mustang GT V8 5.0L, 480 mã lực.',                                        N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE46.jpg', 'HX06', 1),
('XE47', N'Ford Transit Limited 2024',         N'Van',       2024, 920000000,  N'Bạc Moondust',  N'Van 16 chỗ Ford Transit Limited, máy dầu 2.2L Duratorq.',                                N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE47.jpg', 'HX06', 4),
-- Mercedes-Benz
('XE09', N'Mercedes-Benz C300 AMG 2024',       N'Sedan',     2024, 2099000000, N'Bạc Polar',     N'Sedan hạng sang Mercedes C300 AMG, gói AMG Line, MBUX.',                                 N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE09.jpg', 'HX07', 3),
('XE49', N'Mercedes-Benz E300 AMG 2024',       N'Sedan',     2024, 3179000000, N'Đen Obsidian',  N'Sedan E300 AMG Line 2024, máy 2.0L turbo + EQ Boost.',                                   N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE49.jpg', 'HX07', 2),
('XE50', N'Mercedes-Benz S450 4MATIC 2024',    N'Sedan',     2024, 5759000000, N'Đen Onyx',      N'Sedan flagship Mercedes S450 4MATIC, máy 3.0L L6 turbo, EQ Boost.',                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE50.jpg', 'HX07', 1),
('XE51', N'Mercedes-Benz GLC 300 4MATIC 2024', N'SUV',       2024, 2629000000, N'Trắng Polar',   N'SUV hạng sang Mercedes GLC 300 4MATIC, máy 2.0L Turbo + EQ Boost.',                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE51.jpg', 'HX07', 4),
('XE52', N'Mercedes-Benz GLE 450 4MATIC 2024', N'SUV',       2024, 4659000000, N'Xám Selenite',  N'SUV cỡ lớn Mercedes GLE 450 4MATIC, L6 3.0L, AIRMATIC.',                                 N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE52.jpg', 'HX07', 2),
('XE53', N'Mercedes-Benz A200 Progressive 2024', N'Hatchback', 2024, 1689000000, N'Trắng Digital', N'Hatchback hạng sang Mercedes A200, máy 1.3L Turbo.',                                    N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE53.jpg', 'HX07', 5),
('XE54', N'Mercedes-Benz GLA 200 2024',        N'SUV',       2024, 1789000000, N'Đỏ Patagonia',  N'SUV đô thị Mercedes GLA 200, máy 1.3L Turbo, MBUX.',                                     N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE54.jpg', 'HX07', 4),
-- BMW
('XE10', N'BMW X5 xDrive40i 2024',             N'SUV',       2024, 4519000000, N'Đen Carbon',    N'SUV hạng sang BMW X5 với máy xăng 3.0L Twin-Power Turbo, xDrive AWD.',                   N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE10.jpg', 'HX08', 2),
('XE55', N'BMW X3 xDrive30i 2024',             N'SUV',       2024, 2629000000, N'Xanh Phytonic', N'SUV BMW X3 xDrive30i, máy 2.0L Turbo, AWD.',                                             N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE55.jpg', 'HX08', 3),
('XE56', N'BMW X7 xDrive40i 2024',             N'SUV',       2024, 6499000000, N'Đen Carbon',    N'SUV 7 chỗ flagship BMW X7, máy 3.0L Twin-Power Turbo, xDrive AWD.',                      N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE56.jpg', 'HX08', 1),
('XE57', N'BMW 320i Sport Line 2024',          N'Sedan',     2024, 1599000000, N'Đỏ Vermillion', N'Sedan BMW 320i Sport Line, máy 2.0L Turbo, dẫn động cầu sau.',                           N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE57.jpg', 'HX08', 5),
('XE58', N'BMW 530i M Sport 2024',             N'Sedan',     2024, 2499000000, N'Xám Brooklyn',  N'Sedan BMW 530i M Sport, máy 2.0L Turbo, 48V mild hybrid.',                               N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE58.jpg', 'HX08', 3),
('XE59', N'BMW 740i Pure Excellence 2024',     N'Sedan',     2024, 5859000000, N'Đen Carbon',    N'Sedan flagship BMW 740i, máy L6 3.0L Turbo, 48V mild hybrid.',                           N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE59.jpg', 'HX08', 1),
('XE60', N'BMW i4 eDrive40 2024',              N'Sedan điện',2024, 3759000000, N'Xanh Portimao', N'Gran Coupe điện BMW i4 eDrive40, pin 83.9kWh, RWD.',                                     N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE60.jpg', 'HX08', 2),
-- VinFast
('XE11', N'VinFast VF8 Plus 2024',             N'SUV điện',  2024, 1310000000, N'Xanh Lá',       N'SUV điện 5 chỗ VinFast VF8 Plus, động cơ kép 402hp, pin 87.7kWh, ADAS.',                 N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE11.jpg', 'HX09', 7),
('XE61', N'VinFast VF9 Plus 2024',             N'SUV điện',  2024, 1499000000, N'Đen Obsidian',  N'SUV điện 7 chỗ VinFast VF9 Plus, AWD, pin 92kWh.',                                       N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE61.jpg', 'HX09', 4),
('XE62', N'VinFast VF6 Plus 2024',             N'SUV điện',  2024, 765000000,  N'Đỏ Cherry',     N'SUV B điện VinFast VF6 Plus, pin LFP 59.6kWh, ADAS.',                                    N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE62.jpg', 'HX09', 8),
('XE63', N'VinFast VF7 Plus 2024',             N'SUV điện',  2024, 949000000,  N'Xám Bão',       N'SUV C điện VinFast VF7 Plus, AWD, pin 75.3kWh.',                                         N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE63.jpg', 'HX09', 6),
('XE64', N'VinFast VF3 2024',                  N'SUV điện',  2024, 322000000,  N'Vàng Hoa Cúc',  N'Mini SUV điện VinFast VF3, pin 18.6kWh, đi 210km/sạc.',                                  N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE64.jpg', 'HX09', 25),
('XE65', N'VinFast VF5 Plus 2024',             N'SUV điện',  2024, 530000000,  N'Xanh Da Trời',  N'SUV A điện VinFast VF5 Plus, pin 37.23kWh, FWD.',                                        N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE65.jpg', 'HX09', 15),
('XE66', N'VinFast VF e34 2024',               N'SUV điện',  2024, 690000000,  N'Xám Titanium',  N'SUV điện đầu tiên VinFast VF e34, pin 42kWh.',                                           N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE66.jpg', 'HX09', 10),
-- Tesla
('XE12', N'Tesla Model 3 Long Range 2024',     N'Sedan điện',2024, 1690000000, N'Trắng Pearl',   N'Sedan điện Tesla Model 3 LR, AWD, tăng tốc 0-100 trong 4.4s, Autopilot.',                N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE12.jpg', 'HX10', 4),
('XE67', N'Tesla Model S Plaid 2024',          N'Sedan điện',2024, 4099000000, N'Đen Solid',     N'Sedan flagship Tesla Model S Plaid, tri-motor, 1020hp, 0-100 trong 2.1s.',               N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE67.jpg', 'HX10', 1),
('XE68', N'Tesla Model X Plaid 2024',          N'SUV điện',  2024, 4599000000, N'Trắng Pearl',   N'SUV 7 chỗ Tesla Model X Plaid, cửa cánh chim, tri-motor 1020hp.',                        N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE68.jpg', 'HX10', 1),
('XE69', N'Tesla Model Y Long Range 2024',     N'SUV điện',  2024, 1990000000, N'Xanh Deep Blue',N'Crossover điện Tesla Model Y LR, AWD, đi 525km/sạc.',                                    N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE69.jpg', 'HX10', 6),
('XE70', N'Tesla Cybertruck Cyberbeast 2024',  N'Bán tải điện', 2024, 2599000000, N'Bạc Inox',   N'Bán tải điện tương lai Tesla Cybertruck Cyberbeast, AWD, 0-100 trong 2.6s.',             N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE70.jpg', 'HX10', 2),
('XE71', N'Tesla Roadster 2024',               N'Coupe điện',2024, 5999000000, N'Đỏ Signature',  N'Hypercar điện Tesla Roadster, 0-100 trong 1.9s, đi 1000km/sạc.',                         N'D:\visua studio 2022\DATA\hqtcsdl\HQTCSDL_Doan\Doan\Doan\images\cars\XE71.jpg', 'HX10', 1);
GO

-- 2.3 KHÁCH HÀNG
INSERT INTO KhachHang (MaKH, HoTen, SDT, CCCD, Email, DiaChi) VALUES
('KH001', N'Nguyễn Minh Tuấn',    '0931845672', '079091234571', 'nguyenminhtuan@gmail.com',   N'123 Nguyễn Huệ, Bến Nghé, Quận 1, TP.HCM'),
('KH002', N'Trần Thị Thanh Hà',   '0768234591', '079185234582', 'tranthithanhha@gmail.com',   N'45 Lê Lợi, Bến Thành, Quận 3, TP.HCM'),
('KH003', N'Lê Quốc Cường',       '0853712946', '079094537893', 'lequoccuong@gmail.com',      N'78 Xô Viết Nghệ Tĩnh, P.24, Bình Thạnh, TP.HCM'),
('KH004', N'Phạm Thị Lan Anh',    '0912638457', '079185812604', 'phamthilananh@gmail.com',    N'12 Phan Văn Trị, P.7, Gò Vấp, TP.HCM'),
('KH005', N'Hoàng Trọng Nghĩa',   '0375824193', '079095634215', 'hoangtrongnghia@gmail.com',  N'56 Võ Văn Ngân, Linh Chiểu, Thủ Đức, TP.HCM'),
('KH006', N'Vũ Thị Mỹ Hạnh',      '0896314725', '079185417326', 'vuthimyhanh@gmail.com',      N'34 Bình Giã, P.13, Quận 10, TP.HCM'),
('KH007', N'Đặng Văn Hùng',       '0582947361', '079093248537', 'dangvanhung@gmail.com',      N'89 Trường Chinh, P.14, Tân Bình, TP.HCM'),
('KH008', N'Bùi Ngọc Hương',      '0703581924', '079180539148', 'buingochuong@gmail.com',     N'21 Nguyễn Thị Thập, Tân Phú, Quận 7, TP.HCM'),
('KH009', N'Đỗ Thanh Khoa',       '0846275319', '079095624759', 'dothankhoa@gmail.com',       N'67 Nguyễn Tất Thành, P.13, Quận 4, TP.HCM'),
('KH010', N'Ngô Thị Phương Linh', '0563748291', '079181234860', 'ngothiphuonglinh@gmail.com', N'15 Kinh Dương Vương, An Lạc, Bình Tân, TP.HCM');
GO

-- 2.4 NHÂN VIÊN
INSERT INTO NhanVien VALUES
('NV001', N'Huỳnh Thanh Phong',    N'Quản lý',              '2019-03-01', '0938145762', N'Đang làm việc'),
('NV002', N'Lê Minh Khang',        N'Nhân viên bán hàng',   '2021-06-15', '0764823915', N'Đang làm việc'),
('NV003', N'Trần Thị Ngọc Lan',    N'Nhân viên bán hàng',   '2022-08-10', '0852917364', N'Đang làm việc'),
('NV004', N'Nguyễn Hữu Tài',       N'Kỹ thuật viên',        '2020-11-20', '0915284736', N'Đang làm việc'),
('NV005', N'Phan Đình Phúc',       N'Kỹ thuật viên',        '2021-04-05', '0376924158', N'Đang làm việc'),
('NV006', N'Cao Thị Mỹ Linh',      N'Kế toán',              '2020-01-10', '0895471362', N'Đang làm việc'),
('NV007', N'Võ Trung Kiên',        N'Bảo vệ',               '2022-02-28', '0584619273', N'Đang làm việc'),
('NV008', N'Đinh Tuấn Anh',        N'Kỹ thuật viên',        '2023-07-01', '0702381945', N'Đang làm việc'),
('NV009', N'Lương Thị Bích Huyền', N'Chăm sóc khách hàng',  '2023-11-15', '0849627153', N'Đang làm việc'),
('NV010', N'Phan Văn Triều',       N'Nhân viên bán hàng',   '2020-09-12', '0561384972', N'Tạm nghỉ');
GO

-- 2.5 TÀI KHOẢN
INSERT INTO TaiKhoan VALUES
('admin',      '123456', N'Quản lý',             'NV001'),
('nv_khang',   '123456', N'Bán hàng',            'NV002'),
('nv_lan',     '123456', N'Bán hàng',            'NV003'),
('kt_tai',     '123456', N'Kỹ thuật',            'NV004'),
('kt_phuc',    '123456', N'Kỹ thuật',            'NV005'),
('kt_linh',    '123456', N'Kế toán',             'NV006'),
('bv_kien',    '123456', N'Bảo vệ',              'NV007'),
('kt_anh',     '123456', N'Kỹ thuật',            'NV008'),
('cskh_huyen', '123456', N'Chăm sóc khách hàng', 'NV009'),
('nv_trieu',   '123456', N'Bán hàng',            'NV010');
GO

-- 2.6 DỊCH VỤ & PHỤ TÙNG
INSERT INTO DichVuPhuTung VALUES
('DV001', N'Bảo dưỡng định kỳ 10.000km',           1500000,    0),
('DV002', N'Rửa xe + dọn nội thất cao cấp',         250000,    0),
('DV003', N'Thay nhớt động cơ + lọc nhớt',          650000,    0),
('DV004', N'Đại tu động cơ',                      12000000,    0),
('DV005', N'Cân chỉnh thước lái + cân mâm',         450000,    0),
('DV006', N'Kiểm tra hệ thống điện ECU',            800000,    0),
('PT001', N'Nhớt Mobil 1 ESP 5W-30 4L',             850000,   40),
('PT002', N'Lọc gió động cơ K&N',                  1200000,   25),
('PT003', N'Bugi NGK Iridium IX',                   280000,  100),
('PT004', N'Lốp Michelin Pilot Sport 4 225/45R17', 5800000,   20),
('PT005', N'Ắc quy Varta Silver Dynamic 70Ah',     2950000,   15),
('PT006', N'Phanh đĩa Brembo trước',               3500000,   18),
('PT007', N'Camera hành trình Vietmap C61',        2400000,   35),
('PT008', N'Phim cách nhiệt 3M Crystalline',       9500000,    8);
GO

-- 2.7 HÓA ĐƠN
INSERT INTO HoaDon (MaHD, NgayLap, MaNV, MaKH, TenDV_SP, SoLuong, ThanhTien, PhuongThucThanhToan) VALUES
('HD001', '2025-01-08 09:15:00', 'NV002', 'KH001', N'Toyota Camry 2.5Q 2024',              1, 1495000000, N'Trả góp'),
('HD002', '2025-01-15 10:30:00', 'NV003', 'KH002', N'Toyota Vios E 2024',                  1, 488000000,  N'Tiền mặt'),
('HD003', '2025-02-03 08:45:00', 'NV004', 'KH003', N'Bảo dưỡng định kỳ 10.000km',          1, 1500000,    N'Tiền mặt'),
('HD004', '2025-02-18 14:00:00', 'NV005', 'KH004', N'Lốp Michelin Pilot Sport 4 225/45R17',4, 23200000,   N'Chuyển khoản'),
('HD005', '2025-03-05 11:00:00', 'NV002', 'KH005', N'Honda Civic RS 2024',                 1, 875000000,  N'Trả góp'),
('HD006', '2025-03-20 13:30:00', 'NV003', 'KH006', N'Mazda CX-5 Premium 2024',             1, 879000000,  N'Chuyển khoản'),
('HD007', '2025-04-02 09:00:00', 'NV004', 'KH007', N'Rửa xe + dọn nội thất cao cấp',       1, 250000,     N'Tiền mặt'),
('HD008', '2025-04-10 15:00:00', 'NV002', 'KH008', N'Hyundai Accent AT 2024',              1, 539000000,  N'Trả góp'),
('HD009', '2025-04-25 10:00:00', 'NV003', 'KH009', N'BMW X5 xDrive40i 2024',               1, 4519000000, N'Trả góp'),
('HD010', '2025-05-07 08:30:00', 'NV002', 'KH010', N'Camera hành trình Vietmap C61',       1, 2400000,    N'Tiền mặt');
GO

-- Lưu ý: PRINT không cho phép chứa subquery, nên dùng SELECT để xem
-- số dòng đã nạp cho từng bảng (kết quả hiện ở tab Results trong SSMS).
PRINT N'>>> DL_OTO đã được tạo và nạp xong dữ liệu mẫu.';
GO
SELECT      N'HangXe'        AS Bang, COUNT(*) AS SoDong FROM HangXe
UNION ALL SELECT N'Xe',            COUNT(*) FROM Xe
UNION ALL SELECT N'KhachHang',     COUNT(*) FROM KhachHang
UNION ALL SELECT N'NhanVien',      COUNT(*) FROM NhanVien
UNION ALL SELECT N'TaiKhoan',      COUNT(*) FROM TaiKhoan
UNION ALL SELECT N'DichVuPhuTung', COUNT(*) FROM DichVuPhuTung
UNION ALL SELECT N'HoaDon',        COUNT(*) FROM HoaDon;
GO

/* ============================================================
   PHẦN 3. CÁC CẤU TRÚC XỬ LÝ (theo yêu cầu đồ án)
   Gồm 5 loại khác nhau: VIEW, FUNCTION, STORED PROCEDURE,
   CURSOR (trong procedure) và TRIGGER.
   ------------------------------------------------------------
   Xoá các đối tượng cũ (nếu chạy lại file) trước khi tạo mới.
   ============================================================ */

IF OBJECT_ID('trg_Xe_LuuLichSuGia', 'TR')   IS NOT NULL DROP TRIGGER trg_Xe_LuuLichSuGia;
IF OBJECT_ID('trg_HoaDon_HoanTra', 'TR')    IS NOT NULL DROP TRIGGER trg_HoaDon_HoanTra;
IF OBJECT_ID('sp_LapHoaDon', 'P')           IS NOT NULL DROP PROCEDURE sp_LapHoaDon;
IF OBJECT_ID('sp_XacNhanDonChoXuLy', 'P')   IS NOT NULL DROP PROCEDURE sp_XacNhanDonChoXuLy;
IF OBJECT_ID('sp_CanhBaoTonKhoThap', 'P')   IS NOT NULL DROP PROCEDURE sp_CanhBaoTonKhoThap;
IF OBJECT_ID('fn_TongDoanhThuNhanVien','FN')IS NOT NULL DROP FUNCTION fn_TongDoanhThuNhanVien;
IF OBJECT_ID('fn_XepHangKhachHang', 'FN')   IS NOT NULL DROP FUNCTION fn_XepHangKhachHang;
IF OBJECT_ID('vw_DanhSachNhanVien', 'V')    IS NOT NULL DROP VIEW vw_DanhSachNhanVien;
IF OBJECT_ID('vw_TonKho', 'V')              IS NOT NULL DROP VIEW vw_TonKho;
IF OBJECT_ID('vw_XeBanChay', 'V')           IS NOT NULL DROP VIEW vw_XeBanChay;
GO

/* ------------------------------------------------------------
   3.1 VIEW – phục vụ thống kê (yêu cầu: xây dựng View thống kê)
   ------------------------------------------------------------ */

-- View 1: Danh sách nhân viên kèm số hóa đơn đã lập và tổng doanh thu.
CREATE VIEW vw_DanhSachNhanVien AS
SELECT  nv.MaNV,
        nv.HoTen,
        nv.ChucVu,
        nv.SDT,
        nv.TrangThai,
        COUNT(hd.MaHD)                AS SoHoaDon,
        ISNULL(SUM(hd.ThanhTien), 0)  AS TongDoanhThu
FROM        NhanVien nv
LEFT JOIN   HoaDon   hd ON hd.MaNV = nv.MaNV
GROUP BY    nv.MaNV, nv.HoTen, nv.ChucVu, nv.SDT, nv.TrangThai;
GO

-- View 2: Tồn kho xe theo từng hãng, kèm tình trạng còn/sắp hết/hết hàng.
CREATE VIEW vw_TonKho AS
SELECT  x.MaXe,
        x.TenXe,
        h.TenHang,
        x.GiaBan,
        x.SoLuongTon,
        CASE WHEN x.SoLuongTon <= 0 THEN N'Hết hàng'
             WHEN x.SoLuongTon < 5  THEN N'Sắp hết'
             ELSE                        N'Còn hàng'
        END AS TinhTrang
FROM        Xe     x
LEFT JOIN   HangXe h ON h.MaHang = x.MaHang;
GO

-- View 3: Xe bán chạy – tổng số lượng bán và doanh thu lấy từ hóa đơn.
CREATE VIEW vw_XeBanChay AS
SELECT  x.MaXe,
        x.TenXe,
        h.TenHang,
        SUM(hd.SoLuong)    AS SoLuongBan,
        SUM(hd.ThanhTien)  AS DoanhThu
FROM        HoaDon hd
INNER JOIN  Xe     x ON x.TenXe  = hd.TenDV_SP
INNER JOIN  HangXe h ON h.MaHang = x.MaHang
GROUP BY    x.MaXe, x.TenXe, h.TenHang;
GO

/* ------------------------------------------------------------
   3.2 FUNCTION – hàm tự định nghĩa
   ------------------------------------------------------------ */

-- Function 1: Tính tổng doanh thu của một nhân viên.
CREATE FUNCTION fn_TongDoanhThuNhanVien(@MaNV VARCHAR(20))
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @Tong DECIMAL(18,2);
    SELECT  @Tong = ISNULL(SUM(ThanhTien), 0)
    FROM    HoaDon
    WHERE   MaNV = @MaNV;
    RETURN @Tong;
END;
GO

-- Function 2: Xếp hạng khách hàng dựa trên tổng tiền đã mua.
CREATE FUNCTION fn_XepHangKhachHang(@MaKH VARCHAR(20))
RETURNS NVARCHAR(30)
AS
BEGIN
    DECLARE @Tong DECIMAL(18,2);
    SELECT  @Tong = ISNULL(SUM(ThanhTien), 0)
    FROM    HoaDon
    WHERE   MaKH = @MaKH;

    DECLARE @XepHang NVARCHAR(30);
    IF      @Tong >= 2000000000 SET @XepHang = N'Khách VIP';
    ELSE IF @Tong >=  500000000 SET @XepHang = N'Khách thân thiết';
    ELSE                        SET @XepHang = N'Khách thường';
    RETURN @XepHang;
END;
GO

/* ------------------------------------------------------------
   3.3 STORED PROCEDURE – xử lý nghiệp vụ
   ------------------------------------------------------------ */

-- Procedure 1: Lập hóa đơn bán xe.
-- Tự sinh mã hóa đơn, kiểm tra tồn kho, trừ kho, dùng transaction + TRY/CATCH.
CREATE PROCEDURE sp_LapHoaDon
    @MaNV       VARCHAR(20),
    @MaKH       VARCHAR(20),
    @MaXe       VARCHAR(20),
    @SoLuong    INT,
    @PhuongThuc NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Ton INT, @Gia DECIMAL(18,2), @TenXe NVARCHAR(100);
        SELECT  @Ton = SoLuongTon, @Gia = GiaBan, @TenXe = TenXe
        FROM    Xe WHERE MaXe = @MaXe;

        IF @TenXe IS NULL
        BEGIN
            RAISERROR(N'Khong tim thay xe.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END
        IF @SoLuong <= 0
        BEGIN
            RAISERROR(N'So luong phai lon hon 0.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END
        IF @Ton < @SoLuong
        BEGIN
            RAISERROR(N'Khong du ton kho.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Sinh mã hóa đơn mới dạng HDxxx.
        DECLARE @SoMoi INT;
        SELECT  @SoMoi = ISNULL(MAX(CAST(SUBSTRING(MaHD, 3, 10) AS INT)), 0) + 1
        FROM    HoaDon WHERE MaHD LIKE 'HD%';
        DECLARE @MaHD VARCHAR(20) = 'HD' + RIGHT('000' + CAST(@SoMoi AS VARCHAR(10)), 3);

        INSERT INTO HoaDon (MaHD, NgayLap, MaNV, MaKH, TenDV_SP, SoLuong, ThanhTien, PhuongThucThanhToan, TrangThai)
        VALUES (@MaHD, GETDATE(), @MaNV, @MaKH, @TenXe, @SoLuong, @Gia * @SoLuong, @PhuongThuc, N'Đã xác nhận');

        UPDATE Xe SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaXe = @MaXe;

        COMMIT TRANSACTION;
        PRINT N'>>> Da lap hoa don ' + @MaHD;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        PRINT N'>>> Loi: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

-- Procedure 2: Xác nhận toàn bộ đơn 'Chờ xác nhận' của 1 khách, gán nhân viên phụ trách.
CREATE PROCEDURE sp_XacNhanDonChoXuLy
    @MaKH VARCHAR(20),
    @MaNV VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE  HoaDon
    SET     TrangThai = N'Đã xác nhận', MaNV = @MaNV
    WHERE   MaKH = @MaKH AND TrangThai = N'Chờ xác nhận';
    PRINT N'>>> Da xac nhan ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' dong hoa don.';
END;
GO

-- Procedure 3 (CURSOR): duyệt từng xe có tồn kho thấp để in cảnh báo nhập hàng.
CREATE PROCEDURE sp_CanhBaoTonKhoThap
    @Nguong INT = 5
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaXe VARCHAR(20), @TenXe NVARCHAR(100), @Ton INT, @Dem INT = 0;

    DECLARE con_xe CURSOR FOR
        SELECT  MaXe, TenXe, SoLuongTon
        FROM    Xe
        WHERE   SoLuongTon < @Nguong
        ORDER BY SoLuongTon;

    OPEN con_xe;
    FETCH NEXT FROM con_xe INTO @MaXe, @TenXe, @Ton;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT N'Canh bao: ' + @TenXe + N' (' + @MaXe + N') chi con '
              + CAST(@Ton AS NVARCHAR(10)) + N' chiec.';
        SET @Dem = @Dem + 1;
        FETCH NEXT FROM con_xe INTO @MaXe, @TenXe, @Ton;
    END
    CLOSE con_xe;
    DEALLOCATE con_xe;

    PRINT N'>>> Tong so xe can nhap them: ' + CAST(@Dem AS NVARCHAR(10));
END;
GO

/* ------------------------------------------------------------
   3.4 TRIGGER – xử lý tự động
   ------------------------------------------------------------ */

-- Bảng phụ lưu lịch sử thay đổi giá xe (phục vụ trigger audit bên dưới).
IF OBJECT_ID('LichSuGiaXe', 'U') IS NULL
CREATE TABLE LichSuGiaXe (
    ID       INT IDENTITY(1,1) PRIMARY KEY,
    MaXe     VARCHAR(20),
    GiaCu    DECIMAL(18,2),
    GiaMoi   DECIMAL(18,2),
    ThoiGian DATETIME DEFAULT GETDATE()
);
GO

-- Trigger 1 (AFTER UPDATE): tự động ghi log mỗi khi giá bán của xe thay đổi.
CREATE TRIGGER trg_Xe_LuuLichSuGia
ON Xe AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO LichSuGiaXe (MaXe, GiaCu, GiaMoi)
    SELECT  i.MaXe, d.GiaBan, i.GiaBan
    FROM    inserted i
    INNER JOIN deleted d ON i.MaXe = d.MaXe
    WHERE   ISNULL(i.GiaBan, 0) <> ISNULL(d.GiaBan, 0);
END;
GO

-- Trigger 2 (AFTER DELETE): khi xóa hóa đơn thì hoàn trả lại tồn kho cho xe.
CREATE TRIGGER trg_HoaDon_HoanTra
ON HoaDon AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE  x
    SET     x.SoLuongTon = x.SoLuongTon + d.SoLuong
    FROM    Xe x
    INNER JOIN deleted d ON x.TenXe = d.TenDV_SP
    WHERE   d.SoLuong IS NOT NULL;
END;
GO

/* ------------------------------------------------------------
   3.5 VÍ DỤ GỌI THỬ (bỏ dấu -- để chạy demo cho cô xem)
   ------------------------------------------------------------ */
-- SELECT * FROM vw_DanhSachNhanVien;
-- SELECT * FROM vw_TonKho;
-- SELECT * FROM vw_XeBanChay;
-- SELECT dbo.fn_TongDoanhThuNhanVien('NV002') AS DoanhThu_NV002;
-- SELECT dbo.fn_XepHangKhachHang('KH009')     AS XepHang_KH009;
-- EXEC sp_CanhBaoTonKhoThap 5;
-- EXEC sp_LapHoaDon 'NV002', 'KH001', 'XE02', 1, N'Tiền mặt';
-- EXEC sp_XacNhanDonChoXuLy 'KH001', 'NV002';

PRINT N'>>> Da tao xong VIEW / FUNCTION / PROCEDURE / CURSOR / TRIGGER.';
GO

/* ============================================================
   PHẦN 4. RÀNG BUỘC UNIQUE & QUAN HỆ 1-1
   (bổ sung cho phần Xây dựng CSDL)
   ------------------------------------------------------------
   - Các bảng đã có sẵn ràng buộc PRIMARY KEY, FOREIGN KEY,
     CHECK (GiaBan > 0, SoLuongTon >= 0, ...) và DEFAULT
     (NgayVaoLam, TrangThai, NgayLap...) ở phần tạo bảng.
   - Phần này bổ sung ràng buộc UNIQUE và thiết lập quan hệ 1-1.
   - Dùng "filtered unique index" (UNIQUE ... WHERE) để vẫn cho
     phép nhiều dòng NULL/rỗng (vd: nhiều tài khoản khách hàng
     có MaNV = NULL, khách đăng ký online chưa nhập CCCD/Email).
   ============================================================ */

-- QUAN HỆ 1-1: mỗi nhân viên chỉ gắn với tối đa MỘT tài khoản đăng nhập.
-- (NhanVien 1 --- 1 TaiKhoan). Tài khoản khách hàng có MaNV = NULL nên được bỏ qua.
CREATE UNIQUE INDEX UX_TaiKhoan_MaNV  ON TaiKhoan(MaNV)  WHERE MaNV  IS NOT NULL;
GO

-- Số điện thoại khách hàng là duy nhất.
CREATE UNIQUE INDEX UX_KhachHang_SDT  ON KhachHang(SDT)  WHERE SDT   IS NOT NULL;
GO
-- CCCD duy nhất (bỏ qua rỗng vì khách đăng ký online có thể chưa nhập).
CREATE UNIQUE INDEX UX_KhachHang_CCCD ON KhachHang(CCCD) WHERE CCCD  IS NOT NULL AND CCCD  <> '';
GO
-- Email duy nhất (bỏ qua rỗng).
CREATE UNIQUE INDEX UX_KhachHang_Email ON KhachHang(Email) WHERE Email IS NOT NULL AND Email <> '';
GO
-- Số điện thoại nhân viên là duy nhất.
CREATE UNIQUE INDEX UX_NhanVien_SDT   ON NhanVien(SDT)   WHERE SDT   IS NOT NULL;
GO

/* ============================================================
   PHẦN 5. MÔ TẢ THUỘC TÍNH TỪNG BẢNG (extended properties)
   ------------------------------------------------------------
   Mô tả sẽ hiển thị trong SSMS ở cột Description khi xem thiết kế
   bảng, hoặc truy vấn từ sys.extended_properties.
   ============================================================ */

-- Bảng HangXe
EXEC sp_addextendedproperty 'MS_Description', N'Hãng xe (thương hiệu)', 'SCHEMA','dbo','TABLE','HangXe',NULL,NULL;
EXEC sp_addextendedproperty 'MS_Description', N'Mã hãng - khóa chính',          'SCHEMA','dbo','TABLE','HangXe','COLUMN','MaHang';
EXEC sp_addextendedproperty 'MS_Description', N'Tên hãng xe',                    'SCHEMA','dbo','TABLE','HangXe','COLUMN','TenHang';
EXEC sp_addextendedproperty 'MS_Description', N'Quốc gia của hãng',              'SCHEMA','dbo','TABLE','HangXe','COLUMN','QuocGia';
EXEC sp_addextendedproperty 'MS_Description', N'Đường dẫn ảnh logo',             'SCHEMA','dbo','TABLE','HangXe','COLUMN','LogoPath';
GO

-- Bảng Xe
EXEC sp_addextendedproperty 'MS_Description', N'Xe ô tô đang bán', 'SCHEMA','dbo','TABLE','Xe',NULL,NULL;
EXEC sp_addextendedproperty 'MS_Description', N'Mã xe - khóa chính',             'SCHEMA','dbo','TABLE','Xe','COLUMN','MaXe';
EXEC sp_addextendedproperty 'MS_Description', N'Tên/đời xe',                      'SCHEMA','dbo','TABLE','Xe','COLUMN','TenXe';
EXEC sp_addextendedproperty 'MS_Description', N'Loại xe (Sedan, SUV, ...)',       'SCHEMA','dbo','TABLE','Xe','COLUMN','LoaiXe';
EXEC sp_addextendedproperty 'MS_Description', N'Năm sản xuất',                    'SCHEMA','dbo','TABLE','Xe','COLUMN','NamSX';
EXEC sp_addextendedproperty 'MS_Description', N'Giá bán (VNĐ)',                   'SCHEMA','dbo','TABLE','Xe','COLUMN','GiaBan';
EXEC sp_addextendedproperty 'MS_Description', N'Màu sắc',                         'SCHEMA','dbo','TABLE','Xe','COLUMN','MauSac';
EXEC sp_addextendedproperty 'MS_Description', N'Mô tả chi tiết',                  'SCHEMA','dbo','TABLE','Xe','COLUMN','MoTa';
EXEC sp_addextendedproperty 'MS_Description', N'Đường dẫn ảnh xe',               'SCHEMA','dbo','TABLE','Xe','COLUMN','HinhAnh';
EXEC sp_addextendedproperty 'MS_Description', N'Mã hãng - khóa ngoại tới HangXe', 'SCHEMA','dbo','TABLE','Xe','COLUMN','MaHang';
EXEC sp_addextendedproperty 'MS_Description', N'Số lượng tồn kho',               'SCHEMA','dbo','TABLE','Xe','COLUMN','SoLuongTon';
GO

-- Bảng KhachHang
EXEC sp_addextendedproperty 'MS_Description', N'Khách hàng', 'SCHEMA','dbo','TABLE','KhachHang',NULL,NULL;
EXEC sp_addextendedproperty 'MS_Description', N'Mã khách hàng - khóa chính',     'SCHEMA','dbo','TABLE','KhachHang','COLUMN','MaKH';
EXEC sp_addextendedproperty 'MS_Description', N'Họ tên khách',                    'SCHEMA','dbo','TABLE','KhachHang','COLUMN','HoTen';
EXEC sp_addextendedproperty 'MS_Description', N'Số điện thoại (dùng đăng nhập)',  'SCHEMA','dbo','TABLE','KhachHang','COLUMN','SDT';
EXEC sp_addextendedproperty 'MS_Description', N'Căn cước công dân',               'SCHEMA','dbo','TABLE','KhachHang','COLUMN','CCCD';
EXEC sp_addextendedproperty 'MS_Description', N'Email',                           'SCHEMA','dbo','TABLE','KhachHang','COLUMN','Email';
EXEC sp_addextendedproperty 'MS_Description', N'Địa chỉ',                         'SCHEMA','dbo','TABLE','KhachHang','COLUMN','DiaChi';
EXEC sp_addextendedproperty 'MS_Description', N'Ngày sinh',                       'SCHEMA','dbo','TABLE','KhachHang','COLUMN','NgaySinh';
EXEC sp_addextendedproperty 'MS_Description', N'Giới tính',                       'SCHEMA','dbo','TABLE','KhachHang','COLUMN','GioiTinh';
EXEC sp_addextendedproperty 'MS_Description', N'Đường dẫn ảnh cá nhân',          'SCHEMA','dbo','TABLE','KhachHang','COLUMN','AnhCaNhan';
GO

-- Bảng NhanVien
EXEC sp_addextendedproperty 'MS_Description', N'Nhân viên', 'SCHEMA','dbo','TABLE','NhanVien',NULL,NULL;
EXEC sp_addextendedproperty 'MS_Description', N'Mã nhân viên - khóa chính',      'SCHEMA','dbo','TABLE','NhanVien','COLUMN','MaNV';
EXEC sp_addextendedproperty 'MS_Description', N'Họ tên nhân viên',               'SCHEMA','dbo','TABLE','NhanVien','COLUMN','HoTen';
EXEC sp_addextendedproperty 'MS_Description', N'Chức vụ',                         'SCHEMA','dbo','TABLE','NhanVien','COLUMN','ChucVu';
EXEC sp_addextendedproperty 'MS_Description', N'Ngày vào làm',                    'SCHEMA','dbo','TABLE','NhanVien','COLUMN','NgayVaoLam';
EXEC sp_addextendedproperty 'MS_Description', N'Số điện thoại',                   'SCHEMA','dbo','TABLE','NhanVien','COLUMN','SDT';
EXEC sp_addextendedproperty 'MS_Description', N'Trạng thái (Đang làm việc/Tạm nghỉ)','SCHEMA','dbo','TABLE','NhanVien','COLUMN','TrangThai';
GO

-- Bảng TaiKhoan
EXEC sp_addextendedproperty 'MS_Description', N'Tài khoản đăng nhập', 'SCHEMA','dbo','TABLE','TaiKhoan',NULL,NULL;
EXEC sp_addextendedproperty 'MS_Description', N'Tên đăng nhập - khóa chính',     'SCHEMA','dbo','TABLE','TaiKhoan','COLUMN','Username';
EXEC sp_addextendedproperty 'MS_Description', N'Mật khẩu',                        'SCHEMA','dbo','TABLE','TaiKhoan','COLUMN','Password';
EXEC sp_addextendedproperty 'MS_Description', N'Vai trò/quyền (KhachHang, Quản lý, Bán hàng...)','SCHEMA','dbo','TABLE','TaiKhoan','COLUMN','Role';
EXEC sp_addextendedproperty 'MS_Description', N'Mã nhân viên - khóa ngoại (NULL nếu là khách)','SCHEMA','dbo','TABLE','TaiKhoan','COLUMN','MaNV';
GO

-- Bảng DichVuPhuTung
EXEC sp_addextendedproperty 'MS_Description', N'Dịch vụ và phụ tùng', 'SCHEMA','dbo','TABLE','DichVuPhuTung',NULL,NULL;
EXEC sp_addextendedproperty 'MS_Description', N'Mã DV/PT - khóa chính',          'SCHEMA','dbo','TABLE','DichVuPhuTung','COLUMN','MaPT';
EXEC sp_addextendedproperty 'MS_Description', N'Tên dịch vụ/phụ tùng',           'SCHEMA','dbo','TABLE','DichVuPhuTung','COLUMN','Ten';
EXEC sp_addextendedproperty 'MS_Description', N'Giá (VNĐ)',                       'SCHEMA','dbo','TABLE','DichVuPhuTung','COLUMN','Gia';
EXEC sp_addextendedproperty 'MS_Description', N'Tồn kho (dịch vụ = 0)',           'SCHEMA','dbo','TABLE','DichVuPhuTung','COLUMN','TonKho';
GO

-- Bảng HoaDon
EXEC sp_addextendedproperty 'MS_Description', N'Hóa đơn bán hàng/đơn đặt', 'SCHEMA','dbo','TABLE','HoaDon',NULL,NULL;
EXEC sp_addextendedproperty 'MS_Description', N'Mã hóa đơn - khóa chính',        'SCHEMA','dbo','TABLE','HoaDon','COLUMN','MaHD';
EXEC sp_addextendedproperty 'MS_Description', N'Ngày lập',                        'SCHEMA','dbo','TABLE','HoaDon','COLUMN','NgayLap';
EXEC sp_addextendedproperty 'MS_Description', N'Mã NV lập - khóa ngoại (NULL nếu khách tự đặt)','SCHEMA','dbo','TABLE','HoaDon','COLUMN','MaNV';
EXEC sp_addextendedproperty 'MS_Description', N'Mã khách - khóa ngoại',          'SCHEMA','dbo','TABLE','HoaDon','COLUMN','MaKH';
EXEC sp_addextendedproperty 'MS_Description', N'Tên dịch vụ/sản phẩm',           'SCHEMA','dbo','TABLE','HoaDon','COLUMN','TenDV_SP';
EXEC sp_addextendedproperty 'MS_Description', N'Số lượng',                        'SCHEMA','dbo','TABLE','HoaDon','COLUMN','SoLuong';
EXEC sp_addextendedproperty 'MS_Description', N'Thành tiền (VNĐ)',                'SCHEMA','dbo','TABLE','HoaDon','COLUMN','ThanhTien';
EXEC sp_addextendedproperty 'MS_Description', N'Phương thức thanh toán',          'SCHEMA','dbo','TABLE','HoaDon','COLUMN','PhuongThucThanhToan';
EXEC sp_addextendedproperty 'MS_Description', N'Trạng thái (Chờ xác nhận/Đã xác nhận)','SCHEMA','dbo','TABLE','HoaDon','COLUMN','TrangThai';
GO

/* ============================================================
   PHẦN 6. LƯỢC ĐỒ QUAN HỆ (DIAGRAM)
   ------------------------------------------------------------
   Diagram là đối tượng đồ họa, không tạo bằng câu lệnh T-SQL.
   Cách tạo trong SSMS để chụp nộp:
     1. Mở database DL_OTO > chuột phải "Database Diagrams" > New Database Diagram.
        (Nếu hỏi cài đối tượng hỗ trợ thì bấm Yes.)
     2. Add 7 bảng: HangXe, Xe, KhachHang, NhanVien, TaiKhoan,
        DichVuPhuTung, HoaDon.
     3. SSMS tự vẽ các đường khóa ngoại:
        - HangXe (1) --- (n) Xe
        - NhanVien (1) --- (n) HoaDon
        - KhachHang (1) --- (n) HoaDon
        - NhanVien (1) --- (1) TaiKhoan   (do UNIQUE UX_TaiKhoan_MaNV)
     4. Save (Ctrl+S), đặt tên "SoDoQuanHe", rồi chụp màn hình.
   ============================================================ */

PRINT N'>>> Da them UNIQUE, quan he 1-1 va mo ta thuoc tinh.';
GO
