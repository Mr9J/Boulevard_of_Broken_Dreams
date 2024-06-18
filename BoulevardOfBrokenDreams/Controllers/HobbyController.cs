using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetHobbyList()
        {
            var hobbyList=_db.ProjectTypes
                           .Select(x=> new GetHobbyListDTO 
                           { 
                               HobbyId=x.ProjectTypeId,
                               HobbyName=x.ProjectTypeName
                           }).ToList();
            return Ok(hobbyList);
        }

        [HttpPost]
        public async Task<IActionResult> AddHobbies([FromBody] AddHobbyDTO request)
        {
            var user = await _db.Members.FindAsync(request.UserId);
            if (user == null) {
                return NotFound("查無此使用者");
            }
            foreach(var hobbyID in request.HobbyIds)
            {
                var hobby=await _db.ProjectTypes.FindAsync(hobbyID);
                if (hobby == null)
                {
                    continue; 
                }
                var userHobby = new Hobby
                {
                    MemberId = user.MemberId,
                    ProjectTypeId = hobby.ProjectTypeId
                };
                await _db.Hobbies.AddAsync(userHobby);
            }
            await _db.SaveChangesAsync();

            return Ok("資料儲存成功");
        }

       

    }
}
