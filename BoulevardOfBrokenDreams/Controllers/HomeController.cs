using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(MumuDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<HomeProjectDTO> Get()
        {
            var projects = from p in _db.Projects.Where(x=>x.StatusId ==1)
                           select new HomeProjectDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectGoal = p.ProjectGoal,
                               StartDate = p.StartDate,
                               EndDate = p.EndDate,
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,
                               TotalAmount = ((from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.Price).Sum()) + ((from order in _db.Orders
                                                                                    join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                                                                    where orderDetail.ProjectId == p.ProjectId
                                                                                    select order.Donate ?? 0).FirstOrDefault()),
                               SponsorCount = (from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.OrderId).Count(),
                           };
            Random rd = new Random();
            var randomProjects = projects.ToList().OrderBy(x => rd.Next()).Take(9);//tolist才能用隨機 不然EF沒辦法把random轉成sql語法
            return randomProjects;
        }

        // GET api/<HomeController>/5

        [HttpGet("POP")]
        public IEnumerable<HomeProjectCardDTO> POP()
        {
            var projects = from p in _db.Projects.Where(x => x.StatusId == 1)
                           select new HomeProjectCardDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectGoal = p.ProjectGoal,
                               DayLeft = (p.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber),
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,
                               TotalAmount = ((from orderDetail in _db.OrderDetails
                                                             where orderDetail.ProjectId == p.ProjectId
                                                             select orderDetail.Price).Sum()) + ((from order in _db.Orders
                                                                                                  join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                                                                                  where orderDetail.ProjectId == p.ProjectId
                                                                                                  select order.Donate ?? 0).FirstOrDefault()),
                               SponsorCount = (from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.OrderId).Count(),
                           };
            return projects.OrderByDescending(x=>x.SponsorCount).Take(7);
        }

        [HttpGet("DayLeft")]
        public IEnumerable<HomeProjectCardDTO> DayLeft()
        {
            var projects = from p in _db.Projects.Where(x => x.StatusId == 1)
                           select new HomeProjectCardDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectGoal = p.ProjectGoal,
                               DayLeft = (p.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber),
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,
                               TotalAmount = ((from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.Price).Sum()) + ((from order in _db.Orders
                                                                                    join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                                                                    where orderDetail.ProjectId == p.ProjectId
                                                                                    select order.Donate ?? 0).FirstOrDefault()),
                               SponsorCount = (from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.OrderId).Count(),
                           };
            return projects.ToList().OrderBy(x => x.DayLeft).Take(7);
        }

        [HttpGet("ProjectType")]
        public IEnumerable<ProjectType> ProjectType()
        {
            return _db.ProjectTypes;
        }

        [HttpGet("Searching")]
        public SearchProjectDTO Searching([FromQuery] string keyword="", [FromQuery] int page=1, [FromQuery] int type = 0)
        {
            var projects = from p in _db.Projects.Where(x =>  x.StatusId == 1 && x.ProjectName.Contains(keyword))
                           join t in _db.ProjectIdtypes
                           on p.ProjectId equals t.ProjectId
                           where (type == 0 ? true : t.ProjectTypeId == type)
                           select new HomeProjectCardDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectGoal = p.ProjectGoal,
                               DayLeft = (p.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber),
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,
                               TotalAmount = ((from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.Price).Sum()) + ((from order in _db.Orders
                                                                                    join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                                                                    where orderDetail.ProjectId == p.ProjectId
                                                                                    select order.Donate ?? 0).FirstOrDefault()),
                               SponsorCount = (from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.OrderId).Count(),
                           };
            SearchProjectDTO searchProjectDTO = new SearchProjectDTO()
            {
                projectData = projects.Skip((page - 1) * 12).Take(12).ToList(),
                totalPage = (int)Math.Ceiling((double)projects.Count() / 12),
            };
            
                       
            return searchProjectDTO;
        }

        // POST api/<HomeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HomeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HomeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
