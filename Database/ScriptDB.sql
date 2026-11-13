CREATE DATABASE LibraryDB;
GO
USE LibraryDB;
GO

-- 1. Bảng Độc giả
CREATE TABLE DocGia (
    MaDocGia INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    NgayHetHanThe DATETIME NOT NULL
);

-- 2. Bảng Đầu sách
CREATE TABLE DauSach (
    MaSach INT PRIMARY KEY IDENTITY(1,1),
    TenSach NVARCHAR(200) NOT NULL,
    TheLoai NVARCHAR(50),
    SoLuongCon INT CHECK (SoLuongCon >= 0)
);

-- 3. Bảng Phiếu mượn
CREATE TABLE PhieuMuon (
    MaPhieu INT PRIMARY KEY IDENTITY(1,1),
    MaDocGia INT FOREIGN KEY REFERENCES DocGia(MaDocGia),
    NgayMuon DATETIME DEFAULT GETDATE(),
    HanTra DATETIME NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Đang mượn'
);

-- 4. Bảng Chi tiết mượn
CREATE TABLE ChiTietMuon (
    MaPhieu INT FOREIGN KEY REFERENCES PhieuMuon(MaPhieu),
    MaSach INT FOREIGN KEY REFERENCES DauSach(MaSach),
    SoLuong INT CHECK (SoLuong > 0),
    PRIMARY KEY (MaPhieu, MaSach)
);
GO

-- Chèn dữ liệu mẫu
INSERT INTO DocGia (HoTen, Email, NgayHetHanThe) VALUES
(N'Đỗ Quang Toàn', 'toan.dq@gmail.com', DATEADD(MONTH, 6, GETDATE())),   -- Thẻ còn hạn
(N'Nguyễn Trọng Hiếu', 'hieu.nt@gmail.com', DATEADD(YEAR, 1, GETDATE())),   -- Thẻ còn hạn
(N'Thái lọ', 'thai.l@gmail.com', DATEADD(DAY, -10, GETDATE())); -- Thẻ đã hết hạn

INSERT INTO DauSach (TenSach, TheLoai, SoLuongCon) VALUES
(N'Lập trình C# căn bản đến nâng cao', N'Công nghệ thông tin', 10),
(N'Cấu trúc dữ liệu và giải thuật', N'Công nghệ thông tin', 5),
(N'Kinh tế học vĩ mô', N'Kinh tế', 8),
(N'Nhà giả kim', N'Văn học', 12);

INSERT INTO PhieuMuon (MaDocGia, NgayMuon, HanTra, TrangThai) VALUES
(1, DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, 12, GETDATE()), N'Đang mượn'),
(2, DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -5, GETDATE()), N'Quá hạn');

INSERT INTO ChiTietMuon (MaPhieu, MaSach, SoLuong) VALUES
(1, 1, 1),
(1, 3, 2),
(2, 2, 1);
GO