using BoulevardOfBrokenDreams.DataAccess;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using BoulevardOfBrokenDreams.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly MumuDbContext context;
        private readonly IConfiguration configuration;
        private MemberRepository mr;
        public MemberController(MumuDbContext _context, IConfiguration _configuration)
        {
            this.context = _context;
            this.configuration = _configuration;
            this.mr = new MemberRepository(context);
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

        [HttpGet("check-username"), Authorize]
        public async Task<IActionResult> GetCurrentUser(string username)
        {
            Member? member = await mr.GetMember(username);

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

                    return Ok(jwt);
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

        [HttpPost("get-current-user"), Authorize]
        public async Task<IActionResult> getCurrentUser(string jwt)
        {
            try
            {
                if (jwt == null || jwt == "")
                {
                    return NotFound("請輸入 JWT");
                }

                jwt = jwt.Replace("Bearer ", "");

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

                string? username = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;

                if (username == null)
                {
                    return NotFound("使用者不存在");
                }

                Member? member = await mr.GetMember(username);

                if (member == null)
                {
                    return NotFound("使用者不存在");
                }

                GetCurrentUserDTO currentUser = new GetCurrentUserDTO
                {
                    id = member.MemberId,
                    username = member.Username,
                    email = member.Email ?? string.Empty,
                };

                return Ok(currentUser);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }


        }
    }
}
