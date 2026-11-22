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
                    Description = "Đầm voan tơ xếp bèo tầng tông tím lavender pastel dịu dàng, đính nơ nhỏ tinh tế, tôn dáng thanh thoát cho buổi dạo phố và tiệc nhẹ."
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
                    Description = "Áo cardigan len dệt kim mềm mịn dáng lửng, hàng cúc ngọc trai phong cách Hàn Quốc, dễ khoác ngoài váy liền hoặc chân váy xếp ly."
                },
                new Product
                {
                    Id = 3,
                    Name = "Chân Váy Midi Xếp Ly Phối Ren",
                    Category = "Chân váy",
                    Price = 290000,
                    ThumbColor = "#F4EEF9",
                    Icon = "",
                    ImageFileName = "prod_3.jpg",
                    Description = "Chân váy midi cạp cao xếp ly mềm rủ phối viền ren tinh xảo, tôn vòng eo và dễ phối cùng áo sơ mi lụa hoặc cardigan."
                },
                new Product
                {
                    Id = 4,
                    Name = "Áo Kiểu Cổ Vuông Tay Bồng Thêu Hoa",
                    Category = "Áo",
                    Price = 260000,
                    ThumbColor = "#FAF4FF",
                    Icon = "",
                    ImageFileName = "prod_4.jpg",
                    Description = "Áo kiểu nữ cổ vuông tay bồng thêu hoa nhí thanh lịch, chất cotton thoáng mát, phù hợp đi làm, đi học và hẹn hò cuối tuần."
                },
                new Product
                {
                    Id = 5,
                    Name = "Đầm Nhung Tím Than Dự Tiệc Cổ Điển",
                    Category = "Đầm",
                    Price = 520000,
                    ThumbColor = "#EFE7F7",
                    Icon = "",
                    ImageFileName = "prod_5.jpg",
                    Description = "Đầm nhung tím than cao cấp cổ tim viền ren nhẹ, phom ôm eo tôn dáng sang trọng và quý phái cho các buổi tiệc tối."
                },
                new Product
                {
                    Id = 6,
                    Name = "Đầm Linen Kem Cổ Yếm Dáng Suông",
                    Category = "Đầm",
                    Price = 390000,
                    ThumbColor = "#F6F1FA",
                    Icon = "",
                    ImageFileName = "prod_6.jpg",
                    Description = "Đầm linen màu kem nhã nhặn thiết kế cổ yếm thanh thoát, chất vải tự nhiên mát nhẹ, lý tưởng cho mùa hè và du lịch."
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
                    Description = "Quần tây ống suông cạp cao tông tím khói trầm sang trọng, chất vải đứng phom ít nhăn giúp kéo dài đôi chân hiệu quả."
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
                    Description = "Áo khoác dạ tweed cổ tròn dáng ngắn phối cúc ngọc trai sang chảnh, tông tím pastel ngọt ngào chuẩn phong cách tiểu thư."
                }
            });
        }

        public IReadOnlyList<Product> GetAllProducts() => _products;

        public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
    }
}
