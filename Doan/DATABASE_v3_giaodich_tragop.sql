/* ============================================================
   DL_OTO - NÂNG CẤP V3
   - HoaDon.MaGiaoDich: gộp nhiều mặt hàng 1 lần mua thành 1 giao dịch
   - PhieuTraGop / ChiTietTraGop: chức năng trả góp + theo dõi
   CHẠY: sqlcmd -S . -E -I -f 65001 -i DATABASE_v3_giaodich_tragop.sql
   (hoặc mở bằng SSMS)
   ============================================================ */
USE DL_OTO;
GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

/* 1. HoaDon.MaGiaoDich */
IF COL_LENGTH('dbo.HoaDon', 'MaGiaoDich') IS NULL
    ALTER TABLE HoaDon ADD MaGiaoDich VARCHAR(20) NULL;
GO
-- Dữ liệu cũ: mỗi hóa đơn là 1 giao dịch riêng.
UPDATE HoaDon SET MaGiaoDich = MaHD WHERE MaGiaoDich IS NULL OR LTRIM(RTRIM(MaGiaoDich)) = '';
GO

/* 2. Bảng trả góp */
IF OBJECT_ID('ChiTietTraGop', 'U') IS NULL
BEGIN
    IF OBJECT_ID('PhieuTraGop', 'U') IS NULL
    CREATE TABLE PhieuTraGop (
        MaTraGop   VARCHAR(20) PRIMARY KEY,
        MaGiaoDich VARCHAR(20),
        MaKH       VARCHAR(20),
        TongTien   DECIMAL(18,2) NOT NULL,
        TraTruoc   DECIMAL(18,2) NOT NULL DEFAULT 0,
        SoKy       INT NOT NULL DEFAULT 1,
        DaTra      DECIMAL(18,2) NOT NULL DEFAULT 0,
        NgayLap    DATETIME NOT NULL DEFAULT GETDATE(),
        TrangThai  NVARCHAR(30) NOT NULL DEFAULT N'Đang trả',
        CONSTRAINT FK_TraGop_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
    );

    CREATE TABLE ChiTietTraGop (
        ID       INT IDENTITY(1,1) PRIMARY KEY,
        MaTraGop VARCHAR(20) NOT NULL,
        NgayTra  DATETIME NOT NULL DEFAULT GETDATE(),
        SoTien   DECIMAL(18,2) NOT NULL CHECK (SoTien > 0),
        GhiChu   NVARCHAR(200),
        CONSTRAINT FK_CTTG_PhieuTraGop FOREIGN KEY (MaTraGop) REFERENCES PhieuTraGop(MaTraGop)
    );
END
ELSE IF OBJECT_ID('PhieuTraGop', 'U') IS NULL
    CREATE TABLE PhieuTraGop (
        MaTraGop   VARCHAR(20) PRIMARY KEY,
        MaGiaoDich VARCHAR(20),
        MaKH       VARCHAR(20),
        TongTien   DECIMAL(18,2) NOT NULL,
        TraTruoc   DECIMAL(18,2) NOT NULL DEFAULT 0,
        SoKy       INT NOT NULL DEFAULT 1,
        DaTra      DECIMAL(18,2) NOT NULL DEFAULT 0,
        NgayLap    DATETIME NOT NULL DEFAULT GETDATE(),
        TrangThai  NVARCHAR(30) NOT NULL DEFAULT N'Đang trả',
        CONSTRAINT FK_TraGop_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
    );
GO

/* 3. Stored procedure + view */
IF OBJECT_ID('sp_TaoPhieuTraGop', 'P') IS NOT NULL DROP PROCEDURE sp_TaoPhieuTraGop;
IF OBJECT_ID('sp_GhiNhanTraGop', 'P') IS NOT NULL DROP PROCEDURE sp_GhiNhanTraGop;
IF OBJECT_ID('vw_TheoDoiTraGop', 'V') IS NOT NULL DROP VIEW vw_TheoDoiTraGop;
GO

CREATE PROCEDURE sp_TaoPhieuTraGop
    @MaGiaoDich VARCHAR(20),
    @MaKH       VARCHAR(20),
    @TongTien   DECIMAL(18,2),
    @TraTruoc   DECIMAL(18,2),
    @SoKy       INT,
    @MaTraGop   VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @SoMoi INT;
    SELECT @SoMoi = ISNULL(MAX(CAST(SUBSTRING(MaTraGop, 3, 10) AS INT)), 0) + 1
    FROM   PhieuTraGop WHERE MaTraGop LIKE 'TG%';
    SET @MaTraGop = 'TG' + RIGHT('000' + CAST(@SoMoi AS VARCHAR(10)), 3);

    INSERT INTO PhieuTraGop (MaTraGop, MaGiaoDich, MaKH, TongTien, TraTruoc, SoKy, DaTra, NgayLap, TrangThai)
    VALUES (@MaTraGop, @MaGiaoDich, @MaKH, @TongTien, @TraTruoc, @SoKy, @TraTruoc, GETDATE(),
            CASE WHEN @TraTruoc >= @TongTien THEN N'Đã tất toán' ELSE N'Đang trả' END);
END;
GO

-- Ghi nhận 1 lần đóng tiền góp (giao tác).
CREATE PROCEDURE sp_GhiNhanTraGop
    @MaTraGop VARCHAR(20),
    @SoTien   DECIMAL(18,2),
    @GhiChu   NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT INTO ChiTietTraGop (MaTraGop, SoTien, GhiChu) VALUES (@MaTraGop, @SoTien, @GhiChu);
        UPDATE PhieuTraGop SET DaTra = DaTra + @SoTien WHERE MaTraGop = @MaTraGop;
        UPDATE PhieuTraGop
        SET    TrangThai = CASE WHEN DaTra >= TongTien THEN N'Đã tất toán' ELSE N'Đang trả' END
        WHERE  MaTraGop = @MaTraGop;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

CREATE VIEW vw_TheoDoiTraGop AS
SELECT  tg.MaTraGop, tg.MaGiaoDich, tg.MaKH,
        kh.HoTen AS TenKhach, kh.SDT,
        tg.TongTien, tg.TraTruoc, tg.SoKy, tg.DaTra,
        (tg.TongTien - tg.DaTra) AS ConLai,
        CASE WHEN tg.SoKy > 0 THEN (tg.TongTien - tg.TraTruoc) / tg.SoKy ELSE 0 END AS SoTienMoiKy,
        tg.NgayLap, tg.TrangThai
FROM    PhieuTraGop tg
LEFT JOIN KhachHang kh ON kh.MaKH = tg.MaKH;
GO

PRINT N'>>> V3: da them MaGiaoDich + chuc nang tra gop.';
GO
