using AIClothingConsultant.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ProductService>();
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
app.MapPost("/api/chat", async (ChatRequest req, OllamaService ollama) =>
{
    if (string.IsNullOrWhiteSpace(req.Message))
        return Results.BadRequest(new { error = "Tin nhắn không được để trống." });

    try
    {
        string reply = await ollama.SendMessageAsync(req.Message);
        return Results.Ok(new { reply });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = $"Không thể kết nối Ollama AI (localhost:11434): {ex.Message}" }, statusCode: 503);
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

app.Run();

public record ChatRequest(string Message);
