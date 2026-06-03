/*==============================================================================
  MÔN: HỆ QUẢN TRỊ CƠ SỞ DỮ LIỆU
  CHƯƠNG 4 - PHẦN BACKUP & RESTORE (Sao lưu - Phục hồi)
  Hệ quản trị: Microsoft SQL Server (T-SQL)

  Nội dung:
    - PHẦN A: Trả lời các câu hỏi lý thuyết liên quan backup (Câu 1, 2, 3, 4)
    - PHẦN B: Giải các bài tập backup     (Bài 4.1, 4.2, 4.3, 4.4, 4.5)

  Lưu ý chung:
    - Các câu lệnh SELECT chỉ dùng WHERE, KHÔNG dùng JOIN (theo yêu cầu).
    - Đường dẫn file backup ví dụ: D:\BACKUP\  -> hãy tạo thư mục này trước
      hoặc đổi lại đường dẫn cho phù hợp máy của bạn.
    - Khái niệm khôi phục chuỗi:
        + Bản backup TRƯỚC bản cuối cùng phục hồi -> WITH NORECOVERY
        + Bản backup CUỐI CÙNG phục hồi           -> WITH RECOVERY
==============================================================================*/


/*==============================================================================
  PHẦN A - LÝ THUYẾT (trả lời dạng ghi chú)
==============================================================================*/

/*------------------------------------------------------------------------------
  Câu 1) Phân biệt 3 mô hình phục hồi: Simple, Bulk-Logged và Full
--------------------------------------------------------------------------------
  | Tiêu chí          | Simple                | Bulk-Logged           | Full                  |
  |-------------------|-----------------------|-----------------------|-----------------------|
  | Ghi log           | Log bị cắt (truncate) | Ghi tối thiểu cho các | Ghi đầy đủ MỌI giao   |
  |                   | tự động sau checkpoint | thao tác hàng loạt    | dịch                  |
  | Log backup        | KHÔNG hỗ trợ          | Có hỗ trợ             | Có hỗ trợ             |
  | Phục hồi tới điểm | KHÔNG (point-in-time) | Hạn chế               | CÓ (point-in-time)    |
  | Nguy cơ mất DL    | Mất từ Full/Diff gần  | Có thể mất thao tác   | Gần như 0 nếu có      |
  |                   | nhất                  | bulk khi sự cố        | tail-log backup       |
  | Dùng khi          | DL ít quan trọng, môi | Khi nạp dữ liệu lớn   | DL quan trọng, cần    |
  |                   | trường test           | (bulk insert, index)  | khôi phục chính xác   |

  -> Simple: chỉ Full + Differential.
  -> Full / Bulk-Logged: Full + Differential + Log.
------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------
  Câu 2) Các hình thức sao lưu CSDL & phân biệt
--------------------------------------------------------------------------------
  1. Full backup        : Sao lưu TOÀN BỘ CSDL (dữ liệu + phần log cần thiết).
                          Là nền tảng cho mọi chuỗi phục hồi.
  2. Differential backup: Sao lưu những thay đổi KỂ TỪ lần Full GẦN NHẤT.
                          Phục hồi cần: Full + 1 Diff mới nhất.
  3. Transaction Log    : Sao lưu phần nhật ký giao dịch kể từ log backup trước.
     backup               Cho phép phục hồi tới một thời điểm bất kỳ.
                          (chỉ dùng được với Full / Bulk-Logged recovery)
  4. File / Filegroup   : Sao lưu riêng từng file/nhóm file của CSDL lớn.
     backup
------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------
  Câu 3) Cú pháp sao lưu / phục hồi bằng lệnh
--------------------------------------------------------------------------------
  -- Sao lưu:
     BACKUP DATABASE <ten_db> TO DISK = '...\file.bak'             -- Full
     BACKUP DATABASE <ten_db> TO DISK = '...\file.bak' WITH DIFFERENTIAL  -- Diff
     BACKUP LOG      <ten_db> TO DISK = '...\file.trn'             -- Log

  -- Phục hồi:
     RESTORE DATABASE <ten_db> FROM DISK = '...\full.bak' WITH NORECOVERY
     RESTORE DATABASE <ten_db> FROM DISK = '...\diff.bak' WITH NORECOVERY
     RESTORE LOG      <ten_db> FROM DISK = '...\log.trn'  WITH RECOVERY

  -- Tùy chọn hay dùng: WITH INIT (ghi đè), NOINIT (ghi nối tiếp vào file),
     WITH NORECOVERY (chưa mở DB, chờ phục hồi tiếp),
     WITH RECOVERY  (kết thúc, mở DB cho sử dụng).
------------------------------------------------------------------------------*/

/*------------------------------------------------------------------------------
  Câu 4) Các bước phục hồi với mô hình Simple Recovery
--------------------------------------------------------------------------------
  Vì Simple KHÔNG có log backup, chuỗi phục hồi chỉ gồm Full + Differential:
    B1. Phục hồi bản Full GẦN NHẤT      -> WITH NORECOVERY
    B2. Phục hồi bản Differential mới nhất (nếu có) -> WITH RECOVERY
  -> Dữ liệu phát sinh sau bản Diff cuối cùng sẽ BỊ MẤT (đây là hạn chế của Simple).
------------------------------------------------------------------------------*/



/*==============================================================================
  PHẦN B - BÀI TẬP

  Bài tập 4.1
  - Tạo CSDL DB1, mô hình phục hồi Simple.
  - Bảng KHACH(MAKH, TENKH, DIACHI), thêm 3 dòng.
  - Lịch sao lưu (mỗi lần 1 file riêng):
       Thứ 2 22:00 Full
       Thứ 3 05:00 Diff
       Thứ 4 05:00 Diff
       Thứ 5 05:00 Diff
       Thứ 6 22:00 Full
       Thứ 7 05:00 Diff
       CN    05:00 Diff
  - a) Thực hiện sao lưu theo lịch (giữa 2 lần thêm 1 dòng để có thay đổi).
  - b) Sự cố lúc 21:00 thứ 6 -> phục hồi.
==============================================================================*/

------------------------------- Tạo CSDL & cấu hình -----------------------------
USE master;
GO
IF DB_ID('DB1') IS NOT NULL
BEGIN
    ALTER DATABASE DB1 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DB1;
END
GO
CREATE DATABASE DB1;
GO
ALTER DATABASE DB1 SET RECOVERY SIMPLE;   -- Mô hình phục hồi Simple
GO

USE DB1;
GO
CREATE TABLE KHACH(
    MAKH   VARCHAR(10) PRIMARY KEY,
    TENKH  NVARCHAR(100),
    DIACHI NVARCHAR(200)
);
GO
INSERT INTO KHACH(MAKH, TENKH, DIACHI) VALUES
    ('K01', N'Nguyen Van A', N'Ha Noi'),
    ('K02', N'Tran Thi B',   N'Da Nang'),
    ('K03', N'Le Van C',      N'TP HCM');
GO

------------------------------- a) Sao lưu theo lịch ----------------------------
-- Thứ 2 - 22:00 : FULL (mỗi lần một file riêng -> WITH INIT để file sạch)
BACKUP DATABASE DB1 TO DISK = N'D:\BACKUP\DB1_T2_Full.bak'
    WITH INIT, NAME = N'DB1-Full Thu2';
GO
-- (thêm 1 dòng tạo thay đổi trước Diff thứ 3)
INSERT INTO KHACH VALUES ('K04', N'Khach Thu3', N'Hue');
GO
-- Thứ 3 - 05:00 : DIFFERENTIAL
BACKUP DATABASE DB1 TO DISK = N'D:\BACKUP\DB1_T3_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'DB1-Diff Thu3';
GO

INSERT INTO KHACH VALUES ('K05', N'Khach Thu4', N'Vinh');
GO
-- Thứ 4 - 05:00 : DIFFERENTIAL
BACKUP DATABASE DB1 TO DISK = N'D:\BACKUP\DB1_T4_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'DB1-Diff Thu4';
GO

INSERT INTO KHACH VALUES ('K06', N'Khach Thu5', N'Can Tho');
GO
-- Thứ 5 - 05:00 : DIFFERENTIAL  (đây là bản Diff cuối cùng TRƯỚC sự cố)
BACKUP DATABASE DB1 TO DISK = N'D:\BACKUP\DB1_T5_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'DB1-Diff Thu5';
GO

-- (Các bản dưới đây xảy ra SAU sự cố 21:00 thứ 6 nên ở câu b sẽ KHÔNG dùng,
--  vẫn viết cho đủ lịch trình)
INSERT INTO KHACH VALUES ('K07', N'Khach Thu6', N'Nha Trang');
GO
-- Thứ 6 - 22:00 : FULL
BACKUP DATABASE DB1 TO DISK = N'D:\BACKUP\DB1_T6_Full.bak'
    WITH INIT, NAME = N'DB1-Full Thu6';
GO
INSERT INTO KHACH VALUES ('K08', N'Khach Thu7', N'Quy Nhon');
GO
-- Thứ 7 - 05:00 : DIFFERENTIAL
BACKUP DATABASE DB1 TO DISK = N'D:\BACKUP\DB1_T7_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'DB1-Diff Thu7';
GO
INSERT INTO KHACH VALUES ('K09', N'Khach CN', N'Da Lat');
GO
-- Chủ nhật - 05:00 : DIFFERENTIAL
BACKUP DATABASE DB1 TO DISK = N'D:\BACKUP\DB1_CN_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'DB1-Diff CN';
GO

------------------------------- b) Phục hồi sau sự cố 21:00 thứ 6 ---------------
/*
  Sự cố lúc 21:00 thứ 6 -> XẢY RA TRƯỚC bản Full thứ 6 (22:00).
  Mô hình Simple => chỉ dùng Full + Diff.
  Bản Full gần nhất còn dùng được: Full THỨ 2.
  Bản Diff mới nhất trước sự cố:    Diff THỨ 5.
  => Phục hồi: Full Thứ 2 (NORECOVERY) + Diff Thứ 5 (RECOVERY).
*/
USE master;
GO
RESTORE DATABASE DB1 FROM DISK = N'D:\BACKUP\DB1_T2_Full.bak'
    WITH NORECOVERY, REPLACE;
GO
RESTORE DATABASE DB1 FROM DISK = N'D:\BACKUP\DB1_T5_Diff.bak'
    WITH RECOVERY;
GO
-- Kiểm tra (chỉ dùng WHERE, không JOIN):
USE DB1;
SELECT * FROM KHACH WHERE MAKH IS NOT NULL;   -- mong đợi tới K06 (Thứ 5)
GO



/*==============================================================================
  Bài tập 4.2
  - CSDL QLHV, bảng KHOAHOC(MaKH, TenKH, Thoiluong), mô hình Full Recovery.
  - Kịch bản:
       + Thêm kh01 (Tiếng Anh,3)  -> FULL  : FullQLHV.bak
       + Thêm kh02 (Tiếng Trung,6)-> DIFF  : DiffQLHV.bak
       + Thêm kh03 (Tiếng Hàn,12) -> LOG   : LogQLHV.trn
       + Thêm kh04 (Tiếng Nhật,6) -> LOG   : LogQLHV.trn (cùng file, ghi nối)
  - Sự cố -> viết lệnh phục hồi.
==============================================================================*/
USE master;
GO
IF DB_ID('QLHV') IS NOT NULL
BEGIN
    ALTER DATABASE QLHV SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLHV;
END
GO
CREATE DATABASE QLHV;
GO
ALTER DATABASE QLHV SET RECOVERY FULL;   -- Bắt buộc Full để có Log backup
GO
USE QLHV;
GO
CREATE TABLE KHOAHOC(
    MaKH      VARCHAR(10) PRIMARY KEY,
    TenKH     NVARCHAR(100),
    Thoiluong INT
);
GO

-- 1) Thêm kh01 rồi FULL backup
INSERT INTO KHOAHOC VALUES ('kh01', N'Tieng Anh', 3);
GO
BACKUP DATABASE QLHV TO DISK = N'D:\BACKUP\FullQLHV.bak'
    WITH INIT, NAME = N'QLHV-Full';
GO

-- 2) Thêm kh02 rồi DIFFERENTIAL backup
INSERT INTO KHOAHOC VALUES ('kh02', N'Tieng Trung', 6);
GO
BACKUP DATABASE QLHV TO DISK = N'D:\BACKUP\DiffQLHV.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'QLHV-Diff';
GO

-- 3) Thêm kh03 rồi LOG backup (file LogQLHV.trn)
INSERT INTO KHOAHOC VALUES ('kh03', N'Tieng Han', 12);
GO
BACKUP LOG QLHV TO DISK = N'D:\BACKUP\LogQLHV.trn'
    WITH INIT, NAME = N'QLHV-Log1';
GO

-- 4) Thêm kh04 rồi LOG backup vào CÙNG file LogQLHV.trn (NOINIT = ghi nối)
INSERT INTO KHOAHOC VALUES ('kh04', N'Tieng Nhat', 6);
GO
BACKUP LOG QLHV TO DISK = N'D:\BACKUP\LogQLHV.trn'
    WITH NOINIT, NAME = N'QLHV-Log2';
GO

------------------------------- Phục hồi QLHV -----------------------------------
/*
  Trước khi phục hồi, NÊN sao lưu tail-log (phần log còn lại) nếu DB còn truy cập:
      BACKUP LOG QLHV TO DISK = N'D:\BACKUP\TailQLHV.trn' WITH NORECOVERY;
  Chuỗi phục hồi:
      Full (NORECOVERY) -> Diff (NORECOVERY)
      -> Log lần 1 (NORECOVERY) -> Log lần 2 (RECOVERY)
  Vì 2 log nằm trong CÙNG 1 file nên tham chiếu bằng FILE = 1, FILE = 2.
*/
USE master;
GO
RESTORE DATABASE QLHV FROM DISK = N'D:\BACKUP\FullQLHV.bak'
    WITH NORECOVERY, REPLACE;
GO
RESTORE DATABASE QLHV FROM DISK = N'D:\BACKUP\DiffQLHV.bak'
    WITH NORECOVERY;
GO
RESTORE LOG QLHV FROM DISK = N'D:\BACKUP\LogQLHV.trn'
    WITH FILE = 1, NORECOVERY;     -- bản log lần 1 (kh03)
GO
RESTORE LOG QLHV FROM DISK = N'D:\BACKUP\LogQLHV.trn'
    WITH FILE = 2, RECOVERY;       -- bản log lần 2 (kh04) -> mở DB
GO
USE QLHV;
SELECT * FROM KHOAHOC WHERE MaKH IS NOT NULL;  -- mong đợi đủ kh01..kh04
GO



/*==============================================================================
  Bài tập 4.3
  - Bảng LOPHOC(MALH, TENLH, NGAYBD, NGAYKT, MAKH) trong QLHV, thêm 2 dòng.
  - a) Lịch sao lưu (theo hình):
        t0: Tạo Database
        t1: FULL Backup
        t2: LOG Backup
        t3: LOG Backup
        t4: DIFFERENTIAL Backup
       (Tại mỗi ti (i>=1) tự thêm 1 dòng vào LOPHOC để có thay đổi.)
  - b) Sự cố xảy ra SAU t4 -> viết lệnh phục hồi.
==============================================================================*/
USE QLHV;
GO
IF OBJECT_ID('LOPHOC') IS NOT NULL DROP TABLE LOPHOC;
GO
CREATE TABLE LOPHOC(
    MALH   VARCHAR(10) PRIMARY KEY,
    TENLH  NVARCHAR(100),
    NGAYBD DATE,
    NGAYKT DATE,
    MAKH   VARCHAR(10)
);
GO
INSERT INTO LOPHOC VALUES
    ('LH1', N'Tieng Anh 1', '2024-02-11', '2024-04-11', 'kh01'),
    ('LH2', N'Tieng Anh 2', '2024-03-15', '2024-05-15', 'kh01');
GO

-- t1: FULL
BACKUP DATABASE QLHV TO DISK = N'D:\BACKUP\QLHV_43_Full.bak'
    WITH INIT, NAME = N'QLHV43-Full t1';
GO
-- ti thêm dòng -> t2: LOG
INSERT INTO LOPHOC VALUES ('LH3', N'Tieng Anh 3', '2024-04-01', '2024-06-01', 'kh01');
GO
BACKUP LOG QLHV TO DISK = N'D:\BACKUP\QLHV_43_Log.trn'
    WITH INIT, NAME = N'QLHV43-Log t2';
GO
-- ti thêm dòng -> t3: LOG (ghi nối cùng file)
INSERT INTO LOPHOC VALUES ('LH4', N'Tieng Anh 4', '2024-05-01', '2024-07-01', 'kh01');
GO
BACKUP LOG QLHV TO DISK = N'D:\BACKUP\QLHV_43_Log.trn'
    WITH NOINIT, NAME = N'QLHV43-Log t3';
GO
-- ti thêm dòng -> t4: DIFFERENTIAL
INSERT INTO LOPHOC VALUES ('LH5', N'Tieng Anh 5', '2024-06-01', '2024-08-01', 'kh01');
GO
BACKUP DATABASE QLHV TO DISK = N'D:\BACKUP\QLHV_43_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'QLHV43-Diff t4';
GO

------------------------------- b) Phục hồi sau t4 ------------------------------
/*
  Sự cố SAU t4. Bản Diff t4 đã chứa MỌI thay đổi kể từ Full t1
  => không cần dùng lại 2 log t2, t3.
  Chuỗi phục hồi ngắn nhất: Full t1 (NORECOVERY) + Diff t4 (RECOVERY).
*/
USE master;
GO
RESTORE DATABASE QLHV FROM DISK = N'D:\BACKUP\QLHV_43_Full.bak'
    WITH NORECOVERY, REPLACE;
GO
RESTORE DATABASE QLHV FROM DISK = N'D:\BACKUP\QLHV_43_Diff.bak'
    WITH RECOVERY;
GO
USE QLHV;
SELECT * FROM LOPHOC WHERE MALH IS NOT NULL;   -- mong đợi tới LH5
GO



/*==============================================================================
  Bài tập 4.4
  - CSDL QLBH, bảng HOADON. Lịch sao lưu:
        t1 (17:00 Thứ 7) : FULL
        t2 (17:00 Thứ 2) : LOG
        t3 (17:00 Thứ 4) : DIFFERENTIAL
        t4 (17:00 Thứ 6) : LOG
  - a) Tại mỗi ti thêm 1 dòng vào HOADON rồi backup bằng lệnh.
  - b) Sự cố lúc 08:00 SÁNG THỨ 7 -> viết lệnh phục hồi.
==============================================================================*/
USE master;
GO
IF DB_ID('QLBH') IS NOT NULL
BEGIN
    ALTER DATABASE QLBH SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLBH;
END
GO
CREATE DATABASE QLBH;
GO
ALTER DATABASE QLBH SET RECOVERY FULL;
GO
USE QLBH;
GO
CREATE TABLE HOADON(
    SOHD    VARCHAR(10) PRIMARY KEY,
    NGAYLAP DATE,
    TONGTIEN MONEY
);
GO

-- a) Thực hiện theo lịch (mỗi ti thêm 1 dòng rồi backup):
-- t1 - FULL
INSERT INTO HOADON VALUES ('HD1', '2024-01-06', 100000);
BACKUP DATABASE QLBH TO DISK = N'D:\BACKUP\QLBH_t1_Full.bak'
    WITH INIT, NAME = N'QLBH-Full t1';
GO
-- t2 - LOG
INSERT INTO HOADON VALUES ('HD2', '2024-01-08', 200000);
BACKUP LOG QLBH TO DISK = N'D:\BACKUP\QLBH_t2_Log.trn'
    WITH INIT, NAME = N'QLBH-Log t2';
GO
-- t3 - DIFFERENTIAL
INSERT INTO HOADON VALUES ('HD3', '2024-01-10', 300000);
BACKUP DATABASE QLBH TO DISK = N'D:\BACKUP\QLBH_t3_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'QLBH-Diff t3';
GO
-- t4 - LOG
INSERT INTO HOADON VALUES ('HD4', '2024-01-12', 400000);
BACKUP LOG QLBH TO DISK = N'D:\BACKUP\QLBH_t4_Log.trn'
    WITH INIT, NAME = N'QLBH-Log t4';
GO

------------------------------- b) Phục hồi (sự cố 08:00 Thứ 7) -----------------
/*
  Mốc thời gian: t1(T7) < t2(T2) < t3(T4-Diff) < t4(T6) < SỰ CỐ(08:00 T7).
  Quy tắc: Full + Diff mới nhất + tất cả Log SAU bản Diff đó.
    - Full mới nhất  : t1
    - Diff mới nhất  : t3
    - Log sau Diff   : t4   (log t2 nằm TRƯỚC Diff t3 nên bỏ)
  Chuỗi: Full t1 (NORECOVERY) + Diff t3 (NORECOVERY) + Log t4 (RECOVERY).
*/
USE master;
GO
RESTORE DATABASE QLBH FROM DISK = N'D:\BACKUP\QLBH_t1_Full.bak'
    WITH NORECOVERY, REPLACE;
GO
RESTORE DATABASE QLBH FROM DISK = N'D:\BACKUP\QLBH_t3_Diff.bak'
    WITH NORECOVERY;
GO
RESTORE LOG QLBH FROM DISK = N'D:\BACKUP\QLBH_t4_Log.trn'
    WITH RECOVERY;
GO
USE QLBH;
SELECT * FROM HOADON WHERE SOHD IS NOT NULL;   -- mong đợi đủ HD1..HD4
GO



/*==============================================================================
  Bài tập 4.5
  - CSDL QLDH. Lịch sao lưu:
        t1 (17:00 Thứ 7) : FULL
        t2 (00:00 Thứ 2) : LOG
        t3 (17:00 Thứ 3) : DIFFERENTIAL
        t4 (00:00 Thứ 4) : LOG
  - a) Tại mỗi ti tự thêm/xóa/sửa dữ liệu rồi backup bằng lệnh.
  - b) Sự cố lúc 16:00 (4 giờ chiều) THỨ 4 -> viết lệnh phục hồi.
==============================================================================*/
USE master;
GO
IF DB_ID('QLDH') IS NOT NULL
BEGIN
    ALTER DATABASE QLDH SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLDH;
END
GO
CREATE DATABASE QLDH;
GO
ALTER DATABASE QLDH SET RECOVERY FULL;
GO
USE QLDH;
GO
CREATE TABLE DONHANG(
    MADH    VARCHAR(10) PRIMARY KEY,
    NGAYDAT DATE,
    SOLUONG INT
);
GO

-- a) Thực hiện theo lịch (mỗi ti thêm/sửa/xóa rồi backup):
-- t1 - FULL
INSERT INTO DONHANG VALUES ('DH1', '2024-02-03', 5);
BACKUP DATABASE QLDH TO DISK = N'D:\BACKUP\QLDH_t1_Full.bak'
    WITH INIT, NAME = N'QLDH-Full t1';
GO
-- t2 - LOG (thêm 1 dòng)
INSERT INTO DONHANG VALUES ('DH2', '2024-02-05', 8);
BACKUP LOG QLDH TO DISK = N'D:\BACKUP\QLDH_t2_Log.trn'
    WITH INIT, NAME = N'QLDH-Log t2';
GO
-- t3 - DIFFERENTIAL (sửa 1 dòng)
UPDATE DONHANG SET SOLUONG = 10 WHERE MADH = 'DH1';
BACKUP DATABASE QLDH TO DISK = N'D:\BACKUP\QLDH_t3_Diff.bak'
    WITH DIFFERENTIAL, INIT, NAME = N'QLDH-Diff t3';
GO
-- t4 - LOG (xóa 1 dòng)
DELETE FROM DONHANG WHERE MADH = 'DH2';
BACKUP LOG QLDH TO DISK = N'D:\BACKUP\QLDH_t4_Log.trn'
    WITH INIT, NAME = N'QLDH-Log t4';
GO

------------------------------- b) Phục hồi (sự cố 16:00 Thứ 4) -----------------
/*
  Mốc thời gian:
     t1(17:00 T7) < t2(00:00 T2) < t3(17:00 T3-Diff) < t4(00:00 T4) < SỰ CỐ(16:00 T4)
  Quy tắc: Full + Diff mới nhất + tất cả Log SAU bản Diff đó.
    - Full mới nhất : t1
    - Diff mới nhất : t3
    - Log sau Diff  : t4   (log t2 trước Diff t3 nên bỏ)
  Chuỗi: Full t1 (NORECOVERY) + Diff t3 (NORECOVERY) + Log t4 (RECOVERY).
*/
USE master;
GO
RESTORE DATABASE QLDH FROM DISK = N'D:\BACKUP\QLDH_t1_Full.bak'
    WITH NORECOVERY, REPLACE;
GO
RESTORE DATABASE QLDH FROM DISK = N'D:\BACKUP\QLDH_t3_Diff.bak'
    WITH NORECOVERY;
GO
RESTORE LOG QLDH FROM DISK = N'D:\BACKUP\QLDH_t4_Log.trn'
    WITH RECOVERY;
GO
USE QLDH;
SELECT * FROM DONHANG WHERE MADH IS NOT NULL;  -- mong đợi DH1(SL=10), không còn DH2
GO

/*==============================================================================
  HẾT PHẦN BACKUP - CHƯƠNG 4
  (Các bài 4.6 -> 4.9 thuộc phần Login/User/Phân quyền, không thuộc backup.)
==============================================================================*/
