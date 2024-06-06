using BoulevardOfBrokenDreams.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyController : ControllerBase
    {
        private readonly MumuDbContext _context;
        public VerifyController(MumuDbContext context)
        {
            _context = context;
        }

        [HttpGet("verify-email/{username}/{email}")]
        public async Task<IActionResult> VerifyEmail(string username, string email)
        {
            Member? member = await _context.Members.FirstOrDefaultAsync(m => m.Username == username);

            if (member == null)
            {
                return BadRequest("無此用戶");  
            }

            //if(member.Verified == "Y")
            //{
            //    return BadRequest("用戶已驗證");
            //}

            member.Verified = "Y";
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
