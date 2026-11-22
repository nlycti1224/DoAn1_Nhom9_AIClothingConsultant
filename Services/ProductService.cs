using System.Collections.Generic;
using System.Linq;
using AIClothingConsultant.Web.Models;

namespace AIClothingConsultant.Web.Services
{
    public class ProductService
    {
        private readonly List<Product> _products = new();

        public ProductService()
        {
            SeedProducts();
        }

        private void SeedProducts()
        {
            _products.AddRange(new[]
            {
                new Product
                {
                    Id = 1,
                    Name = "Đầm Dạ Hội Satin Đỏ Dáng Xòe",
                    Category = "Váy đầm",
                    Price = 420000,
                    ThumbColor = "#F3EAFB",
                    Icon = "",
                    ImageFileName = "pastel_dress.jpg",
                    Description = "Đầm satin đỏ cổ yếm dáng xòe rộng, chân váy bồng bềnh bay bổng khi chuyển động, phù hợp dự tiệc, dạ hội và chụp ảnh kỷ yếu."
                },
                new Product
                {
                    Id = 2,
                    Name = "Áo Poncho Len Dệt Kim Tua Rua",
                    Category = "Áo khoác",
                    Price = 350000,
                    ThumbColor = "#F4F1EC",
                    Icon = "",
                    ImageFileName = "korean_cardigan.jpg",
                    Description = "Áo poncho len dệt kim lưới màu kem cổ chữ V, viền tua rua mềm mại phong cách bohemian, dễ khoác ngoài áo thun hoặc đầm mùa se lạnh."
                },
                new Product
                {
                    Id = 3,
                    Name = "Chân Váy Xếp Ly Đen Đính Nơ Eo",
                    Category = "Chân váy",
                    Price = 290000,
                    ThumbColor = "#F3F2EF",
                    Icon = "",
                    ImageFileName = "lace_skirt.jpg",
                    Description = "Chân váy chữ A xếp ly màu đen cạp cao đính nơ ở eo, dáng xòe nhẹ tôn đôi chân, dễ phối cùng áo sơ mi kẻ sọc hoặc áo thun đi làm, đi học."
                },
                new Product
                {
                    Id = 4,
                    Name = "Áo Sơ Mi Trắng Thêu Hoa Buộc Vạt",
                    Category = "Áo",
                    Price = 260000,
                    ThumbColor = "#F7F4EE",
                    Icon = "",
                    ImageFileName = "puff_blouse.jpg",
                    Description = "Áo sơ mi trắng cộc tay thêu hoa nhí tinh tế, có thể buộc vạt tạo kiểu croptop, chất vải mềm thoáng, hợp phối cùng quần jeans và chân váy."
                },
                new Product
                {
                    Id = 5,
                    Name = "Quần Ống Rộng Kẻ Sọc Đen Trắng",
                    Category = "Quần",
                    Price = 520000,
                    ThumbColor = "#ECEAE8",
                    Icon = "",
                    ImageFileName = "gothic_dress.jpg",
                    Description = "Quần ống suông rộng cạp cao kẻ sọc đen trắng, chất vải đứng form, kéo dài chân, hợp phối cùng áo trễ vai hoặc áo thun trắng cho set đồ cá tính."
                },
                new Product
                {
                    Id = 6,
                    Name = "Đầm Trắng Trễ Vai Bèo Nhún",
                    Category = "Váy đầm",
                    Price = 390000,
                    ThumbColor = "#F0F2ED",
                    Icon = "",
                    ImageFileName = "mori_dress.jpg",
                    Description = "Đầm trắng trễ vai viền bèo nhún thêu ren, eo chun co giãn thoải mái, phong cách nữ tính trong trẻo, hợp đi biển và dạo phố mùa hè."
                },
                new Product
                {
                    Id = 7,
                    Name = "Đầm Hoa Đỏ Cổ Chữ V Kèm Thắt Lưng",
                    Category = "Váy đầm",
                    Price = 680000,
                    ThumbColor = "#F7EFF2",
                    Icon = "",
                    ImageFileName = "lolita_dress.jpg",
                    Description = "Đầm đỏ họa tiết hoa thêu cổ chữ V, tay cánh dơi, đi kèm thắt lưng đan cói nhấn eo, chân váy xòe duyên dáng phong cách retro."
                },
                new Product
                {
                    Id = 8,
                    Name = "Áo Sơ Mi Chambray Xanh Họa Tiết Tim",
                    Category = "Áo",
                    Price = 320000,
                    ThumbColor = "#EFF1F3",
                    Icon = "",
                    ImageFileName = "linen_shirt.jpg",
                    Description = "Áo sơ mi chambray xanh denim nhạt tay lỡ, in họa tiết trái tim nhỏ xinh, có túi ngực, chất cotton mềm mát dễ mặc hằng ngày."
                }
            });
        }

        public IReadOnlyList<Product> GetAllProducts() => _products;

        public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
    }
}

