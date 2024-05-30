using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private MumuDbContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public  ProjectController(MumuDbContext _db, IHttpContextAccessor httpContextAccessor)
        {
            this.db = _db;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: api/<ProjectController>
        [HttpGet]
        public IEnumerable<ProjectDTO> Get()
        {
            var projects = from p in db.Projects
                           select new ProjectDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectDescription = p.ProjectDescription,
                               ProjectGoal = p.ProjectGoal,
                               StartDate = p.StartDate,
                               EndDate = p.EndDate,
                               MemberId = p.MemberId,
                               GroupId = p.GroupId,
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value+ "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,


                               StatusId = p.StatusId,
                               ProjectAmount = (from orderDetail in db.OrderDetails
                                                where orderDetail.ProjectId == p.ProjectId
                                                select orderDetail.Price).Sum(),
                               Products = (from product in db.Products
                                           where product.ProjectId == p.ProjectId
                                           select new ProductDTO
                                           {
                                               ProductId = product.ProductId,
                                               ProductName = product.ProductName,
                                               OnSalePrice = product.OnSalePrice,
                                               ProductPrice = product.ProductPrice,
                                               ProductDescription = product.ProductDescription,
                                               InitialStock = product.InitialStock,
                                               CurrentStock = product.CurrentStock,
                                               StartDate = product.StartDate,
                                               EndDate = product.EndDate,
                                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + product.Thumbnail,
                                               StatusId = product.StatusId,
                                               OrderBy = product.OrderBy,
                                           }).ToList()
                           };

            return projects;
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProjectController>
        [HttpPost]
        public IActionResult Post([FromBody] ProjectDTO value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid project data.");
            }

            var project = new Project
            {
                ProjectName = value.ProjectName,
                ProjectDescription = value.ProjectDescription,
                ProjectGoal = value.ProjectGoal,
                StartDate = value.StartDate,
                EndDate = value.EndDate,
                MemberId = value.MemberId,
                GroupId = value.GroupId,
                Thumbnail = value.Thumbnail,
                StatusId = value.StatusId,
            };

            db.Projects.Add(project);
            db.SaveChanges();

            return Ok(value);
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProjectDTO value)
        {
            Project? p = db.Projects.FirstOrDefault(x => x.ProjectId == id);
            if (p == null)
            {
                return NotFound("Project not found.");
            }
            p.ProjectName = value.ProjectName;
            p.ProjectDescription = value.ProjectDescription;
            p.StatusId = value.StatusId;
            p.ProjectGoal = value.ProjectGoal;
            p.StartDate = value.StartDate;
            p.EndDate = value.EndDate;
            db.SaveChanges();
            return Ok(value);
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        private static string ConvertImageToBase64(string thumbnail)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var imagePath = Path.Combine(baseDirectory, "mumuThumbnail\\Projects_Products_Thumbnail", thumbnail);
            
            Console.WriteLine($"Image path: {imagePath}");
            if (System.IO.File.Exists(imagePath))
            {
                var imageBytes = System.IO.File.ReadAllBytes(imagePath);
                return Convert.ToBase64String(imageBytes);
            }
            return string.Empty; // 或者你可以返回一個默認的圖片
        }

    }
}
