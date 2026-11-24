CREATE TABLE dbo.Products (
    Id NVARCHAR(10) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NULL,
    Price DECIMAL(12,0) NOT NULL,
    Size NVARCHAR(10) NULL,
    Color NVARCHAR(50) NULL,
    Description NVARCHAR(255) NULL,
    ImageUrl NVARCHAR(500) NULL
);
-- Import products.csv bằng SQL Server Import Flat File Wizard.
-- Chọn encoding UTF-8 và kiểm tra mapping cột trước khi Finish.
