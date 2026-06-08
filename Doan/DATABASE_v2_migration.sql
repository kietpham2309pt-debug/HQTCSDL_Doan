/* ============================================================
   DL_OTO  -  NÂNG CẤP V2 (chạy BỔ SUNG sau DATABASE.sql)
   ------------------------------------------------------------
   Script idempotent (chạy lại nhiều lần vẫn an toàn).
   Bổ sung cho các chức năng mới:
     - Ẩn/hiện xe              (Xe.TrangThaiHienThi)
     - Tách Dịch vụ / Phụ tùng (DichVuPhuTung.Loai)
     - Khoá tài khoản + quên mật khẩu bằng câu hỏi bảo mật
       (TaiKhoan.TrangThai / CauHoiBaoMat / CauTraLoiBaoMat)
     - Nhập kho + lịch sử nhập (PhieuNhap / ChiTietPhieuNhap)
     - VIEW thống kê chi tiết, PROC nghiệp vụ, TRIGGER tự động
   ============================================================ */

USE DL_OTO;
GO

-- Bắt buộc cho bảng có filtered index / computed column persisted
-- (các SET này giữ nguyên cho cả phiên kết nối, áp dụng cho mọi batch bên dưới).
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

/* ============================================================
   1. THÊM CỘT MỚI VÀO BẢNG CÓ SẴN (idempotent)
   ============================================================ */

-- 1.1 Xe: trạng thái hiển thị (Đang bán = cho khách thấy / Ẩn = chỉ tồn kho)
IF COL_LENGTH('dbo.Xe', 'TrangThaiHienThi') IS NULL
    ALTER TABLE Xe ADD TrangThaiHienThi NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Xe_TrangThaiHienThi DEFAULT N'Đang bán';
GO

-- 1.2 DichVuPhuTung: phân loại rõ Dịch vụ vs Phụ tùng
IF COL_LENGTH('dbo.DichVuPhuTung', 'Loai') IS NULL
    ALTER TABLE DichVuPhuTung ADD Loai NVARCHAR(20) NOT NULL
        CONSTRAINT DF_DichVuPhuTung_Loai DEFAULT N'DichVu';
GO
-- Backfill theo tiền tố mã (DV -> DichVu, PT -> PhuTung)
UPDATE DichVuPhuTung
SET    Loai = CASE WHEN MaPT LIKE 'PT%' THEN N'PhuTung' ELSE N'DichVu' END;
GO

-- 1.3 TaiKhoan: trạng thái khoá + câu hỏi bảo mật cho 'quên mật khẩu'
IF COL_LENGTH('dbo.TaiKhoan', 'TrangThai') IS NULL
    ALTER TABLE TaiKhoan ADD TrangThai NVARCHAR(20) NOT NULL
        CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT N'Hoạt động';
GO
IF COL_LENGTH('dbo.TaiKhoan', 'CauHoiBaoMat') IS NULL
    ALTER TABLE TaiKhoan ADD CauHoiBaoMat NVARCHAR(200) NULL;
GO
IF COL_LENGTH('dbo.TaiKhoan', 'CauTraLoiBaoMat') IS NULL
    ALTER TABLE TaiKhoan ADD CauTraLoiBaoMat NVARCHAR(200) NULL;
GO

-- Seed câu hỏi bảo mật mặc định cho tài khoản nhân viên (để demo 'quên mật khẩu').
-- Câu hỏi: "Mã nhân viên của bạn là gì?"  -> đáp án chính là MaNV (vd: NV001)
UPDATE TaiKhoan
SET    CauHoiBaoMat    = N'Mã nhân viên của bạn là gì?',
       CauTraLoiBaoMat = MaNV
WHERE  MaNV IS NOT NULL
  AND  (CauHoiBaoMat IS NULL OR LTRIM(RTRIM(CauHoiBaoMat)) = N'');
GO

/* ============================================================
   2. BẢNG NHẬP KHO (PhieuNhap / ChiTietPhieuNhap)
   ============================================================ */

IF OBJECT_ID('ChiTietPhieuNhap', 'U') IS NULL
BEGIN
    -- PhieuNhap phải tồn tại trước (FK)
    IF OBJECT_ID('PhieuNhap', 'U') IS NULL
    BEGIN
        CREATE TABLE PhieuNhap (
            MaPN       VARCHAR(20)  PRIMARY KEY,
            NgayNhap   DATETIME     NOT NULL DEFAULT GETDATE(),
            MaNV       VARCHAR(20)  NULL,
            NhaCungCap NVARCHAR(150),
            TongTien   DECIMAL(18,2) NOT NULL DEFAULT 0,
            GhiChu     NVARCHAR(255),
            CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
        );
    END

    CREATE TABLE ChiTietPhieuNhap (
        ID          INT IDENTITY(1,1) PRIMARY KEY,
        MaPN        VARCHAR(20)  NOT NULL,
        LoaiMatHang NVARCHAR(10) NOT NULL,                 -- N'Xe' | N'PhuTung'
        MaXe        VARCHAR(20)  NULL,
        MaPT        VARCHAR(20)  NULL,
        SoLuong     INT          NOT NULL CHECK (SoLuong > 0),
        DonGiaNhap  DECIMAL(18,2) NOT NULL CHECK (DonGiaNhap >= 0),
        ThanhTien   AS (SoLuong * DonGiaNhap) PERSISTED,
        CONSTRAINT FK_CTPN_PhieuNhap FOREIGN KEY (MaPN) REFERENCES PhieuNhap(MaPN),
        CONSTRAINT FK_CTPN_Xe        FOREIGN KEY (MaXe) REFERENCES Xe(MaXe),
        CONSTRAINT FK_CTPN_PhuTung   FOREIGN KEY (MaPT) REFERENCES DichVuPhuTung(MaPT),
        CONSTRAINT CK_CTPN_MatHang   CHECK (
            (LoaiMatHang = N'Xe'      AND MaXe IS NOT NULL AND MaPT IS NULL) OR
            (LoaiMatHang = N'PhuTung' AND MaPT IS NOT NULL AND MaXe IS NULL)
        )
    );
END
ELSE IF OBJECT_ID('PhieuNhap', 'U') IS NULL
BEGIN
    CREATE TABLE PhieuNhap (
        MaPN       VARCHAR(20)  PRIMARY KEY,
        NgayNhap   DATETIME     NOT NULL DEFAULT GETDATE(),
        MaNV       VARCHAR(20)  NULL,
        NhaCungCap NVARCHAR(150),
        TongTien   DECIMAL(18,2) NOT NULL DEFAULT 0,
        GhiChu     NVARCHAR(255),
        CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
    );
END
GO

/* ============================================================
   3. DỌN OBJECT V2 CŨ ĐỂ TẠO LẠI (idempotent)
   ============================================================ */
IF OBJECT_ID('sp_NhapKho', 'P')                 IS NOT NULL DROP PROCEDURE sp_NhapKho;
IF OBJECT_ID('sp_DoiMatKhau', 'P')              IS NOT NULL DROP PROCEDURE sp_DoiMatKhau;
IF OBJECT_ID('sp_CapMatKhau', 'P')              IS NOT NULL DROP PROCEDURE sp_CapMatKhau;
IF OBJECT_ID('sp_DatLaiMatKhauQuaCauHoi', 'P')  IS NOT NULL DROP PROCEDURE sp_DatLaiMatKhauQuaCauHoi;
IF OBJECT_ID('sp_KhoaTaiKhoanTheoNhanVien', 'P')IS NOT NULL DROP PROCEDURE sp_KhoaTaiKhoanTheoNhanVien;
GO
IF OBJECT_ID('trg_ChiTietPhieuNhap_TongTien','TR') IS NOT NULL DROP TRIGGER trg_ChiTietPhieuNhap_TongTien;
IF OBJECT_ID('trg_NhanVien_KhoaTaiKhoan','TR')     IS NOT NULL DROP TRIGGER trg_NhanVien_KhoaTaiKhoan;
GO
IF OBJECT_ID('vw_LichSuNhap', 'V')      IS NOT NULL DROP VIEW vw_LichSuNhap;
IF OBJECT_ID('vw_ThongKeXe', 'V')       IS NOT NULL DROP VIEW vw_ThongKeXe;
IF OBJECT_ID('vw_ThongKeDichVu', 'V')   IS NOT NULL DROP VIEW vw_ThongKeDichVu;
IF OBJECT_ID('vw_ThongKePhuTung', 'V')  IS NOT NULL DROP VIEW vw_ThongKePhuTung;
GO
-- Kiểu bảng (TVP) phải drop SAU proc dùng nó
IF TYPE_ID('dbo.ChiTietNhapType') IS NOT NULL DROP TYPE dbo.ChiTietNhapType;
GO

/* ============================================================
   4. KIỂU BẢNG (TVP) cho nhập kho nhiều dòng trong 1 giao tác
   ============================================================ */
CREATE TYPE dbo.ChiTietNhapType AS TABLE (
    LoaiMatHang NVARCHAR(10),
    MaXe        VARCHAR(20)  NULL,
    MaPT        VARCHAR(20)  NULL,
    SoLuong     INT,
    DonGiaNhap  DECIMAL(18,2)
);
GO

/* ============================================================
   5. STORED PROCEDURE
   ============================================================ */

-- 5.1 Nhập kho: tạo phiếu + nhiều chi tiết, cộng tồn, trong 1 GIAO TÁC.
CREATE PROCEDURE sp_NhapKho
    @MaNV       VARCHAR(20),
    @NhaCungCap NVARCHAR(150),
    @GhiChu     NVARCHAR(255),
    @ChiTiet    dbo.ChiTietNhapType READONLY,
    @MaPN       VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM @ChiTiet)
        BEGIN
            RAISERROR(N'Phieu nhap phai co it nhat 1 dong.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;

        -- Sinh mã phiếu nhập dạng PNxxx
        DECLARE @SoMoi INT;
        SELECT  @SoMoi = ISNULL(MAX(CAST(SUBSTRING(MaPN, 3, 10) AS INT)), 0) + 1
        FROM    PhieuNhap WHERE MaPN LIKE 'PN%';
        SET @MaPN = 'PN' + RIGHT('000' + CAST(@SoMoi AS VARCHAR(10)), 3);

        INSERT INTO PhieuNhap (MaPN, NgayNhap, MaNV, NhaCungCap, TongTien, GhiChu)
        VALUES (@MaPN, GETDATE(), @MaNV, @NhaCungCap, 0, @GhiChu);

        INSERT INTO ChiTietPhieuNhap (MaPN, LoaiMatHang, MaXe, MaPT, SoLuong, DonGiaNhap)
        SELECT @MaPN, LoaiMatHang, MaXe, MaPT, SoLuong, DonGiaNhap FROM @ChiTiet;

        -- Cộng tồn cho XE
        UPDATE x
        SET    x.SoLuongTon = ISNULL(x.SoLuongTon, 0) + ct.TongSL
        FROM   Xe x
        JOIN  (SELECT MaXe, SUM(SoLuong) AS TongSL
               FROM @ChiTiet WHERE LoaiMatHang = N'Xe' GROUP BY MaXe) ct
          ON  ct.MaXe = x.MaXe;

        -- Cộng tồn cho PHỤ TÙNG
        UPDATE p
        SET    p.TonKho = ISNULL(p.TonKho, 0) + ct.TongSL
        FROM   DichVuPhuTung p
        JOIN  (SELECT MaPT, SUM(SoLuong) AS TongSL
               FROM @ChiTiet WHERE LoaiMatHang = N'PhuTung' GROUP BY MaPT) ct
          ON  ct.MaPT = p.MaPT;

        COMMIT TRANSACTION;   -- TongTien được trigger trg_ChiTietPhieuNhap_TongTien tính lại tự động
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- 5.2 Đổi mật khẩu (tự đổi, cần mật khẩu cũ). @KetQua: 1=OK, 0=sai MK cũ, -1=không có TK
CREATE PROCEDURE sp_DoiMatKhau
    @Username   VARCHAR(50),
    @MatKhauCu  VARCHAR(255),
    @MatKhauMoi VARCHAR(255),
    @KetQua     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @KetQua = 0;
    IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE Username = @Username)
    BEGIN SET @KetQua = -1; RETURN; END
    IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE Username = @Username AND Password = @MatKhauCu)
    BEGIN SET @KetQua = 0; RETURN; END
    UPDATE TaiKhoan SET Password = @MatKhauMoi WHERE Username = @Username;
    SET @KetQua = 1;
END;
GO

-- 5.3 Admin cấp lại mật khẩu cho nhân viên (không cần MK cũ).
CREATE PROCEDURE sp_CapMatKhau
    @Username   VARCHAR(50),
    @MatKhauMoi VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE TaiKhoan SET Password = @MatKhauMoi WHERE Username = @Username;
END;
GO

-- 5.4 Quên mật khẩu: đặt lại qua câu hỏi bảo mật. @KetQua: 1=OK, 0=sai đáp án, -1=không có TK
CREATE PROCEDURE sp_DatLaiMatKhauQuaCauHoi
    @Username   VARCHAR(50),
    @CauTraLoi  NVARCHAR(200),
    @MatKhauMoi VARCHAR(255),
    @KetQua     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @KetQua = 0;
    IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE Username = @Username)
    BEGIN SET @KetQua = -1; RETURN; END
    IF NOT EXISTS (
        SELECT 1 FROM TaiKhoan
        WHERE Username = @Username
          AND LOWER(LTRIM(RTRIM(CauTraLoiBaoMat))) = LOWER(LTRIM(RTRIM(@CauTraLoi)))
    )
    BEGIN SET @KetQua = 0; RETURN; END
    UPDATE TaiKhoan SET Password = @MatKhauMoi WHERE Username = @Username;
    SET @KetQua = 1;
END;
GO

-- 5.5 Khoá/mở khoá toàn bộ tài khoản của 1 nhân viên (@Khoa: 1=khoá, 0=mở).
CREATE PROCEDURE sp_KhoaTaiKhoanTheoNhanVien
    @MaNV VARCHAR(20),
    @Khoa BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE TaiKhoan
    SET    TrangThai = CASE WHEN @Khoa = 1 THEN N'Đã khóa' ELSE N'Hoạt động' END
    WHERE  MaNV = @MaNV;
END;
GO

/* ============================================================
   6. TRIGGER
   ============================================================ */

-- 6.1 Tự tính lại TongTien của phiếu nhập khi chi tiết thay đổi.
CREATE TRIGGER trg_ChiTietPhieuNhap_TongTien
ON ChiTietPhieuNhap AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH PN_AFFECTED AS (
        SELECT MaPN FROM inserted
        UNION
        SELECT MaPN FROM deleted
    )
    UPDATE pn
    SET    pn.TongTien = ISNULL((SELECT SUM(ct.ThanhTien)
                                 FROM ChiTietPhieuNhap ct WHERE ct.MaPN = pn.MaPN), 0)
    FROM   PhieuNhap pn
    JOIN   PN_AFFECTED a ON a.MaPN = pn.MaPN;
END;
GO

-- 6.2 Khi nhân viên chuyển 'Đã nghỉ việc' -> tự khoá tài khoản; quay lại làm -> mở khoá.
CREATE TRIGGER trg_NhanVien_KhoaTaiKhoan
ON NhanVien AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tk
    SET    tk.TrangThai = N'Đã khóa'
    FROM   TaiKhoan tk
    JOIN   inserted i ON tk.MaNV = i.MaNV
    JOIN   deleted  d ON d.MaNV = i.MaNV
    WHERE  i.TrangThai = N'Đã nghỉ việc'
      AND  ISNULL(d.TrangThai, N'') <> N'Đã nghỉ việc';

    UPDATE tk
    SET    tk.TrangThai = N'Hoạt động'
    FROM   TaiKhoan tk
    JOIN   inserted i ON tk.MaNV = i.MaNV
    JOIN   deleted  d ON d.MaNV = i.MaNV
    WHERE  i.TrangThai = N'Đang làm việc'
      AND  d.TrangThai = N'Đã nghỉ việc';
END;
GO

/* ============================================================
   7. VIEW thống kê chi tiết
   ============================================================ */

-- 7.1 Lịch sử nhập (phẳng theo từng dòng chi tiết)
CREATE VIEW vw_LichSuNhap AS
SELECT  pn.MaPN,
        pn.NgayNhap,
        pn.NhaCungCap,
        pn.GhiChu,
        pn.TongTien,
        pn.MaNV,
        nv.HoTen AS TenNhanVien,
        ct.ID    AS ChiTietID,
        ct.LoaiMatHang,
        CASE WHEN ct.LoaiMatHang = N'Xe' THEN ct.MaXe ELSE ct.MaPT END AS MaMatHang,
        CASE WHEN ct.LoaiMatHang = N'Xe' THEN x.TenXe ELSE p.Ten END   AS TenMatHang,
        ct.SoLuong,
        ct.DonGiaNhap,
        ct.ThanhTien
FROM    PhieuNhap pn
LEFT JOIN NhanVien        nv ON nv.MaNV = pn.MaNV
LEFT JOIN ChiTietPhieuNhap ct ON ct.MaPN = pn.MaPN
LEFT JOIN Xe              x  ON x.MaXe  = ct.MaXe
LEFT JOIN DichVuPhuTung   p  ON p.MaPT  = ct.MaPT;
GO

-- 7.2 Thống kê xe theo hãng (số mẫu, tồn, giá trị tồn, đang bán/ẩn)
CREATE VIEW vw_ThongKeXe AS
SELECT  h.MaHang,
        h.TenHang,
        COUNT(x.MaXe)                                   AS SoMauXe,
        ISNULL(SUM(x.SoLuongTon), 0)                    AS TongTon,
        ISNULL(SUM(x.SoLuongTon * x.GiaBan), 0)         AS GiaTriTon,
        SUM(CASE WHEN x.TrangThaiHienThi = N'Đang bán' THEN 1 ELSE 0 END) AS SoXeDangBan,
        SUM(CASE WHEN x.TrangThaiHienThi = N'Ẩn'       THEN 1 ELSE 0 END) AS SoXeAn
FROM    HangXe h
LEFT JOIN Xe x ON x.MaHang = h.MaHang
GROUP BY h.MaHang, h.TenHang;
GO

-- 7.3 Thống kê dịch vụ (doanh thu, số lượng bán theo tên)
CREATE VIEW vw_ThongKeDichVu AS
SELECT  d.MaPT,
        d.Ten,
        d.Gia,
        ISNULL(SUM(hd.SoLuong), 0)   AS SoLuongBan,
        ISNULL(SUM(hd.ThanhTien), 0) AS DoanhThu
FROM    DichVuPhuTung d
LEFT JOIN HoaDon hd ON hd.TenDV_SP = d.Ten
WHERE   d.Loai = N'DichVu'
GROUP BY d.MaPT, d.Ten, d.Gia;
GO

-- 7.4 Thống kê phụ tùng (doanh thu, số lượng bán, tồn kho)
CREATE VIEW vw_ThongKePhuTung AS
SELECT  d.MaPT,
        d.Ten,
        d.Gia,
        d.TonKho,
        ISNULL(SUM(hd.SoLuong), 0)   AS SoLuongBan,
        ISNULL(SUM(hd.ThanhTien), 0) AS DoanhThu
FROM    DichVuPhuTung d
LEFT JOIN HoaDon hd ON hd.TenDV_SP = d.Ten
WHERE   d.Loai = N'PhuTung'
GROUP BY d.MaPT, d.Ten, d.Gia, d.TonKho;
GO

PRINT N'>>> NÂNG CẤP V2 hoàn tất: cột mới, bảng nhập kho, PROC/VIEW/TRIGGER/TVP.';
GO

/* ============================================================
   8. SỬA DỮ LIỆU + DEFAULT (đảm bảo Unicode đúng, idempotent)
   ------------------------------------------------------------
   Đặt lại DEFAULT constraint và chuẩn hóa các giá trị tiếng Việt
   để khớp với chuỗi so sánh trong ứng dụng C#.
   ============================================================ */
IF OBJECT_ID('DF_Xe_TrangThaiHienThi', 'D') IS NOT NULL
    ALTER TABLE Xe DROP CONSTRAINT DF_Xe_TrangThaiHienThi;
ALTER TABLE Xe ADD CONSTRAINT DF_Xe_TrangThaiHienThi DEFAULT N'Đang bán' FOR TrangThaiHienThi;
GO
IF OBJECT_ID('DF_TaiKhoan_TrangThai', 'D') IS NOT NULL
    ALTER TABLE TaiKhoan DROP CONSTRAINT DF_TaiKhoan_TrangThai;
ALTER TABLE TaiKhoan ADD CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT N'Hoạt động' FOR TrangThai;
GO
-- Chuẩn hóa giá trị: gom mọi biến thể về đúng chuẩn (xử lý cả dữ liệu đã hỏng mã hóa trước đó)
UPDATE Xe       SET TrangThaiHienThi = N'Đang bán'  WHERE TrangThaiHienThi IS NULL OR TrangThaiHienThi NOT IN (N'Đang bán', N'Ẩn');
UPDATE TaiKhoan SET TrangThai        = N'Hoạt động' WHERE TrangThai IS NULL OR TrangThai NOT IN (N'Hoạt động', N'Đã khóa');
UPDATE TaiKhoan SET CauHoiBaoMat     = N'Mã nhân viên của bạn là gì?' WHERE MaNV IS NOT NULL;
GO
-- Bổ sung Ngày sinh / Giới tính còn trống cho khách hàng mẫu (cho danh sách đầy đủ)
UPDATE KhachHang
SET    GioiTinh = CASE WHEN CAST(SUBSTRING(MaKH, 3, 3) AS INT) % 2 = 0 THEN N'Nữ' ELSE N'Nam' END
WHERE  GioiTinh IS NULL OR LTRIM(RTRIM(GioiTinh)) = N'';
UPDATE KhachHang
SET    NgaySinh = DATEADD(YEAR, -(25 + CAST(SUBSTRING(MaKH, 3, 3) AS INT)), '2026-01-01')
WHERE  NgaySinh IS NULL;
GO
PRINT N'>>> Da chuan hoa du lieu tieng Viet.';
GO

/* ------------------------------------------------------------
   VÍ DỤ GỌI THỬ (bỏ -- để demo)
   ------------------------------------------------------------
-- Nhập kho 2 dòng (1 xe + 1 phụ tùng):
DECLARE @ct dbo.ChiTietNhapType, @pn VARCHAR(20);
INSERT INTO @ct VALUES (N'Xe', 'XE64', NULL, 5, 300000000),
                       (N'PhuTung', NULL, 'PT003', 50, 250000);
EXEC sp_NhapKho 'NV004', N'Cong ty ABC', N'Nhap dau ky', @ct, @pn OUTPUT;
SELECT @pn AS MaPhieuVuaTao;
SELECT * FROM vw_LichSuNhap WHERE MaPN = @pn;

-- Quên mật khẩu (đáp án = MaNV):
DECLARE @kq INT;
EXEC sp_DatLaiMatKhauQuaCauHoi 'kt_tai', N'NV004', 'matmoi123', @kq OUTPUT;  -- @kq = 1
SELECT @kq;

-- Cho nhân viên nghỉ việc -> trigger tự khoá tài khoản:
UPDATE NhanVien SET TrangThai = N'Đã nghỉ việc' WHERE MaNV = 'NV010';
SELECT Username, TrangThai FROM TaiKhoan WHERE MaNV = 'NV010';  -- 'Đã khóa'

SELECT * FROM vw_ThongKeXe;
SELECT * FROM vw_ThongKeDichVu;
SELECT * FROM vw_ThongKePhuTung;
   ------------------------------------------------------------ */
