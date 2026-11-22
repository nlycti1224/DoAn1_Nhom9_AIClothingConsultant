# Báo cáo Đo Hiệu năng AI (Baseline Benchmark) — Q-06

> **Dự án:** Chatbot Tư vấn Thời trang AI (VELOUR) — Đồ án 1 · Nhóm 9 (75DCTT26)  
> **Model AI:** `qwen2.5:3b` (Ollama Local · ~1.93 GB)  
> **Ngày đo:** 22/11/2026  
> **Người thực hiện:** Nguyễn Ngọc Ly (phối hợp đối chiếu cùng Nguyễn Khắc Trọng)

---

## 1. Cấu hình máy thử nghiệm (Máy #1 — Laptop Nguyễn Ngọc Ly)

| Thông số | Chi tiết |
| :--- | :--- |
| **CPU** | Intel(R) Core(TM) i5-5350U @ 1.80GHz (2 nhân / 4 luồng) |
| **RAM** | 8 GB (khả dụng 7.9 GB) |
| **GPU** | Intel(R) HD Graphics 6000 (onboard, không có card rời) |
| **Chế độ chạy Ollama** | CPU-only (`http://localhost:11434/api/chat`, `stream: false`) |
| **Model** | `qwen2.5:3b` (1.93 GB) |

---

## 2. Kết quả đo 5 câu hỏi tư vấn thời trang thực tế

| Câu # | Nội dung câu hỏi | Tổng thời gian (s) | Thời gian nạp model (s) | Số token sinh ra | Tốc độ (tok/s) | RAM toàn máy | CPU sau xử lý |
| :---: | :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **1** *(Cold start)* | *Chào shop, shop có những mẫu váy nào dưới 400k?* | **34.20s** | 10.42s | 46 | 5.82 | 6.62 / 7.9 GB (83.8%) | 33% |
| **2** | *Tư vấn giúp mình 1 set đồ nhẹ nhàng đi dạo phố cuối tuần.* | **17.49s** | 0.01s | 73 | 4.68 | 6.67 / 7.9 GB (84.4%) | 51% |
| **3** | *Mình muốn phối đồ với Korean Knit Cardigan thì mặc gì cho hợp?* | **11.24s** | 0.01s | 44 | 4.70 | 6.75 / 7.9 GB (85.4%) | 48% |
| **4** | *Shop có đồ phong cách cổ điển hoặc đi tiệc tối không?* | **12.34s** | 0.00s | 51 | 4.76 | 6.73 / 7.9 GB (85.2%) | 32% |
| **5** | *Mình cao 1m58, thích màu pastel thì nên chọn mẫu nào?* | **8.98s** | 0.00s | 40 | 5.60 | 6.69 / 7.9 GB (84.7%) | 25% |

### Tổng hợp chỉ số Baseline
- **Câu đầu tiên (Cold Start — nạp model vào RAM):** `34.20 giây` (trong đó riêng nạp model mất `10.42 giây`).
- **Trung bình các câu tiếp theo (Warm — câu 2 đến 5):** **`12.51 giây / câu`** (nhanh nhất `8.98s`, chậm nhất `17.49s`).
- **Tốc độ sinh chữ trung bình:** **`5.11 tokens/giây`**.
- **Mức RAM sử dụng toàn hệ thống:** **`6.62 – 6.75 GB / 7.9 GB` (~84–85%)**.
- **Tính ổn định:** 5/5 câu trả lời thành công, không lỗi timeout (timeout backend hiện đặt 90 giây).

---

## 3. Nhận xét & Khuyến nghị cho buổi Demo bảo vệ

1. **Bắt buộc "làm nóng" (warm-up) trước khi demo:**  
   Câu đầu tiên khi Ollama mới khởi động mất ~34 giây do phải nạp model 1.93 GB lên RAM. Trước giờ bảo vệ 2 phút, nhóm cần gửi trước 1 câu test để model nằm sẵn trong RAM → lúc demo trước giảng viên tốc độ sẽ chỉ còn **~9–15 giây/câu**.
2. **Quản lý RAM trên máy 8 GB:**  
   Khi chạy `qwen2.5:3b` trên máy 8 GB RAM, hệ thống đã chạm ~85% RAM. Nếu chạy đồng thời thêm SQL Server + ASP.NET Core + trình duyệt, cần **tắt toàn bộ ứng dụng nền và tab thừa**, hoặc ưu tiên chọn máy **16 GB RAM / có GPU rời** (đối chiếu thêm máy của Trọng bên dưới).
3. **Ghi chú tối ưu Prompt (gửi Phạm Diệu Linh):**  
   Trong quá trình đo, `qwen2.5:3b` đôi khi còn bị lẫn ký tự tiếng Trung cuối câu (`哦`) hoặc sinh lệch cú pháp thẻ ảnh (`[ID 1]`, `[Mã: 1]`, `[MID:1]` thay vì đúng `[IMG:1]`). Cần bổ sung quy tắc cấm tiếng Trung và ví dụ few-shot chuẩn `[IMG:1]` trong System Prompt ở các tuần tiếp theo.

---

## 4. Bảng đối chiếu thêm trên máy Nguyễn Khắc Trọng *(bổ sung nếu dùng máy Trọng demo)*

| Thông số / Kết quả | Máy #2 — Nguyễn Khắc Trọng |
| :--- | :--- |
| **CPU / RAM / GPU** | *(Điền cấu hình máy Trọng)* |
| **Thời gian câu 1 (Cold start)** | *(s)* |
| **Trung bình câu 2–5 (Warm)** | *(s)* |
| **RAM tối đa khi chạy kèm SQL + Web** | *(GB / %)* |
| **Kết luận máy chọn đi demo** | *(Chọn Máy #1 hay Máy #2)* |
