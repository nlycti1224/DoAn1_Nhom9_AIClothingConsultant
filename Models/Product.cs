namespace AIClothingConsultant.Web.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public int Price { get; set; }
        public string ThumbColor { get; set; } = "#F3E1FF";
        public string Icon { get; set; } = "👗";
        public string ImageFileName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsWishlisted { get; set; } = false;

        public string FormattedPrice => $"{Price:N0} ₫";
        public string ImageUrl => string.IsNullOrEmpty(ImageFileName) ? "" : $"/images/{ImageFileName}";
    }

    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public int Subtotal => Product.Price * Quantity;
    }
}
