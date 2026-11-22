using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        private readonly int _numThread;
        private readonly int _numCtx;
        private readonly int _numPredict;
        private readonly List<object> _messages = new();

        public OllamaService(HttpClient httpClient, ProductService productService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _productService = productService;

            string baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434/";
            if (!baseUrl.EndsWith('/')) baseUrl += "/";
            _model = configuration["Ollama:Model"] ?? "qwen2.5:3b";
            int timeoutSeconds = int.TryParse(configuration["Ollama:TimeoutSeconds"], out int sec) && sec > 0 ? sec : 90;
            _numThread = int.TryParse(configuration["Ollama:NumThread"], out int th) && th > 0 ? th : 2;
            _numCtx = int.TryParse(configuration["Ollama:NumCtx"], out int ctx) && ctx >= 512 ? ctx : 2048;
            _numPredict = int.TryParse(configuration["Ollama:NumPredict"], out int pred) && pred >= 64 ? pred : 220;

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
                sb.AppendLine($"- [ID {p.Id}] {p.Name} ({p.Category}) - Giá: {p.FormattedPrice}. {p.Description}");
            }

            string systemPrompt = $"""
            Bạn là Velour AI, stylist tư vấn thời trang của cửa hàng VELOUR.

            {sb}

            QUY TẮC TƯ VẤN:
            - Tuyệt đối CHỈ trả lời bằng tiếng Việt lịch sự, thân thiện. KHÔNG dùng bất kỳ chữ Hán hay ký tự tiếng Trung nào.
            - Trả lời súc tích, gọn gàng (khoảng 3-5 câu), đi thẳng vào gợi ý phối đồ và dịp mặc.
            - Khi nhắc sản phẩm của shop, BẮT BUỘC gắn đúng cú pháp [IMG:ID] (ví dụ: [IMG:1], [IMG:2]; không viết [ID 1] hay [Mã: 1]) ngay cạnh tên sản phẩm.
              Ví dụ: "Bạn có thể phối Đầm Xòe Xếp Bèo Pastel [IMG:1] cùng Áo Cardigan Len Dệt Kim Hàn Quốc [IMG:2] rất thanh lịch!"
            - Không bịa đặt giá hoặc sản phẩm ngoài danh sách trên.
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
                stream = false,
                keep_alive = "10m",
                options = new
                {
                    num_thread = _numThread,
                    num_ctx = _numCtx,
                    num_predict = _numPredict,
                    temperature = 0.6
                }
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

                string rawAnswer = contentEl.GetString() ?? "";
                string answer = SanitizeAiResponse(rawAnswer);

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

        /// <summary>
        /// Post-processing guard for small local models (qwen2.5:3b):
        /// normalizes product image tags to [IMG:id] and strips stray Chinese characters.
        /// </summary>
        private static string SanitizeAiResponse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";

            // 1. Normalize malformed product tags like [ID 1], [Mã: 1], [MID:1] -> [IMG:1]
            string text = Regex.Replace(
                raw,
                @"\[(?:ID|Mã|Ma|MID|IMG|IMAGE)\s*:?\s*(\d+)\]",
                "[IMG:$1]",
                RegexOptions.IgnoreCase);

            // 2. Replace Chinese punctuation with standard Vietnamese punctuation
            text = text.Replace("。", ".").Replace("，", ",").Replace("！", "!").Replace("？", "?");

            // 3. Remove any stray CJK Unified Ideographs (Chinese characters)
            text = Regex.Replace(text, @"[\u4E00-\u9FFF\u3400-\u4DBF]+", "");

            // 4. Tidy extra spaces before punctuation
            text = Regex.Replace(text, @"[ \t]{2,}", " ");
            text = Regex.Replace(text, @" +([.,!?])", "$1");

            return text.Trim();
        }
    }
}
