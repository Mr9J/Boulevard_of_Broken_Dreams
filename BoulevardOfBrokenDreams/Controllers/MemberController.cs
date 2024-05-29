using BoulevardOfBrokenDreams.DataAccess;
using BoulevardOfBrokenDreams.Interface;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using BoulevardOfBrokenDreams.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly MumuDbContext _context;
        private readonly IConfiguration _configuration;
        private MemberRepository _memberRepository;
        private readonly IEmailSender _emailSender;
        public MemberController(MumuDbContext _context, IConfiguration _configuration, IEmailSender _emailSender)
        {
            this._context = _context;
            this._configuration = _configuration;
            this._memberRepository = new MemberRepository(this._context);
            this._emailSender = _emailSender;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDTO user)
        {
            try
            {
                string res = await _memberRepository.CreateMember(user);

                if (res == "註冊成功")
                {
                    //測試用，應該要在前端做
                    Member? member = await _memberRepository.GetMember(user.username);
                    if (member == null) return BadRequest("註冊失敗");
                    var receiver = user.email;
                    var subject = "Mumu 用戶註冊驗證";
                    var message = $"請點擊以下連結驗證您的帳號: https://localhost:7150/mumu/verify/verify-email/{member.Username}/{member.Eid}";

                    await _emailSender.SendEmailAsync(receiver, subject, message);


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

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInDTO user)
        {
            try
            {
                Member? member = await _memberRepository.AuthMember(user);

                if (member != null)
                {
                    var token = (new JwtGenerator(_configuration)).GenerateJwtToken(user.username, "user");

                    string jwt = "Bearer " + token;

                    return Ok(jwt);
                }
                else
                {
                    return BadRequest("帳號或密碼錯誤");
                }
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpGet("check-username"), Authorize]
        public async Task<IActionResult> GetCurrentUser(string username)
        {
            Member? member = await _memberRepository.GetMember(username);

            if (member != null)
            {
                return Ok(member);
            }
            else
            {
                return BadRequest(member);
            }
        }


        [HttpPost("get-current-user"), Authorize]
        public async Task<IActionResult> getCurrentUser(string jwt)
        {
            try
            {
                if (jwt == null || jwt == "")
                {
                    return BadRequest("請輸入 JWT");
                }

                jwt = jwt.Replace("Bearer ", "");

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

                string? username = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;

                if (username == null)
                {
                    return NotFound("使用者不存在");
                }

                Member? member = await _memberRepository.GetMember(username);

                if (member == null)
                {
                    return NotFound("使用者不存在");
                }

                GetCurrentUserDTO currentUser = new GetCurrentUserDTO
                {
                    id = member.MemberId.ToString(),
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


        [HttpGet("send-email")]
        public async Task<IActionResult> SendMail()
        {
            try
            {
                var receiver = "yasmin.corwin16@ethereal.email";
                var subject = "測試";
                //StringBuilder mailBody = new StringBuilder();
                //mailBody.AppendFormat("<h1>用戶註冊</h1>");
                //mailBody.AppendFormat("<br />");
                //mailBody.AppendFormat("<p>感謝您的註冊</p>");
                //var message = mailBody.ToString();
                var message = "測試完成";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                return Ok("信件已寄出");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
