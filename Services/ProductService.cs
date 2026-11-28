using Microsoft.Data.SqlClient;
using AIClothingConsultant.Web.Models;

namespace AIClothingConsultant.Web.Services;

public class ProductService
{
    private readonly string? _connStr;
    private readonly string _contentRoot;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IConfiguration config,
        IWebHostEnvironment env,
        ILogger<ProductService> logger)
    {
        _connStr = config.GetConnectionString("DefaultConnection");
        _contentRoot = env.ContentRootPath;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_connStr))
        {
            _logger.LogWarning("ConnectionStrings:DefaultConnection is not configured. Falling back to 02_Data/products.csv.");
        }
    }

    // === Method Ly đã dùng sẵn trong Index.cshtml.cs / OllamaService.cs / Program.cs ===
    public List<Product> GetAllProducts()
        => GetAllProductsAsync().GetAwaiter().GetResult();

    public async Task<List<Product>> GetAllProductsAsync()
    {
        if (string.IsNullOrWhiteSpace(_connStr))
            return LoadFallbackFromCsv();

        try
        {
            const string sql = @"
                SELECT p.Id, p.Name, c.CategoryName AS Category, p.Price,
                       p.Size, p.Color, p.Description, p.ImageUrl
                FROM dbo.Products p
                JOIN dbo.Categories c ON c.CategoryId = p.CategoryId
                ORDER BY p.Id;";

            await using var conn = new SqlConnection(_connStr);
            await using var cmd  = new SqlCommand(sql, conn);
            return await ReadAllAsync(cmd);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SQL Server unavailable. Falling back to 02_Data/products.csv.");
            return LoadFallbackFromCsv();
        }
    }

    // (1) Lọc theo danh mục + giá tối đa
    public async Task<List<Product>> FilterByCategoryAsync(string category, int maxPrice)
    {
        if (string.IsNullOrWhiteSpace(_connStr))
            return LoadFallbackFromCsv()
                .Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase) && p.Price <= maxPrice)
                .OrderBy(p => p.Price)
                .ToList();

        const string sql = @"
            SELECT p.Id, p.Name, c.CategoryName AS Category, p.Price,
                   p.Size, p.Color, p.Description, p.ImageUrl
            FROM dbo.Products p
            JOIN dbo.Categories c ON c.CategoryId = p.CategoryId
            WHERE c.CategoryName = @Cat AND p.Price <= @MaxPrice
            ORDER BY p.Price;";

        await using var conn = new SqlConnection(_connStr);
        await using var cmd  = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Cat",      category);
        cmd.Parameters.AddWithValue("@MaxPrice", maxPrice);
        return await ReadAllAsync(cmd);
    }

    // (2) Lọc theo màu + size
    public async Task<List<Product>> FilterByColorSizeAsync(string color, string size)
    {
        if (string.IsNullOrWhiteSpace(_connStr))
            return LoadFallbackFromCsv()
                .Where(p => string.Equals(p.Color, color, StringComparison.OrdinalIgnoreCase)
                         && string.Equals(p.Size, size, StringComparison.OrdinalIgnoreCase))
                .ToList();

        const string sql = @"
            SELECT p.Id, p.Name, c.CategoryName AS Category, p.Price,
                   p.Size, p.Color, p.Description, p.ImageUrl
            FROM dbo.Products p
            JOIN dbo.Categories c ON c.CategoryId = p.CategoryId
            WHERE p.Color = @Color AND p.Size = @Size;";

        await using var conn = new SqlConnection(_connStr);
        await using var cmd  = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Color", color);
        cmd.Parameters.AddWithValue("@Size",  size);
        return await ReadAllAsync(cmd);
    }

    // (3) Kết hợp nhiều tiêu chí, NULL = bỏ qua
    public async Task<List<Product>> SearchAsync(
        string? category, string? color, string? size, int? maxPrice)
    {
        if (string.IsNullOrWhiteSpace(_connStr))
            return LoadFallbackFromCsv()
                .Where(p => (string.IsNullOrWhiteSpace(category) || string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
                         && (string.IsNullOrWhiteSpace(color)    || string.Equals(p.Color, color, StringComparison.OrdinalIgnoreCase))
                         && (string.IsNullOrWhiteSpace(size)     || string.Equals(p.Size, size, StringComparison.OrdinalIgnoreCase))
                         && (!maxPrice.HasValue                  || p.Price <= maxPrice.Value))
                .OrderBy(p => p.Price)
                .ToList();

        const string sql = @"
            SELECT p.Id, p.Name, c.CategoryName AS Category, p.Price,
                   p.Size, p.Color, p.Description, p.ImageUrl
            FROM dbo.Products p
            JOIN dbo.Categories c ON c.CategoryId = p.CategoryId
            WHERE (@Cat      IS NULL OR c.CategoryName = @Cat)
              AND (@Color    IS NULL OR p.Color        = @Color)
              AND (@Size     IS NULL OR p.Size         = @Size)
              AND (@MaxPrice IS NULL OR p.Price       <= @MaxPrice)
            ORDER BY p.Price ASC;";

        await using var conn = new SqlConnection(_connStr);
        await using var cmd  = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Cat",      System.Data.SqlDbType.NVarChar, 100).Value = (object?)category ?? DBNull.Value;
        cmd.Parameters.Add("@Color",    System.Data.SqlDbType.NVarChar, 50 ).Value = (object?)color    ?? DBNull.Value;
        cmd.Parameters.Add("@Size",     System.Data.SqlDbType.NVarChar, 10 ).Value = (object?)size     ?? DBNull.Value;
        cmd.Parameters.Add("@MaxPrice", System.Data.SqlDbType.Int          ).Value = (object?)maxPrice ?? DBNull.Value;
        return await ReadAllAsync(cmd);
    }

    // === Helper map SqlDataReader → Product (khớp model thật của Ly) ===
    private static async Task<List<Product>> ReadAllAsync(SqlCommand cmd)
    {
        var list = new List<Product>();
        await cmd.Connection!.OpenAsync();
        await using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            int id = rd.GetInt32(rd.GetOrdinal("Id"));
            string rawImg = rd.IsDBNull(rd.GetOrdinal("ImageUrl")) ? "" : rd.GetString(rd.GetOrdinal("ImageUrl"));
            list.Add(new Product
            {
                Id            = id,
                Name          = rd.GetString(rd.GetOrdinal("Name")),
                Category      = rd.GetString(rd.GetOrdinal("Category")),
                Price         = rd.GetInt32(rd.GetOrdinal("Price")),
                Size          = rd.IsDBNull(rd.GetOrdinal("Size"))  ? "" : rd.GetString(rd.GetOrdinal("Size")),
                Color         = rd.IsDBNull(rd.GetOrdinal("Color")) ? "" : rd.GetString(rd.GetOrdinal("Color")),
                Description   = rd.IsDBNull(rd.GetOrdinal("Description")) ? "" : rd.GetString(rd.GetOrdinal("Description")),
                ImageFileName = ResolveImageFileName(rawImg, id),
            });
        }
        return list;
    }

    private static string ResolveImageFileName(string rawImg, int id)
    {
        if (!string.IsNullOrWhiteSpace(rawImg)
            && !rawImg.Contains("example.com", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFileName(rawImg);
        }
        return $"prod_{((Math.Max(id, 1) - 1) % 8) + 1}.jpg";
    }

    private List<Product> LoadFallbackFromCsv()
    {
        var list = new List<Product>();
        string csvPath = Path.Combine(_contentRoot, "02_Data", "products.csv");
        if (!File.Exists(csvPath))
            return list;

        var lines = File.ReadAllLines(csvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = ParseCsvLine(lines[i]);
            if (cols.Count < 8) continue;
            int id = i;
            if (!int.TryParse(cols[3], out int price)) price = 0;
            list.Add(new Product
            {
                Id            = id,
                Name          = cols[1],
                Category      = cols[2],
                Price         = price,
                Size          = cols[4],
                Color         = cols[5],
                Description   = cols[6],
                ImageFileName = ResolveImageFileName(cols[7], id),
            });
        }
        return list;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();
        foreach (char c in line)
        {
            if (c == '"') { inQuotes = !inQuotes; continue; }
            if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        result.Add(current.ToString());
        return result;
    }
}