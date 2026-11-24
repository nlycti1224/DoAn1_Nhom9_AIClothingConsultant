# Nguồn dữ liệu – Bộ dữ liệu sản phẩm thời trang VELOUR

## 1. Thông tin chung
- **Tên bộ dữ liệu:** Bộ dữ liệu sản phẩm thời trang VELOUR
- **Tên tệp:** `products.csv`
- **Định dạng:** CSV (UTF-8 có BOM để hỗ trợ tiếng Việt khi mở bằng Excel)
- **Ngày tạo:** 24/11/2026
- **Người chuẩn bị:** Nguyễn Khắc Trọng

## 2. Nguồn gốc dữ liệu
- **Nguồn:** Dữ liệu mẫu do người thực hiện tự tạo và tổng hợp phục vụ học tập.
- **Đường dẫn nguồn bên ngoài:** Không có. Đây không phải dữ liệu tải từ website hoặc dataset công khai.
- **Cách tạo:** Tự xây dựng 60 bản ghi sản phẩm thời trang minh họa, gồm mã sản phẩm, tên, danh mục, giá, kích cỡ, màu sắc, mô tả và đường dẫn ảnh minh họa.
- **Giấy phép/điều kiện sử dụng:** Dữ liệu mẫu tự tạo, dùng cho mục đích học tập và thực hành; các URL ảnh dùng miền `example.com` chỉ là địa chỉ minh họa, không phải ảnh sản phẩm thật.

## 3. Mục đích sử dụng
Dùng để thực hành kiểm tra, chuẩn hóa dữ liệu CSV và thử nghiệm thiết kế bảng, nhập dữ liệu vào SQL Server trong phạm vi học tập.

## 4. Cấu trúc dữ liệu

| STT | Tên cột | Kiểu dữ liệu dự kiến | Ý nghĩa |
|---:|---|---|---|
| 1 | `Id` | NVARCHAR(10) | Mã sản phẩm, duy nhất |
| 2 | `Name` | NVARCHAR(100) | Tên sản phẩm |
| 3 | `Category` | NVARCHAR(50) | Danh mục sản phẩm |
| 4 | `Price` | DECIMAL(12,0) | Giá sản phẩm (VNĐ) |
| 5 | `Size` | NVARCHAR(10) | Kích cỡ |
| 6 | `Color` | NVARCHAR(50) | Màu sắc |
| 7 | `Description` | NVARCHAR(255) | Mô tả sản phẩm |
| 8 | `ImageUrl` | NVARCHAR(500) | Đường dẫn ảnh minh họa |

## 5. Kiểm tra và chuẩn hóa
- Tệp có một dòng tiêu đề và 60 dòng dữ liệu.
- Các dòng có đủ 8 trường, theo đúng thứ tự cột.
- Mã sản phẩm không trùng lặp trong bộ dữ liệu mẫu.
- Giá được lưu dưới dạng số nguyên, không kèm ký hiệu tiền tệ hoặc dấu phân cách hàng nghìn.
- Văn bản được lưu bằng UTF-8 có BOM để hạn chế lỗi tiếng Việt khi mở bằng Excel.
- Tệp được xuất bằng thư viện CSV, vì vậy trường văn bản có ký tự phân tách sẽ được đặt dấu ngoặc kép theo quy tắc CSV.

## 6. Kiểm tra trước khi nạp SQL Server
- [x] Có dòng tiêu đề và đúng 8 cột.
- [x] Mỗi bản ghi có đủ số trường.
- [x] Mã sản phẩm không bị trùng.
- [x] Giá sản phẩm là dữ liệu số.
- [x] Đã lưu tệp ở định dạng UTF-8 có BOM.
- [ ] Nạp thử vào SQL Server: chưa thực hiện; cần chạy thử trên máy và kiểm tra kết quả.

## 7. Nhật ký xử lý
| Ngày | Công việc | Kết quả |
|---|---|---|
| 24/11/2026 | Tạo dữ liệu mẫu và xuất CSV | Hoàn thành 60 bản ghi |
| 24/11/2026 | Kiểm tra tiêu đề, số cột, mã và giá | Đã kiểm tra bằng dữ liệu mẫu |

## 8. Trạng thái
- Nguồn dữ liệu: Đã ghi rõ là dữ liệu mẫu tự tạo.
- Cấu trúc cột: Đã xác định 8 cột.
- Kiểm tra định dạng CSV: Đã kiểm tra cấu trúc cơ bản.
- Nạp thử SQL Server: Chưa thực hiện, không ghi nhận là đã thành công.

