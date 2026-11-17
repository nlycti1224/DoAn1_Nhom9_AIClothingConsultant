# Hướng dẫn chạy
Vào Terminal hoặc Command Prompt(cmd) (sau đây sẽ gọi chung là terminal), gõ 2 lệnh sau và ấn enter:
```
git clone https://github.com/nlycti1224/DoAn1_Nhom9_AIClothingConsultant.git
cd DoAn1_Nhom9_AIClothingConsultant
```
Cài đặt .NET 9 SDK từ link sau (Vì project chạy trên .NET 9, bình thường mọi người sẽ dùng .NET 10):
```
https://dotnet.microsoft.com/en-us/download/dotnet/9.0
```
Chọn theo cái mình khoanh:
<img width="1438" height="677" alt="image" src="https://github.com/user-attachments/assets/5a2f0c8c-35d3-47d8-a100-d1f399629238" />
Gõ tiếp trong terminal và enter để chạy:
```
dotnet restore
dotnet run
```
Mở địa chỉ http://localhost:... hoặc https://localhost:... mà terminal hiện ra.
Để dùng tính năng chatbot AI, máy cần cài Ollama: (truy cập link sau và cài đặt ứng dụng Ollama)
```
https://ollama.com/download
```
Cài đặt model mình đang dùng để chạy chatbot AI (gõ trong terminal):
```
ollama pull qwen2.5:3b
```
Bật dịch vụ Ollama chạy nền:
```
ollama serve
```
Sau đó chạy lại ứng dụng và thử chat.
