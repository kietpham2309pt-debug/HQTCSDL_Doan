/* ============================================================
   DL_OTO - QUẢN TRỊ NGƯỜI DÙNG & SAO LƯU
   (Phần "Quản trị người dùng" của đồ án)
   ------------------------------------------------------------
   Nội dung:
     1. Tạo LOGIN (cấp server) - cơ chế xác thực SQL Server.
     2. Tạo USER trong DL_OTO + ROLE (nhóm quyền).
     3. CẤP quyền (GRANT) và HỦY quyền (REVOKE / DENY).
     4. Sao lưu CSDL (BACKUP) - thủ công.
     5. Lập lịch sao lưu TỰ ĐỘNG (SQL Server Agent Job - chạy hằng ngày).
   Chạy file này MỘT LẦN trong SSMS với tài khoản sysadmin.
   ============================================================ */

/* ------------------------------------------------------------
   1. TẠO LOGIN (CẤP SERVER) - cơ chế xác thực SQL Server
   ------------------------------------------------------------ */
USE master;
GO

-- Xoá login cũ nếu chạy lại
IF SUSER_ID('ql_user') IS NOT NULL DROP LOGIN ql_user;
IF SUSER_ID('bh_user') IS NOT NULL DROP LOGIN bh_user;
IF SUSER_ID('kt_user') IS NOT NULL DROP LOGIN kt_user;
GO

CREATE LOGIN ql_user WITH PASSWORD = 'QuanLy@123',  DEFAULT_DATABASE = DL_OTO, CHECK_POLICY = OFF;
CREATE LOGIN bh_user WITH PASSWORD = 'BanHang@123', DEFAULT_DATABASE = DL_OTO, CHECK_POLICY = OFF;
CREATE LOGIN kt_user WITH PASSWORD = 'KyThuat@123', DEFAULT_DATABASE = DL_OTO, CHECK_POLICY = OFF;
GO

/* ------------------------------------------------------------
   2. TẠO USER + ROLE (NHÓM QUYỀN) TRONG DL_OTO
   ------------------------------------------------------------ */
USE DL_OTO;
GO

-- Xoá user/role cũ nếu chạy lại (gỡ thành viên trước khi xoá role)
IF DATABASE_PRINCIPAL_ID('ql_user') IS NOT NULL DROP USER ql_user;
IF DATABASE_PRINCIPAL_ID('bh_user') IS NOT NULL DROP USER bh_user;
IF DATABASE_PRINCIPAL_ID('kt_user') IS NOT NULL DROP USER kt_user;
IF DATABASE_PRINCIPAL_ID('role_QuanLy')  IS NOT NULL DROP ROLE role_QuanLy;
IF DATABASE_PRINCIPAL_ID('role_BanHang') IS NOT NULL DROP ROLE role_BanHang;
IF DATABASE_PRINCIPAL_ID('role_KyThuat') IS NOT NULL DROP ROLE role_KyThuat;
GO

-- Tạo user trong database, liên kết với login cùng tên
CREATE USER ql_user FOR LOGIN ql_user;
CREATE USER bh_user FOR LOGIN bh_user;
CREATE USER kt_user FOR LOGIN kt_user;
GO

-- Tạo các nhóm quyền (role)
CREATE ROLE role_QuanLy;    -- Quản lý: toàn quyền dữ liệu
CREATE ROLE role_BanHang;   -- Bán hàng: lập hóa đơn, quản lý khách
CREATE ROLE role_KyThuat;   -- Kỹ thuật/Kho: quản lý xe, dịch vụ, tồn kho
GO

/* ------------------------------------------------------------
   3. CẤP QUYỀN (GRANT) CHO TỪNG ROLE
   ------------------------------------------------------------ */

-- 3.1 role_QuanLy: toàn quyền trên mọi bảng (CONTROL toàn schema dbo)
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO role_QuanLy;
-- Cho phép chạy các thủ tục/hàm
GRANT EXECUTE ON SCHEMA::dbo TO role_QuanLy;
GO

-- 3.2 role_BanHang: xem toàn bộ; thêm/sửa Hóa đơn & Khách hàng; KHÔNG được xóa
GRANT SELECT ON SCHEMA::dbo TO role_BanHang;
GRANT INSERT, UPDATE ON dbo.HoaDon    TO role_BanHang;
GRANT INSERT, UPDATE ON dbo.KhachHang TO role_BanHang;
GRANT EXECUTE ON dbo.sp_LapHoaDon         TO role_BanHang;
GRANT EXECUTE ON dbo.sp_XacNhanDonChoXuLy TO role_BanHang;
GO

-- 3.3 role_KyThuat: xem toàn bộ; cập nhật Xe & Dịch vụ/Phụ tùng (quản lý kho)
GRANT SELECT ON SCHEMA::dbo TO role_KyThuat;
GRANT INSERT, UPDATE, DELETE ON dbo.Xe            TO role_KyThuat;
GRANT INSERT, UPDATE, DELETE ON dbo.DichVuPhuTung TO role_KyThuat;
GRANT EXECUTE ON dbo.sp_CanhBaoTonKhoThap TO role_KyThuat;
GO

-- Gán user vào role tương ứng (cấp quyền cho người dùng qua nhóm)
ALTER ROLE role_QuanLy  ADD MEMBER ql_user;
ALTER ROLE role_BanHang ADD MEMBER bh_user;
ALTER ROLE role_KyThuat ADD MEMBER kt_user;
GO

/* ------------------------------------------------------------
   3b. HỦY QUYỀN (REVOKE) và TỪ CHỐI (DENY) - ví dụ thu hồi quyền
   ------------------------------------------------------------ */
-- Thu hồi quyền XÓA hóa đơn của nhân viên bán hàng (nếu trước đó có cấp)
REVOKE DELETE ON dbo.HoaDon FROM role_BanHang;
GO
-- Chặn hẳn: nhân viên bán hàng KHÔNG được xóa dữ liệu nhân viên
DENY DELETE ON dbo.NhanVien TO role_BanHang;
GO

PRINT N'>>> Da tao login/user/role va cap/huy quyen xong.';
GO

/* ------------------------------------------------------------
   4. SAO LƯU CSDL (BACKUP) - THỦ CÔNG
   ------------------------------------------------------------ */
-- Tạo thư mục chứa file sao lưu (cần quyền sysadmin)
EXEC master.dbo.xp_create_subdir N'C:\Backup\DL_OTO';
GO

-- Sao lưu đầy đủ (full backup). Mỗi lần ghi đè file cũ bằng WITH INIT.
BACKUP DATABASE DL_OTO
TO DISK = N'C:\Backup\DL_OTO\DL_OTO_Full.bak'
WITH INIT, NAME = N'DL_OTO-Full Backup', STATS = 10;
GO

PRINT N'>>> Da sao luu DL_OTO vao C:\Backup\DL_OTO\DL_OTO_Full.bak';
GO

/* ------------------------------------------------------------
   4b. PHỤC HỒI CSDL (RESTORE)
   ------------------------------------------------------------ */
-- Kiểm tra file sao lưu còn dùng được trước khi phục hồi.
RESTORE VERIFYONLY FROM DISK = N'C:\Backup\DL_OTO\DL_OTO_Full.bak';
GO

-- Phục hồi đè lên DL_OTO. Phải đóng mọi kết nối tới DL_OTO trước khi chạy.
USE master;
GO
ALTER DATABASE DL_OTO SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE DL_OTO
FROM DISK = N'C:\Backup\DL_OTO\DL_OTO_Full.bak'
WITH REPLACE, RECOVERY, STATS = 10;
ALTER DATABASE DL_OTO SET MULTI_USER;
GO

PRINT N'>>> Da phuc hoi DL_OTO tu file sao luu.';
GO

/* ------------------------------------------------------------
   5. LẬP LỊCH SAO LƯU TỰ ĐỘNG (SQL SERVER AGENT JOB)
   ------------------------------------------------------------ */
USE msdb;
GO

-- Xoá job cũ nếu chạy lại
IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = N'DL_OTO_AutoBackup')
    EXEC msdb.dbo.sp_delete_job @job_name = N'DL_OTO_AutoBackup';
GO

EXEC msdb.dbo.sp_add_job
     @job_name = N'DL_OTO_AutoBackup',
     @description = N'Tu dong sao luu day du database DL_OTO hang ngay';
GO

EXEC msdb.dbo.sp_add_jobstep
     @job_name   = N'DL_OTO_AutoBackup',
     @step_name  = N'Backup Full',
     @subsystem  = N'TSQL',
     @command    = N'BACKUP DATABASE DL_OTO TO DISK = N''C:\Backup\DL_OTO\DL_OTO_Auto.bak'' WITH INIT, NAME = N''DL_OTO-Auto Daily'', STATS = 10;',
     @database_name = N'master';
GO

-- Lịch: hằng ngày (freq_type=4 = daily), lúc 02:00:00 (020000)
EXEC msdb.dbo.sp_add_schedule
     @schedule_name = N'Lich_HangNgay_0200',
     @freq_type = 4,
     @freq_interval = 1,
     @active_start_time = 020000;
GO

EXEC msdb.dbo.sp_attach_schedule
     @job_name = N'DL_OTO_AutoBackup',
     @schedule_name = N'Lich_HangNgay_0200';
GO

-- Gắn job vào server hiện tại để Agent chạy
EXEC msdb.dbo.sp_add_jobserver
     @job_name = N'DL_OTO_AutoBackup';
GO

PRINT N'>>> Da tao job tu dong sao luu DL_OTO_AutoBackup (chay 02:00 hang ngay).';
GO

