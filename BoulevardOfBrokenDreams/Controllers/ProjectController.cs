using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectController(MumuDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: api/<ProjectController>
        [HttpGet]
        public IEnumerable<ProjectDTO> Get()
        {
            var projects = from p in _db.Projects
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
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,


                               StatusId = p.StatusId,
                               ProjectAmount = (from orderDetail in _db.OrderDetails
                                                where orderDetail.ProjectId == p.ProjectId
                                                select orderDetail.Price).Sum(),
                               DonationAmount = (from order in _db.Orders
                                                 join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                                 where orderDetail.ProjectId == p.ProjectId
                                                 select order.Donate ?? 0).FirstOrDefault(),
                               TotalAmount = ((from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.Price).Sum()) + ((from order in _db.Orders
                                                                                   join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                                                                   where orderDetail.ProjectId == p.ProjectId
                                                                                   select order.Donate ?? 0).FirstOrDefault()),
            Products = (from product in _db.Products
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
                //Thumbnail = value.Thumbnail,
                StatusId = value.StatusId,
            };

            var pj =  _db.Projects.Add(project);
            _db.SaveChanges();
            int newPjId = pj.Entity.ProjectId;

            if(newPjId > 0 && value.Thumbnail!=null )
            {
                byte[] thumbnailBytes = Convert.FromBase64String(value.Thumbnail);

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "mumuThumbnail", "Projects_Products_Thumbnail", "project-" + newPjId); //設置文件保存的文件夾路徑
                string fileName = "Thumbnail" + ".png"; // 使用唯一文件名
                string filePath = Path.Combine(uploadsFolder, fileName); //構建文件的完整路徑

                Directory.CreateDirectory(uploadsFolder);// 確保目錄存在
                // 將圖片數據寫入文件
                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(thumbnailBytes, 0, thumbnailBytes.Length);
                }

                project.Thumbnail = "project-" + newPjId + "/" + fileName; // 設置圖片路徑
                _db.SaveChanges(); // 再次保存更改
            }
            return Ok(value);
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProjectDTO value)
        {
            Project? p = _db.Projects.FirstOrDefault(x => x.ProjectId == id);
            if (p == null)
            {
                return NotFound("Project not found.");
            }

            if (!string.IsNullOrEmpty(value.Thumbnail))
            {
                byte[] thumbnailBytes = Convert.FromBase64String(value.Thumbnail);

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "mumuThumbnail", "Projects_Products_Thumbnail", "project-" + id); //設置文件保存的文件夾路徑
                string fileName = "Thumbnail" + ".png"; // 使用唯一文件名
                string filePath = Path.Combine(uploadsFolder, fileName); //構建文件的完整路徑

                Directory.CreateDirectory(uploadsFolder);// 確保目錄存在
                // 將圖片數據寫入文件
                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(thumbnailBytes, 0, thumbnailBytes.Length);
                }

                p.Thumbnail = "project-" + id + "/" + fileName; // 設置圖片路徑
            }

            p.ProjectName = value.ProjectName;
            p.ProjectDescription = value.ProjectDescription;
            p.StatusId = value.StatusId;
            p.ProjectGoal = value.ProjectGoal;
            p.StartDate = value.StartDate;
            p.EndDate = value.EndDate;
            _db.SaveChanges();
            return Ok(value);
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return "damedesuyo";
        }

        [HttpGet("member/{memberId}")]
        public IActionResult GetByMemberId(int memberId)
        {
            if (!_db.Projects.Any(p => p.MemberId == memberId))
            {
                return NotFound("No projects found for the given member ID.");
            }

            var projects = from p in _db.Projects.Where(p => p.MemberId == memberId)
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
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,


                               StatusId = p.StatusId,
                               ProjectAmount = (from orderDetail in _db.OrderDetails
                                                where orderDetail.ProjectId == p.ProjectId
                                                select orderDetail.Price).Sum(),
                               Products = (from product in _db.Products
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
            return Ok(projects);
        }

        [HttpGet("count")]
        public List<int> GetProjectCounts()
        {
            List<int> projects = new List<int>();
            int ProjectCount = _db.Projects.Count();
            int activeProjectCount = _db.Projects.Count(p => p.StatusId == 1);
            int inactiveProjectCount = _db.Projects.Count(p => p.StatusId == 0);
            projects.Add(ProjectCount);
            projects.Add(activeProjectCount);
            projects.Add(inactiveProjectCount);
            return projects;
        }

    }
}
