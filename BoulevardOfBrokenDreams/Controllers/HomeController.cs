using Amazon.S3;
using Amazon.S3.Model;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAmazonS3 _s3Client;
        public HomeController(MumuDbContext db, IHttpContextAccessor httpContextAccessor, IAmazonS3 s3Client)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _s3Client = s3Client;
        }
        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<HomeProjectDTO> Get()
        {
            var projects = from p in _db.Projects.Where(x => x.StatusId == 1)
                           select new HomeProjectDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectGoal = p.ProjectGoal,
                               StartDate = p.StartDate,
                               EndDate = p.EndDate,
                               Thumbnail = p.Thumbnail,
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
            var randomProjects = projects.ToList().Where(x => (x.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber) >= 0).OrderBy(x => rd.Next()).Take(9);//tolist才能用隨機 不然EF沒辦法把random轉成sql語法
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
                               StartDate = p.StartDate,
                               EndDate = p.EndDate,
                               DayLeft = (p.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber),
                               Thumbnail = p.Thumbnail,
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
            return projects.ToList().Where(x => x.DayLeft >= 0).OrderByDescending(x => x.SponsorCount).Take(7);
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
                               StartDate = p.StartDate,
                               EndDate = p.EndDate,
                               DayLeft = (p.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber),
                               Thumbnail = p.Thumbnail,
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
            return projects.ToList().Where(x => x.DayLeft >= 0).OrderBy(x => x.DayLeft).Take(7);
        }

        [HttpGet("ProjectType")]
        public IEnumerable<ProjectType> ProjectType()
        {
            return _db.ProjectTypes;
        }

        [HttpGet("Searching")]
        public SearchProjectDTO Searching([FromQuery] string keyword = "", [FromQuery] int page = 1, [FromQuery] int type = 0, [FromQuery] string orderby = "all")
        {
            var projects = from p in _db.Projects.Where(x => x.StatusId == 1 && x.ProjectName.Contains(keyword))
                           join t in _db.ProjectIdtypes
                           on p.ProjectId equals t.ProjectId
                           where (type == 0 ? true : t.ProjectTypeId == type)
                           select new HomeProjectCardDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectGoal = p.ProjectGoal,
                               StartDate = p.StartDate,
                               EndDate = p.EndDate,
                               DayLeft = (p.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber),
                               Thumbnail = p.Thumbnail,
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
            var filteredProjects = projects.AsEnumerable().Where(x => x.DayLeft >= 0);
            switch (orderby)
            {
                case "all":
                    break;
                case "goal":
                    filteredProjects = filteredProjects.OrderByDescending(x => x.TotalAmount);
                    break;
                case "sponsor":
                    filteredProjects = filteredProjects.OrderByDescending(x => x.SponsorCount);
                    break;
                case "enddate":
                    filteredProjects = filteredProjects.OrderBy(x => x.DayLeft);
                    break;
                case "startdate":
                    filteredProjects = filteredProjects.OrderByDescending(x => x.StartDate);
                    break;
                default:
                    break;
            }

            SearchProjectDTO searchProjectDTO = new SearchProjectDTO()
            {
                projectData = filteredProjects.Skip((page - 1) * 12).Take(12).ToList(),
                totalPage = (int)Math.Ceiling((double)filteredProjects.Count() / 12),
            };


            return searchProjectDTO;
        }

        // POST api/<HomeController>
        [HttpPost("CreateProject"), Authorize(Roles = "user")]
        public async Task< IActionResult> CreateProject(CreateProjectDTO value)
        {
            string? jwt = Request.Headers.Authorization;
            int memberId = DecodeJwtToMemberId(jwt);

            var project = new Project
            {
                ProjectName = value.ProjectName,
                ProjectDescription = value.ProjectDescription,
                ProjectGoal = value.ProjectGoal,
                StartDate = value.StartDate,
                EndDate = value.EndDate,
                MemberId = memberId,
                GroupId = 1,
                StatusId = 3,
                ProjectDetails = value.ProjectDetail,
            };
            var pj = _db.Projects.Add(project);
            _db.SaveChanges();
            int newPjId = pj.Entity.ProjectId;
            var type = new ProjectIdtype { ProjectId = newPjId, ProjectTypeId = Convert.ToInt32(value.ProjectTypeId) };
            _db.ProjectIdtypes.Add(type);
            if (newPjId > 0 && value.thumbnail.OpenReadStream() != null)
            {
                Guid g = Guid.NewGuid();
                using (var stream = value.thumbnail.OpenReadStream())
                {
                    string key = $"Test/project-{newPjId}/{g}.png";
                    var request = new PutObjectRequest
                    {
                        BucketName = "mumu",
                        Key = key,
                        InputStream = stream,
                        ContentType = value.thumbnail.ContentType,
                        DisablePayloadSigning = true
                    };

                    var response = await _s3Client.PutObjectAsync(request);



                    project.Thumbnail = $"https://cdn.mumumsit158.com/{key}"; // 設置圖片路徑
                    _db.SaveChanges(); // 再次保存更改
                }


            }
            return Ok(value);
        }

        private int DecodeJwtToMemberId(string? jwt)
        {
            jwt = jwt.Replace("Bearer ", "");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);
            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(id!);
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
