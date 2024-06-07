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
            this.context = _context;
            this.configuration = _configuration;
            this._httpContextAccessor = httpContextAccessor;
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
                  Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/members_Thumbnail/" + m.Thumbnail,
                  Email = m.Email,
                  Address = m.Address,
                  MemberIntroduction = m.MemberIntroduction,
                  Phone = m.Phone,
                  RegistrationTime = m.RegistrationTime,
              })
              .ToList();
        }
        [HttpGet("count")]
        public List<int> GetMemberCounts() //計算被正常與被停權會員數
        {
            List<int> members = new List<int>();
            int activeMemberCount = _context.Members.Count(p => p.StatusId == 7);
            int inactiveMemberCount = _context.Members.Count(p => p.StatusId == 8);
            members.Add(activeMemberCount);
            members.Add(inactiveMemberCount);
            return members;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MemberDTO member)
        {
            Member? m = _context.Members.FirstOrDefault(x => x.MemberId == id);
            if (m == null)
            {
                return NotFound("Member not found.");
            }
            m.MemberId = id;
            m.Username = member.Username;
            m.StatusId = member.StatusId;  
            _context.SaveChanges();
            return Ok(member);
        }
    }
}
