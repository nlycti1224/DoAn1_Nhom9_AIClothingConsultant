using Microsoft.AspNetCore.Mvc.RazorPages;
using AIClothingConsultant.Web.Models;
using AIClothingConsultant.Web.Services;

namespace AIClothingConsultant.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ProductService _productService;

        public IReadOnlyList<Product> Products { get; private set; } = new List<Product>();

        public IndexModel(ProductService productService)
        {
            _productService = productService;
        }

        public void OnGet()
        {
            Products = _productService.GetAllProducts();
        }
    }
}
