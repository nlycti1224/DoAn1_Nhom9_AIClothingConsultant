/* =====================================================================
   schema.sql - Q-07 | VELOUR DESIGN - AI Clothing Consultant (Nhóm 9)
   Chạy 1 lần trong SSMS (F5). Chạy lại nhiều lần vẫn được (tự xóa bảng cũ).
   ===================================================================== */

IF DB_ID(N'ClothingDB') IS NULL
    CREATE DATABASE ClothingDB;
GO

USE ClothingDB;
GO

-- Xóa bảng cũ (con trước, cha sau) để chạy lại không lỗi
IF OBJECT_ID(N'dbo.Products', N'U')   IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID(N'dbo.Categories', N'U') IS NOT NULL DROP TABLE dbo.Categories;
GO

-- ---------------------------------------------------------------------
-- Bảng danh mục
-- ---------------------------------------------------------------------
CREATE TABLE dbo.Categories (
    CategoryId   INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_Categories PRIMARY KEY (CategoryId),
    CONSTRAINT UQ_Categories_Name UNIQUE (CategoryName)
);
GO

-- ---------------------------------------------------------------------
-- Bảng sản phẩm (mỗi dòng = 1 sản phẩm với 1 màu + 1 size)
-- ---------------------------------------------------------------------
CREATE TABLE dbo.Products (
    Id          INT IDENTITY(1,1) NOT NULL,
    Name        NVARCHAR(200)     NOT NULL,
    CategoryId  INT               NOT NULL,
    Price       INT               NOT NULL,          -- VND, số nguyên: 250000
    Size        VARCHAR(10)       NOT NULL,          -- S, M, L, XL
    Color       NVARCHAR(50)      NOT NULL,          -- Trắng, Đen, Be...
    Description NVARCHAR(1000)    NULL,
    ImageUrl    NVARCHAR(500)     NULL,
    CONSTRAINT PK_Products PRIMARY KEY (Id),
    CONSTRAINT FK_Products_Categories
        FOREIGN KEY (CategoryId) REFERENCES dbo.Categories (CategoryId),
    CONSTRAINT CK_Products_Price CHECK (Price >= 0),
    CONSTRAINT CK_Products_Size  CHECK (Size IN ('S', 'M', 'L', 'XL'))
);
GO

-- Index phục vụ lọc theo giá (+ index phụ cho lọc theo danh mục)
CREATE INDEX IX_Products_Price      ON dbo.Products (Price);
CREATE INDEX IX_Products_CategoryId ON dbo.Products (CategoryId);
GO

PRINT N'schema.sql chạy xong: ClothingDB đã có Categories + Products.';
GO
