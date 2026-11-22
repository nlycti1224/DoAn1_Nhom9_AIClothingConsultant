using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AIClothingConsultant.Web.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly ProductService _productService;
        private readonly string _model;
        private readonly List<object> _messages = new();

        public OllamaService(HttpClient httpClient, ProductService productService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _productService = productService;

            string baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434/";
            if (!baseUrl.EndsWith('/')) baseUrl += "/";
            _model = configuration["Ollama:Model"] ?? "qwen2.5:3b";
            int timeoutSeconds = int.TryParse(configuration["Ollama:TimeoutSeconds"], out int sec) && sec > 0 ? sec : 90;

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

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

        public async Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            _messages.Add(new
            {
                role = "user",
                content = message
            });

            var request = new
            {
                model = _model,
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
                    await _httpClient.PostAsync("api/chat", content, cancellationToken);

                response.EnsureSuccessStatusCode();

                string result =
                    await response.Content.ReadAsStringAsync(cancellationToken);

                using JsonDocument document =
                    JsonDocument.Parse(result);

                if (!document.RootElement.TryGetProperty("message", out var msgEl) ||
                    !msgEl.TryGetProperty("content", out var contentEl))
                {
                    throw new InvalidOperationException("Phản hồi từ Ollama không đúng định dạng mong đợi.");
                }

                string answer = contentEl.GetString() ?? "";

                _messages.Add(new
                {
                    role = "assistant",
                    content = answer
                });

                return answer;
            }
            catch
            {
                if (_messages.Count > 0)
                    _messages.RemoveAt(_messages.Count - 1);
                throw;
            }
        }
    }
}
