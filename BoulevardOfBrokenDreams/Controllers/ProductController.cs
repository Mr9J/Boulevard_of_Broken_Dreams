using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(MumuDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public IEnumerable<ProductDTO> Get()
        {
            return _db.Products
               .Select(p => new ProductDTO
               {
                   ProductId = p.ProductId,
                   ProductName = p.ProductName,
                   ProjectId = p.ProjectId,
                   OnSalePrice = p.OnSalePrice,
                   ProductPrice = p.ProductPrice,
                   ProductDescription = p.ProductDescription,
                   InitialStock = p.InitialStock,
                   CurrentStock = p.CurrentStock,
                   StartDate = p.StartDate,
                   EndDate = p.EndDate,
                   Thumbnail = p.Thumbnail,
                   StatusId = p.StatusId,
                   OrderBy = p.OrderBy,
               })
               .ToList();
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductController>
        [HttpPost]
        public IActionResult Post([FromBody] ProductDTO value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid product data.");
            }
            if (!string.IsNullOrEmpty(value.Thumbnail))
            {
                var lastProduct = _db.Products.OrderByDescending(p => p.ProjectId).FirstOrDefault();
                int lastproductId = 0;
                if (lastProduct != null)
                {
                    lastproductId = lastProduct.ProductId;
                }
                else
                {
                    Console.WriteLine("No product found.");
                }
                lastproductId++;

                byte[] thumbnailBytes = Convert.FromBase64String(value.Thumbnail);

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "mumuThumbnail", "Projects_Products_Thumbnail", "project-" + value.ProjectId); //設置文件保存的文件夾路徑
                string fileName = "product-"+ lastproductId + ".png"; // 使用唯一文件名
                string filePath = Path.Combine(uploadsFolder, fileName); //構建文件的完整路徑

                Directory.CreateDirectory(uploadsFolder);// 確保目錄存在
                // 將圖片數據寫入文件
                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(thumbnailBytes, 0, thumbnailBytes.Length);
                }

                value.Thumbnail = "project-" + value.ProjectId + "/" + fileName; // 設置圖片路徑
            }
            var product = new Product
            {
                ProductName = value.ProductName,
                ProjectId = value.ProjectId,
                ProductPrice = value.ProductPrice,
                ProductDescription = value.ProductDescription,
                InitialStock = value.InitialStock,
                CurrentStock = value.CurrentStock,
                StartDate = value.StartDate,
                EndDate = value.EndDate,
                Thumbnail = value.Thumbnail,
                StatusId = value.StatusId,
                OrderBy = value.OrderBy,
            };
            _db.Products.Add(product);
            _db.SaveChanges();
            return Ok(value);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProductDTO value)
        {
            Product? p = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (p == null)
            {
                return NotFound("Product not found.");
            }
            if (!string.IsNullOrEmpty(value.Thumbnail))
            {
                var lastProduct = _db.Products.OrderByDescending(p => p.ProjectId).FirstOrDefault();
                int lastproductId = 0;
                if (lastProduct != null)
                {
                    lastproductId = lastProduct.ProjectId;
                }
                else
                {
                    Console.WriteLine("No product found.");
                }
                lastproductId++;

                byte[] thumbnailBytes = Convert.FromBase64String(value.Thumbnail);

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "mumuThumbnail", "Projects_Products_Thumbnail", "project-" + value.ProjectId); //設置文件保存的文件夾路徑
                string fileName = "product-" + lastproductId + ".png"; // 使用唯一文件名
                string filePath = Path.Combine(uploadsFolder, fileName); //構建文件的完整路徑

                Directory.CreateDirectory(uploadsFolder);// 確保目錄存在
                // 將圖片數據寫入文件
                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(thumbnailBytes, 0, thumbnailBytes.Length);
                }

                value.Thumbnail = "project-" + value.ProjectId + "/" + fileName; // 設置圖片路徑
            }
            var product = new Product
            {
                ProductName = value.ProductName,
                ProductDescription = value.ProductDescription,
                StatusId = value.StatusId,
                ProductPrice = value.ProductPrice,
                InitialStock = value.InitialStock,
                CurrentStock = value.CurrentStock,
                StartDate = value.StartDate,
                EndDate = value.EndDate,
                Thumbnail = value.Thumbnail,
            };
            _db.SaveChanges();
            return Ok(value);
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return "damedesuyo";
        }
    }
}
