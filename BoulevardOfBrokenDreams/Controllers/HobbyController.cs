using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HobbyController : ControllerBase
    {

        private readonly MumuDbContext _db;


        public HobbyController(MumuDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetHobbyList()
        {
            var hobbyList = await _db.ProjectTypes
                           .Select(x => new GetHobbyListDTO
                           {
                               HobbyId = x.ProjectTypeId,
                               HobbyName = x.ProjectTypeName
                           }).ToListAsync();
            return Ok(hobbyList);
        }

        [HttpPost]
        public async Task<IActionResult> AddHobbies([FromBody] AddHobbyDTO request)
        {
            var user = await _db.Members.FindAsync(request.UserId);
            if (user == null) {
                return NotFound("查無此使用者");
            }
            foreach (var hobbyID in request.HobbyIds)
            {
                var hobby = await _db.ProjectTypes.FindAsync(hobbyID);
                if (hobby == null)
                {
                    continue;
                }
                var userHobby = new Hobby
                {
                    MemberId = user.MemberId,
                    ProjectTypeId = hobby.ProjectTypeId
                };
                //await _db.Hobbies.AddAsync(userHobby);
                _db.Hobbies.Add(userHobby);
            }
            await _db.SaveChangesAsync();

            return Ok("資料儲存成功");
        }
        [HttpGet("{userId}")]

        public async Task<IActionResult> CheckHobbies(int userId)
        {
            
            var check = await _db.Hobbies.AnyAsync(x => x.MemberId ==userId);
            return Ok(check);
        }

        [HttpGet("getMemHobby/{userId}")]
        public async Task<IActionResult> GetMemHobby(int userId)
        {
            var projects = await _db.Hobbies
                .Where(h => h.MemberId == userId)
                .Include(h => h.ProjectType)
                    .ThenInclude(pt => pt.ProjectIdtypes)
                        .ThenInclude(pit => pit.Project)
                .SelectMany(h => h.ProjectType.ProjectIdtypes.Select(pit => pit.Project))
                .Distinct()
                .Select(x => new HomeProjectCardDTO
                {
                    ProjectId = x.ProjectId,
                    ProjectName = x.ProjectName,
                    ProjectGoal = x.ProjectGoal,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    DayLeft = (x.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber),
                    Thumbnail = x.Thumbnail,
                    TotalAmount = ((from orderDetail in _db.OrderDetails
                                    where orderDetail.ProjectId == x.ProjectId
                                    select orderDetail.Price).Sum()) +
                              ((from order in _db.Orders
                                join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                where orderDetail.ProjectId == x.ProjectId
                                select order.Donate ?? 0).Sum()),
                    SponsorCount = (from orderDetail in _db.OrderDetails
                                    where orderDetail.ProjectId == x.ProjectId
                                    select orderDetail.OrderId).Count(),
                })
                .ToListAsync();

            if (projects == null || !projects.Any())
            {
                return NotFound();
            }

            return Ok(projects);

        }




    }
}
