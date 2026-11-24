using Microsoft.Data.SqlClient;
using AIClothingConsultant.Web.Models;

namespace AIClothingConsultant.Web.Services;

public class ProductService
{
    private readonly string _connStr;

    public ProductService(IConfiguration config)
    {
        _connStr = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Missing ConnectionStrings:DefaultConnection in appsettings.Development.json");
    }

    // === Method Ly đã dùng sẵn trong Index.cshtml.cs / OllamaService.cs / Program.cs ===
    public List<Product> GetAllProducts()
        => GetAllProductsAsync().GetAwaiter().GetResult();

    public async Task<List<Product>> GetAllProductsAsync()
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

    // (1) Lọc theo danh mục + giá tối đa
    public async Task<List<Product>> FilterByCategoryAsync(string category, int maxPrice)
    {
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
            list.Add(new Product
            {
                Id          = rd.GetInt32(rd.GetOrdinal("Id")),
                Name        = rd.GetString(rd.GetOrdinal("Name")),
                Category    = rd.GetString(rd.GetOrdinal("Category")),
                Price       = rd.GetInt32(rd.GetOrdinal("Price")),                                       // ← int, không phải decimal
                Size        = rd.IsDBNull(rd.GetOrdinal("Size"))  ? "" : rd.GetString(rd.GetOrdinal("Size")),
                Color       = rd.IsDBNull(rd.GetOrdinal("Color")) ? "" : rd.GetString(rd.GetOrdinal("Color")),
                Description = rd.IsDBNull(rd.GetOrdinal("Description")) ? "" : rd.GetString(rd.GetOrdinal("Description")),
                // ImageUrl là computed từ ImageFileName — không map ở đây
                // ImageFileName để mặc định "" → ImageUrl trả về "" (fine cho Q-10 test)
            });
        }
        return list;
    }
}