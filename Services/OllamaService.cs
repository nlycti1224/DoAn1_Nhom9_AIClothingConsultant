using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIClothingConsultant.Web.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly ProductService _productService;
        private readonly List<object> _messages = new();

        public OllamaService(HttpClient httpClient, ProductService productService)
        {
            _httpClient = httpClient;
            _productService = productService;
            _httpClient.BaseAddress = new Uri("http://localhost:11434/");
            _httpClient.Timeout = TimeSpan.FromMinutes(3);

            ResetSystemPrompt();
        }

        public void ResetSystemPrompt()
        {
            _messages.Clear();

            var storeProducts = _productService.GetAllProducts();
            var sb = new StringBuilder();
            sb.AppendLine("DANH SÁCH SẢN PHẨM HIỆN CÓ CỦA CỬA HÀNG VELOUR:");
            foreach (var p in storeProducts)
            {
                sb.AppendLine($"- [ID {p.Id}] {p.Name} ({p.Category}) - Giá: {p.FormattedPrice}. Chi tiết: {p.Description}. Ảnh: {p.ImageUrl}");
            }

            string systemPrompt = $"""
            Bạn là Velour AI, stylist kiêm trợ lý tư vấn thời trang thông minh của cửa hàng thời trang VELOUR.

            {sb}

            QUY TẮC TƯ VẤN:
            - Luôn trả lời bằng tiếng Việt lịch sự, thân thiện, duyên dáng và có chuyên môn về thời trang.
            - Khi tư vấn và gợi ý sản phẩm của shop, hãy dùng cú pháp [IMG:ID] (ví dụ: [IMG:1], [IMG:2]) ngay cạnh tên sản phẩm để hệ thống web tự động hiển thị thẻ card kèm ảnh thật của sản phẩm đó trong khung chat.
              Ví dụ: "Bạn có thể phối mẫu Pastel Ruffle Dress [IMG:1] cùng Korean Knit Cardigan [IMG:2] rất thanh lịch và ngọt ngào!"
            - Gợi ý phối đồ (mix & match): màu sắc, phụ kiện, chất liệu, dịp đi chơi/tiệc/dạo phố.
            - Hướng dẫn người dùng bấm vào sản phẩm hoặc thêm vào giỏ hàng ngay trên web.
            - Không bịa đặt giá hoặc sản phẩm sai lệch với danh sách trên.
            """;

            _messages.Add(new
            {
                role = "system",
                content = systemPrompt
            });
        }

        public async Task<string> SendMessageAsync(string message)
        {
            _messages.Add(new
            {
                role = "user",
                content = message
            });

            var request = new
            {
                model = "qwen2.5:3b",
                messages = _messages,
                stream = false
            };

            string json = JsonSerializer.Serialize(request);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                using HttpResponseMessage response =
                    await _httpClient.PostAsync("api/chat", content);

                response.EnsureSuccessStatusCode();

                string result =
                    await response.Content.ReadAsStringAsync();

                using JsonDocument document =
                    JsonDocument.Parse(result);

                string answer = document.RootElement
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";

                _messages.Add(new
                {
                    role = "assistant",
                    content = answer
                });

                return answer;
            }
            catch
            {
                _messages.RemoveAt(_messages.Count - 1);
                throw;
            }
        }
    }
}
