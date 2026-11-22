# API Contract — Web Frontend (Khánh) ↔ Backend ASP.NET Core (Ly)

> **Dự án:** Chatbot Tư vấn Thời trang AI (VELOUR) — Đồ án 1 · Nhóm 9 (75DCTT26)  
> **Backend:** ASP.NET Core (.NET 9) + Ollama (`qwen2.5:3b`)  
> **Frontend:** Razor Pages + Vanilla JS (`wwwroot/js/site.js`)  
> **Base URL (Local):** `http://localhost:5xxx` (xem trong `Properties/launchSettings.json`)  
> **Định dạng dữ liệu:** `application/json; charset=utf-8` (camelCase theo chuẩn mặc định ASP.NET Core)

---

## 1. Gửi tin nhắn tới AI Stylist

### `POST /api/chat`

Gửi câu hỏi của khách hàng tới Backend để chuyển tiếp sang Ollama (`qwen2.5:3b`) và nhận câu trả lời tư vấn phối đồ.

#### Request Body
```json
{
  "message": "Tìm giúp mình váy nhẹ nhàng đi chơi cuối tuần dưới 400k"
}
```
| Trường | Kiểu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `message` | `string` | Có | Nội dung tin nhắn người dùng nhập (không được rỗng/chỉ có khoảng trắng). |

#### Response thành công — `200 OK`
```json
{
  "reply": "Bạn có thể chọn mẫu Đầm Dạ Hội Satin Đỏ Dáng Xòe [IMG:1] phối cùng Áo Poncho Len Dệt Kim Tua Rua [IMG:2] rất nhẹ nhàng và thanh lịch cho buổi dạo phố cuối tuần nhé!"
}
```
| Trường | Kiểu | Mô tả |
| :--- | :--- | :--- |
| `reply` | `string` | Câu trả lời tiếng Việt từ AI Stylist. Có thể chứa thẻ `[IMG:<id>]` để Frontend tự động render thẻ sản phẩm kèm ảnh & nút `+ Giỏ`. |

> **Quy ước thẻ sản phẩm `[IMG:<id>]`:**  
> Frontend (`site.js`) dùng regex `/\[IMG:(\d+)\]/g` trên chuỗi `reply`, tra cứu sản phẩm theo `id` từ danh sách đã tải ở `GET /api/products` để hiển thị card sản phẩm trực tiếp trong khung chat, đồng thời xóa tag `[IMG:id]` khỏi đoạn văn bản hiển thị.

#### Các mã lỗi có thể trả về
| HTTP Status | Trường hợp | JSON mẫu |
| :---: | :--- | :--- |
| `400 Bad Request` | Tin nhắn trống | `{ "error": "Tin nhắn không được để trống." }` |
| `503 Service Unavailable` | Ollama chưa bật / mất kết nối `localhost:11434` | `{ "error": "Không thể kết nối tới Ollama AI (localhost:11434). Hãy kiểm tra Ollama đang chạy và đã tải model qwen2.5:3b." }` |
| `504 Gateway Timeout` | AI xử lý quá thời gian chờ (mặc định 90 giây) | `{ "error": "AI phản hồi quá lâu (hết thời gian chờ). Bạn vui lòng thử gửi lại câu hỏi ngắn gọn hơn nhé!" }` |
| `499 Client Closed Request` | Người dùng đóng tab / hủy request giữa chừng | *(Không có body)* |
| `500 Internal Server Error` | Lỗi không mong muốn phía server | `{ "error": "Đã xảy ra lỗi khi xử lý câu hỏi. Vui lòng thử lại sau giây lát." }` |

---

## 2. Làm mới lịch sử hội thoại

### `POST /api/chat/reset`

Xóa lịch sử chat hiện tại và nạp lại System Prompt gốc của cửa hàng VELOUR.

#### Request Body
*(Không cần body)*

#### Response thành công — `200 OK`
```json
{
  "message": "Lịch sử chat đã được làm mới."
}
```

---

## 3. Lấy danh sách sản phẩm cửa hàng

### `GET /api/products`

Lấy toàn bộ danh sách sản phẩm để Frontend hiển thị lưới sản phẩm, lọc danh mục, tìm kiếm và dựng thẻ sản phẩm `[IMG:id]` trong khung chat.

#### Response thành công — `200 OK`
```json
[
  {
    "id": 1,
    "name": "Đầm Dạ Hội Satin Đỏ Dáng Xòe",
    "category": "Dresses",
    "price": 320000,
    "thumbColor": "#F3E1FF",
    "icon": "👗",
    "imageFileName": "pastel_dress.jpg",
    "description": "Đầm satin đỏ cổ yếm dáng xòe rộng...",
    "isWishlisted": false,
    "formattedPrice": "320,000 ₫",
    "imageUrl": "/images/pastel_dress.jpg"
  }
]
```

| Trường | Kiểu | Mô tả |
| :--- | :--- | :--- |
| `id` | `number` | Mã sản phẩm duy nhất (khớp với `[IMG:<id>]`) |
| `name` | `string` | Tên sản phẩm |
| `category` | `string` | Danh mục (`Dresses`, `Tops`, `Skirts`, `Outerwear`, `Men`, ...) |
| `price` | `number` | Giá gốc (VNĐ) |
| `formattedPrice` | `string` | Giá đã định dạng sẵn (vd: `"320,000 ₫"`) |
| `imageUrl` | `string` | Đường dẫn ảnh tương đối (vd: `"/images/pastel_dress.jpg"`) |
| `description` | `string` | Mô tả chi tiết chất liệu / phong cách |
| `isWishlisted` | `boolean` | Trạng thái yêu thích |

---

## 4. Ghi chú mở rộng giai đoạn nối SQL Server (Tuần 4–5)

Khi tích hợp xong bảng `Products` từ SQL Server, `POST /api/chat` có thể bổ sung thêm mảng `products` (tùy chọn, tương thích ngược hoàn toàn với `reply`):
```json
{
  "reply": "Mình gợi ý cho bạn 2 mẫu phù hợp...",
  "products": [
    {
      "id": 1,
      "name": "Đầm Dạ Hội Satin Đỏ Dáng Xòe",
      "price": 320000,
      "formattedPrice": "320,000 ₫",
      "category": "Dresses",
      "imageUrl": "/images/pastel_dress.jpg"
    }
  ]
}
```
Frontend chỉ cần kiểm tra: nếu có `data.products` thì render trực tiếp, nếu không thì tiếp tục parse `[IMG:id]` như hiện tại.

