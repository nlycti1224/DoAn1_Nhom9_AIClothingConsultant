using System;
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
                    Name = "Pastel Ruffle Dress",
                    Category = "Pastel Dream",
                    Price = 420000,
                    ThumbColor = "#F3E1FF",
                    Icon = "🌸",
                    ImageFileName = "pastel_dress.jpg",
                    Description = "Váy xòe xếp bèo phong cách pastel nhẹ nhàng, chất vải voan tơ mềm mại."
                },
                new Product
                {
                    Id = 2,
                    Name = "Korean Knit Cardigan",
                    Category = "Korean Soft",
                    Price = 350000,
                    ThumbColor = "#FFEBF5",
                    Icon = "🌼",
                    ImageFileName = "korean_cardigan.jpg",
                    Description = "Áo cardigan len dệt kim dáng lửng, cúc ngọc trai phong cách Hàn Quốc dịu dàng."
                },
                new Product
                {
                    Id = 3,
                    Name = "Lace Midi Skirt",
                    Category = "Korean Soft",
                    Price = 290000,
                    ThumbColor = "#E6F0FF",
                    Icon = "🎀",
                    ImageFileName = "lace_skirt.jpg",
                    Description = "Chân váy midi phối ren nhiều lớp cao cấp, dễ phối với áo thun hoặc áo kiểu."
                },
                new Product
                {
                    Id = 4,
                    Name = "Floral Puff Blouse",
                    Category = "Pastel Dream",
                    Price = 260000,
                    ThumbColor = "#FFF5E6",
                    Icon = "🌺",
                    ImageFileName = "puff_blouse.jpg",
                    Description = "Áo sơ mi croptop tay bồng họa tiết hoa nhí nữ tính, cổ vuông thanh lịch."
                },
                new Product
                {
                    Id = 5,
                    Name = "Gothic Corset Velvet Dress",
                    Category = "Gothic Dark",
                    Price = 520000,
                    ThumbColor = "#DCD7E6",
                    Icon = "🖤",
                    ImageFileName = "gothic_dress.jpg",
                    Description = "Đầm nhung đen viền ren cổ điển phối dây thắt corset tôn dáng kiêu kỳ."
                },
                new Product
                {
                    Id = 6,
                    Name = "Mori Linen Layered Dress",
                    Category = "Mori Forest",
                    Price = 390000,
                    ThumbColor = "#E6F5EB",
                    Icon = "🌿",
                    ImageFileName = "mori_dress.jpg",
                    Description = "Váy linen dáng suông phong cách cô gái rừng xanh mộc mạc, tự nhiên và thoáng mát."
                },
                new Product
                {
                    Id = 7,
                    Name = "Lolita Sweet Ribbon JSK",
                    Category = "Lolita",
                    Price = 680000,
                    ThumbColor = "#FFE1F0",
                    Icon = "🎀",
                    ImageFileName = "lolita_dress.jpg",
                    Description = "Đầm Jumperskirt Sweet Lolita in họa tiết dâu tây và nơ ren bồng bềnh."
                },
                new Product
                {
                    Id = 8,
                    Name = "Oversized Minimal Linen Shirt",
                    Category = "Men",
                    Price = 320000,
                    ThumbColor = "#EBF0F5",
                    Icon = "👔",
                    ImageFileName = "linen_shirt.jpg",
                    Description = "Áo sơ mi đũi dáng rộng unisex basic, chất liệu mát nhẹ phù hợp mùa hè."
                }
            });
        }

        public IReadOnlyList<Product> GetAllProducts() => _products;

        public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
    }
}
