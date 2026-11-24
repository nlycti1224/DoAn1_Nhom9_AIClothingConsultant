using AIClothingConsultant.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ProductService>();
builder.Services.AddHttpClient<OllamaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Minimal API endpoints for Chat & Cart AJAX
app.MapPost("/api/chat", async (ChatRequest req, OllamaService ollama, ILogger<Program> logger, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(req.Message))
        return Results.BadRequest(new { error = "Tin nhắn không được để trống." });

    try
    {
        string reply = await ollama.SendMessageAsync(req.Message, ct);
        return Results.Ok(new { reply });
    }
    catch (OperationCanceledException) when (ct.IsCancellationRequested)
    {
        return Results.StatusCode(499);
    }
    catch (TaskCanceledException ex)
    {
        logger.LogWarning(ex, "Ollama AI request timed out.");
        return Results.Json(
            new { error = "AI phản hồi quá lâu (hết thời gian chờ). Bạn vui lòng thử gửi lại câu hỏi ngắn gọn hơn nhé!" },
            statusCode: StatusCodes.Status504GatewayTimeout);
    }
    catch (HttpRequestException ex)
    {
        logger.LogWarning(ex, "Cannot connect to Ollama AI service.");
        return Results.Json(
            new { error = "Không thể kết nối tới Ollama AI (localhost:11434). Hãy kiểm tra Ollama đang chạy và đã tải model qwen2.5:3b." },
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error while calling Ollama AI.");
        return Results.Json(
            new { error = "Đã xảy ra lỗi khi xử lý câu hỏi. Vui lòng thử lại sau giây lát." },
            statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapPost("/api/chat/reset", (OllamaService ollama) =>
{
    ollama.ResetSystemPrompt();
    return Results.Ok(new { message = "Lịch sử chat đã được làm mới." });
});

app.MapGet("/api/products", (ProductService products) =>
{
    return Results.Ok(products.GetAllProducts());
});

app.MapRazorPages();

// === 3 endpoint test cho Q-10 — giữ để test, xóa trước khi commit nếu muốn ===
app.MapGet("/test/filter", async (ProductService svc) =>
    await svc.FilterByCategoryAsync("Váy", 500000));

app.MapGet("/test/color-size", async (ProductService svc) =>
    await svc.FilterByColorSizeAsync("Be", "M"));

app.MapGet("/test/search", async (ProductService svc) =>
    await svc.SearchAsync("Áo", null, "M", 400000));

app.Run();

public record ChatRequest(string Message);