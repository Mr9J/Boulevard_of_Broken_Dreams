using BoulevardOfBrokenDreams.Models;
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
            var hobbyList=_db.ProjectTypes.ToList();
            return Ok(hobbyList);
        }
 
    }
}
