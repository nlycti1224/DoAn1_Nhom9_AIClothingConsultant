using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        public OllamaService(
            HttpClient httpClient,
            ProductService productService,
            IConfiguration configuration,
            ILogger<OllamaService> logger)
        {
            _httpClient = httpClient;
            _productService = productService;

            string baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434/";
            if (!baseUrl.EndsWith('/')) baseUrl += "/";
            _model = configuration["Ollama:Model"] ?? "qwen2.5:3b";
            int timeoutSeconds = int.TryParse(configuration["Ollama:TimeoutSeconds"], out int sec) && sec > 0 ? sec : 150;

            // Auto-detect hardware specs (CPU logical threads & total RAM) to self-tune performance
            int logicalCores = Environment.ProcessorCount;
            double totalRamGb = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024.0 * 1024.0 * 1024.0);

            int autoThreads;
            int autoCtx;
            int autoPredict;
            string profile;

            if (logicalCores >= 12 && totalRamGb >= 12)
            {
                // High-spec demo machine (e.g. Core i7 8C/16T, 16GB+ RAM)
                autoThreads = Math.Min(12, logicalCores - 2);
                autoCtx = 3072;
                autoPredict = 280;
                profile = "High-Performance";
            }
            else if (logicalCores >= 6 && totalRamGb >= 10)
            {
                // Mid-range machine (6-8 threads, 12-16GB RAM)
                autoThreads = Math.Max(4, logicalCores - 2);
                autoCtx = 2048;
                autoPredict = 220;
                profile = "Balanced";
            }
            else
            {
                // Low-spec laptop (e.g. 2C/4T, 8GB RAM): keep 1 thread free so Windows/Alt+Tab stays smooth
                autoThreads = Math.Max(2, logicalCores - 1);
                autoCtx = 1536;
                autoPredict = 150;
                profile = "Eco-Smooth";
            }

            // Allow manual override via appsettings if explicitly configured
            _numThread = int.TryParse(configuration["Ollama:NumThread"], out int th) && th > 0 ? th : autoThreads;
            _numCtx = int.TryParse(configuration["Ollama:NumCtx"], out int ctx) && ctx >= 512 ? ctx : autoCtx;
            _numPredict = int.TryParse(configuration["Ollama:NumPredict"], out int pred) && pred >= 64 ? pred : autoPredict;

            logger.LogInformation(
                "[Velour AI Auto-Tune] Profile={Profile} | Hardware: {Cores} threads, {Ram:F1} GB RAM -> num_thread={Threads}, num_ctx={Ctx}, num_predict={Predict}",
                profile, logicalCores, totalRamGb, _numThread, _numCtx, _numPredict);

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
                string attr = string.Join(", ",
                    new[]
                    {
                        string.IsNullOrWhiteSpace(p.Color) ? null : $"Màu: {p.Color}",
                        string.IsNullOrWhiteSpace(p.Size) ? null : $"Size: {p.Size}"
                    }.Where(x => x != null));
                string attrPart = string.IsNullOrEmpty(attr) ? "" : $" ({attr})";
                sb.AppendLine($"- [ID {p.Id}] {p.Name} | {p.Category}{attrPart} | Giá: {p.FormattedPrice}. {p.Description}");
            }

            string systemPrompt = $"""
            Bạn là Stylist tư vấn thời trang trực tuyến của cửa hàng VELOUR, giao tiếp thân thiện, lịch sự, tự nhiên.

            {sb}

            QUY TẮC BẮT BUỘC (system_prompt_v1):
            1. Vai trò & Ngôn ngữ: Chỉ sử dụng 100% tiếng Việt có dấu. Tuyệt đối KHÔNG dùng bất kỳ chữ Hán hay ký tự tiếng Trung nào.
            2. Độ dài: Trả lời ngắn gọn, súc tích trong khoảng 3-5 câu, đi thẳng vào nhu cầu của khách.
            3. Chống bịa dữ liệu (Ưu tiên cao nhất): Chỉ gợi ý đúng sản phẩm có trong danh sách trên. Giữ nguyên chính xác tên, giá, màu, size. Tuyệt đối KHÔNG tự bịa tên sản phẩm, giá tiền, khuyến mãi, tồn kho hay chính sách ngoài dữ liệu.
            4. Định dạng thẻ ảnh: Bắt buộc gắn đúng cú pháp [IMG:ID] ngay cạnh tên mỗi sản phẩm được gợi ý (ví dụ: [IMG:1]; không viết [ID 1] hay [Mã: 1]). Không tự chế ID không tồn tại.
            5. Từ chối lịch sự: Nếu khách hỏi món không có trong danh sách hoặc câu hỏi ngoài lĩnh vực thời trang (điện thoại, giày dép, giải toán...), lịch sự báo rõ VELOUR hiện chưa kinh doanh / chưa hỗ trợ nội dung đó và mời khách tham khảo các mẫu thời trang hiện có.

            VÍ DỤ MẪU (FEW-SHOT):
            - Khách: Shop có áo thun trắng nào dưới 250k không?
              VELOUR: Dạ VELOUR hiện có mẫu Áo thun basic màu Trắng, size S, giá 199.000đ [IMG:1] rất thoáng mát và dễ phối đồ ạ. Bạn đang muốn chọn phong cách đơn giản hay năng động để mình tư vấn thêm nhé?
            - Khách: Shop có bán iPhone 16 hay giày sneaker Nike không?
              VELOUR: Dạ rất tiếc VELOUR chuyên về trang phục thời trang (áo, quần, váy, áo khoác) nên hiện chưa kinh doanh điện thoại hay giày sneaker ạ. Nếu bạn cần tìm đồ đi học, đi làm hoặc đi chơi thì nhắn ngân sách để mình gợi ý nhé!
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

