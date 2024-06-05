using BoulevardOfBrokenDreams.DataAccess;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using BoulevardOfBrokenDreams.Services;
using Microsoft.AspNetCore.Authorization;
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
        private MemberRepository mr;
        public MemberController(MumuDbContext _context, IConfiguration _configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.context = _context;
            this.configuration = _configuration;
            this.mr = new MemberRepository(context);
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDTO user)
        {
            try
            {
                string res = await mr.CreateMember(user);

                if (res == "使用者已存在")
                {
                    return Accepted("使用者已存在");
                }

                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpGet("check-username"),Authorize]
        public async Task<IActionResult> GetCurrentUser(string username)
        {
            Member? member = await mr.GetMemberFull(username);

            if (member != null)
            {
               return Ok(member);
            }
            else
            {
                return BadRequest(member);
            }
        } 

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInDTO user)
        {
            try
            {
                Member? member = await mr.AuthMember(user);

                if (member != null)
                {
                    var token = (new JwtGenerator(configuration)).GenerateJwtToken(user.username, "user");

                    string jwt = "Bearer " + token;

                    CurrentUserDTO cu = new CurrentUserDTO
                    {
                        username = member.Username,
                        email = member.Email ?? string.Empty,
                        jwt = jwt
                    };

                    return Ok(cu);
                }
                else
                {
                    return NotFound("使用者不存在");
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
                  Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/members_Thumbnail/" + m.Thumbnail,
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
