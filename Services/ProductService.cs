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
                    Name = "Đầm Xòe Xếp Bèo Pastel",
                    Category = "Váy đầm",
                    Price = 420000,
                    ThumbColor = "#F5F2EE",
                    Icon = "",
                    ImageFileName = "pastel_dress.jpg",
                    Description = "Đầm dáng xòe xếp bèo tầng tông pastel thanh nhã, chất liệu voan tơ mềm nhẹ thoáng mát, phù hợp dạo phố và hẹn hò cuối tuần."
                },
                new Product
                {
                    Id = 2,
                    Name = "Áo Cardigan Len Dệt Kim Hàn Quốc",
                    Category = "Áo khoác",
                    Price = 350000,
                    ThumbColor = "#F4F1EC",
                    Icon = "",
                    ImageFileName = "korean_cardigan.jpg",
                    Description = "Áo cardigan len dệt kim mềm mịn dáng lửng, hàng cúc ngọc trai tinh tế phong cách Hàn Quốc, dễ khoác ngoài váy hoặc áo kiểu."
                },
                new Product
                {
                    Id = 3,
                    Name = "Chân Váy Midi Phối Ren Tầng",
                    Category = "Chân váy",
                    Price = 290000,
                    ThumbColor = "#F3F2EF",
                    Icon = "",
                    ImageFileName = "lace_skirt.jpg",
                    Description = "Chân váy midi cạp cao phối ren nhiều lớp mềm rủ, tôn dáng thanh thoát, dễ kết hợp cùng áo sơ mi hoặc cardigan."
                },
                new Product
                {
                    Id = 4,
                    Name = "Áo Kiểu Tay Bồng Hoa Nhí",
                    Category = "Áo",
                    Price = 260000,
                    ThumbColor = "#F7F4EE",
                    Icon = "",
                    ImageFileName = "puff_blouse.jpg",
                    Description = "Áo kiểu cổ vuông tay bồng họa tiết hoa nhí nữ tính, phom dáng gọn gàng giúp tôn bờ vai và vòng eo."
                },
                new Product
                {
                    Id = 5,
                    Name = "Đầm Nhung Đen Phối Corset Cổ Điển",
                    Category = "Váy đầm",
                    Price = 520000,
                    ThumbColor = "#ECEAE8",
                    Icon = "",
                    ImageFileName = "gothic_dress.jpg",
                    Description = "Đầm nhung đen cao cấp viền ren tinh xảo, thiết kế dây đan corset ôm eo tôn dáng sang trọng cho tiệc tối."
                },
                new Product
                {
                    Id = 6,
                    Name = "Đầm Linen Dáng Suông Tự Nhiên",
                    Category = "Váy đầm",
                    Price = 390000,
                    ThumbColor = "#F0F2ED",
                    Icon = "",
                    ImageFileName = "mori_dress.jpg",
                    Description = "Đầm linen dệt tự nhiên dáng suông nhiều tầng mộc mạc, thấm hút tốt, mang lại cảm giác thoải mái suốt cả ngày."
                },
                new Product
                {
                    Id = 7,
                    Name = "Đầm Lolita Dâu Tây Phối Nơ Ren",
                    Category = "Váy đầm",
                    Price = 680000,
                    ThumbColor = "#F7EFF2",
                    Icon = "",
                    ImageFileName = "lolita_dress.jpg",
                    Description = "Đầm hai dây phong cách Sweet Lolita in họa tiết dâu tây ngọt ngào, điểm nhấn nơ ren thủ công bồng bềnh."
                },
                new Product
                {
                    Id = 8,
                    Name = "Áo Sơ Mi Đũi Dáng Rộng Tối Giản",
                    Category = "Đồ nam",
                    Price = 320000,
                    ThumbColor = "#EFF1F3",
                    Icon = "",
                    ImageFileName = "linen_shirt.jpg",
                    Description = "Áo sơ mi vải đũi dáng rộng tối giản, chất liệu nhẹ thoáng, phù hợp mặc hằng ngày, đi học hoặc dạo phố."
                }
            });
        }

        public IReadOnlyList<Product> GetAllProducts() => _products;

        public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
    }
}
