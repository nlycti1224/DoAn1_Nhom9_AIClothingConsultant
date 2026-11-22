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
                    Name = "Đầm Voan Tơ Xếp Bèo Tím Pastel",
                    Category = "Đầm",
                    Price = 420000,
                    ThumbColor = "#F5EEFC",
                    Icon = "",
                    ImageFileName = "prod_1.jpg",
                    Description = "Đầm voan tơ xếp bèo tầng tông tím lavender pastel dịu dàng, đính nơ cổ tinh tế cho nàng Gen Z đi café, chụp ảnh và dự tiệc nhẹ."
                },
                new Product
                {
                    Id = 2,
                    Name = "Áo Cardigan Len Dệt Kim Cúc Ngọc Trai",
                    Category = "Áo khoác",
                    Price = 350000,
                    ThumbColor = "#F7F1FC",
                    Icon = "",
                    ImageFileName = "prod_2.jpg",
                    Description = "Áo cardigan len dệt kim dáng lửng đính cúc ngọc trai phong cách Hàn Quốc, dễ khoác ngoài váy liền hoặc chân váy xếp ly."
                },
                new Product
                {
                    Id = 3,
                    Name = "Đầm Babydoll Tím Pastel Tay Bồng",
                    Category = "Đầm",
                    Price = 360000,
                    ThumbColor = "#F5EEFC",
                    Icon = "",
                    ImageFileName = "prod_3.jpg",
                    Description = "Đầm babydoll ngắn tông tím lavender pastel tay bồng nhẹ nhàng, chất vải mềm mát chuẩn phong cách nàng thơ Gen Z 16–24 tuổi."
                },
                new Product
                {
                    Id = 4,
                    Name = "Chân Váy Ngắn Xếp Ly Cạp Cao",
                    Category = "Chân váy",
                    Price = 245000,
                    ThumbColor = "#F3ECF8",
                    Icon = "",
                    ImageFileName = "prod_4.jpg",
                    Description = "Chân váy ngắn xếp ly cạp cao màu tím than khói có quần bảo hộ bên trong, hack chân siêu dài khi phối cùng baby tee hoặc áo sơ mi."
                },
                new Product
                {
                    Id = 5,
                    Name = "Đầm Nhung Tím Than Dự Tiệc Cổ Tim",
                    Category = "Đầm",
                    Price = 520000,
                    ThumbColor = "#EFE7F7",
                    Icon = "",
                    ImageFileName = "prod_5.jpg",
                    Description = "Đầm nhung tím than cổ tim viền ren tinh tế, chiết eo tôn dáng sang trọng cho nàng 20–26 tuổi đi tiệc tối và hẹn hò."
                },
                new Product
                {
                    Id = 6,
                    Name = "Áo Khoác Cardigan Len Tím Môn Phom Rộng",
                    Category = "Áo khoác",
                    Price = 340000,
                    ThumbColor = "#F6F0FC",
                    Icon = "",
                    ImageFileName = "prod_6.jpg",
                    Description = "Áo khoác cardigan len mềm phom rộng màu tím môn pastel nhẹ nhàng, khoác ngoài áo hai dây hay váy liền đều cực xinh và trẻ trung."
                },
                new Product
                {
                    Id = 7,
                    Name = "Quần Ống Suông Cạp Cao Thanh Lịch",
                    Category = "Quần",
                    Price = 340000,
                    ThumbColor = "#F3ECF8",
                    Icon = "",
                    ImageFileName = "prod_7.jpg",
                    Description = "Quần ống suông cạp cao tông tím khói trầm trẻ trung, chất vải đứng phom ít nhăn giúp kéo dài đôi chân khi đi học hay đi làm."
                },
                new Product
                {
                    Id = 8,
                    Name = "Áo Khoác Tweed Tím Nhạt Dáng Ngắn",
                    Category = "Áo khoác",
                    Price = 580000,
                    ThumbColor = "#F5EEFB",
                    Icon = "",
                    ImageFileName = "prod_8.jpg",
                    Description = "Áo khoác dạ tweed cổ tròn dáng ngắn phối cúc ngọc trai tông tím pastel ngọt ngào chuẩn phong cách tiểu thư Gen Z."
                }
            });
        }

        public IReadOnlyList<Product> GetAllProducts() => _products;

        public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
    }
}
