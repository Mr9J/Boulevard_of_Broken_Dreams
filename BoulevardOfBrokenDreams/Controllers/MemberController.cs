using BoulevardOfBrokenDreams.DataAccess;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MemberController(MumuDbContext _context, IConfiguration _configuration , IHttpContextAccessor httpContextAccessor)
        {
            this._context = _context;
            this._configuration = _configuration;
            this._memberRepository = new MemberRepository(this._context);
            this._emailSender = _emailSender;
            this._httpContextAccessor = _httpContextAccessor;
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
        [HttpGet]
        public IEnumerable<MemberDTO> Get()
        {
            return context.Members
              .Select(m => new MemberDTO
              {
                  MemberId = m.MemberId,
                  Username = m.Username,
                  Nickname = m.Nickname,
                  Thumbnail = "https://" + _httpContextAccessor.HttpContext!.Request.Host.Value + "/resources/mumuThumbnail/members_Thumbnail/" + m.Thumbnail,
                  Email = m.Email,
                  Address = m.Address,
                  MemberIntroduction = m.MemberIntroduction,
                  Phone = m.Phone,
                  RegistrationTime = m.RegistrationTime,
              })
              .ToList();
        }
    }
}
