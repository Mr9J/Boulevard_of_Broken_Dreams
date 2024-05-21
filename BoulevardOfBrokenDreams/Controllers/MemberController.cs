using BoulevardOfBrokenDreams.DataAccess;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Props;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly MumuDbContext context;
        private readonly IConfiguration configuration;
        public MemberController(MumuDbContext _context, IConfiguration _configuration)
        {
            this.context = _context;
            this.configuration = _configuration;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpProps user)
        {
            try
            {
                string res = await (new MemberRepository(context)).CreateMember(user);

                if (res == "使用者已存在")
                {
                    return BadRequest(res);
                }

                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInProps user)
        {
            try
            {
                string res = await (new MemberRepository(context)).AuthMember(user);

                if (res == "登入成功")
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            try
            {
                bool res = await (new MemberRepository(context)).GetMember(username);

                if (res)
                {
                    return BadRequest("帳號已被註冊");
                }

                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }
    }
}
