CREATE DATABASE InventoryDB;
GO
USE InventoryDB;
GO

CREATE TABLE NhaCungCap (
    MaNCC INT IDENTITY(1,1) PRIMARY KEY,
    TenNCC NVARCHAR(150) NOT NULL,
    DienThoai VARCHAR(15) UNIQUE,
    DiaChi NVARCHAR(255)
);

CREATE TABLE VatTu (
    MaVatTu VARCHAR(20) PRIMARY KEY,
    TenVatTu NVARCHAR(150) NOT NULL,
    DVT NVARCHAR(20),
    SoLuongTon DECIMAL(18,2) CHECK (SoLuongTon >= 0)
);

CREATE TABLE PhieuKho (
    MaPhieu INT IDENTITY(1,1) PRIMARY KEY,
    LoaiPhieu NVARCHAR(10) NOT NULL CHECK (LoaiPhieu IN ('NHAP', 'XUAT')),
    NgayLap DATETIME DEFAULT GETDATE(),
    MaNCC INT FOREIGN KEY REFERENCES NhaCungCap(MaNCC) NULL,
    TongTien DECIMAL(18,2)
);

CREATE TABLE ChiTietPhieuKho (
    MaPhieu INT FOREIGN KEY REFERENCES PhieuKho(MaPhieu),
    MaVatTu VARCHAR(20) FOREIGN KEY REFERENCES VatTu(MaVatTu),
    SoLuong DECIMAL(18,2) CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2),
    PRIMARY KEY (MaPhieu, MaVatTu)
);

-- Dữ liệu mẫu
INSERT INTO VatTu VALUES ('VT01', N'Cà phê hạt', 'Kg', 100), ('VT02', N'Đường trắng', 'Kg', 50);
INSERT INTO NhaCungCap (TenNCC, DienThoai) VALUES (N'Công ty Nông Sản A', '0909123456');
