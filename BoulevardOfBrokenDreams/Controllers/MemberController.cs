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
using System.Runtime.InteropServices;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemberController(MumuDbContext _context, IConfiguration _configuration, IEmailSender _emailSender, IHttpContextAccessor _httpContextAccessor)
        {
            this._context = _context;
            this._configuration = _configuration;
            this._memberRepository = new MemberRepository(this._context);
            this._emailSender = _emailSender;
            _httpContextAccessor = _httpContextAccessor;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDTO user)
        {
            try
            {
                string res = await _memberRepository.CreateMember(user);

                if (res == "註冊成功")
                {
                    Member? member = await _memberRepository.GetMember(user.username);
                    if (member == null) return BadRequest("註冊失敗");
                    var receiver = user.email;
                    var subject = "Mumu 用戶註冊驗證";
                    var message = "<h1>歡迎註冊Mumu</h1>";
                    message += "<p>請點擊以下連結驗證您的帳號:</p>";
                    message += "<a href='https://mumumsit158.com/email-verify/" + member.Username + "/" + member.Eid + "'>點擊這裡</a>進行驗證";

                    await _emailSender.SendEmailAsync(receiver, subject, message);

                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInDTO user)
        {
            try
            {
                Member? member = await _memberRepository.AuthMember(user);

                if (member == null) return BadRequest("帳號或密碼錯誤");

                if (member != null && member.StatusId == 8)
                {
                    return BadRequest("帳號已被停權");
                }

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

        [HttpGet("get-current-user"), Authorize(Roles = "user")]
        public async Task<IActionResult> getCurrentUser()
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                string username = decodeJWT(jwt);

                if (username == null)
                {
                    return BadRequest();
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
                    nickname = member.Nickname ?? string.Empty,
                    thumbnail = member.Thumbnail ?? string.Empty
                };

                return Ok(currentUser);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }


        [HttpPost("resend-verify-email"), Authorize(Roles = "user")]
        public async Task<IActionResult> ReSendEmail(string username)
        {
            try
            {
                Member? member = await _memberRepository.GetMember(username);

                if (member == null)
                {
                    return BadRequest("無此用戶");
                }

                var receiver = member.Email;
                var subject = "Mumu 用戶註冊驗證";
                var message = "<h1 style=\"background-color: cornflowerblue; color: aliceblue\">歡迎註冊MUMU</h1>";
                message += "<p>請點擊以下連結驗證您的帳號 : </p>";
                message += "<a href='https://mumumsit158.com/email-verify/" + member.Username + "/" + member.Eid + "'>點擊這裡</a>進行驗證";

                await _emailSender.SendEmailAsync(receiver!, subject, message);

                return Ok("信件已寄出");
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            try
            {
                if (email == null || email == "") return BadRequest();

                Member? member = await _memberRepository.GetMemberByEmail(email);

                if (member == null)
                {
                    return BadRequest("無此用戶");
                }

                member.ResetPassword = "Y";
                await _context.SaveChangesAsync();

                var receiver = member.Email;
                var subject = "Mumu 重設密碼";
                var message = "<h1 style=\"background-color: cornflowerblue; color: aliceblue\">Mumu 重設密碼</h1>";
                message += "<p>請點擊以下連結重設您的密碼 : </p>";

                var token = (new JwtGenerator(_configuration)).GenerateJwtToken(member.Username, "guest");

                message += "<a href='https://mumumsit158.com/reset-password/" + token + "'>點擊這裡</a>進行重設";

                await _emailSender.SendEmailAsync(receiver!, subject, message);

                return Ok("信件已寄出");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("change-password"), Authorize(Roles = "guest")]
        public async Task<IActionResult> ChangePassword(string password)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                string username = decodeJWT(jwt);

                if (username == null)
                {
                    return BadRequest();
                }

                Member? member = await _memberRepository.GetMember(username);


                if (member == null)
                {
                    return NotFound("使用者不存在");
                }

                if (member.Verified == "N")
                {
                    return BadRequest("帳號尚未驗證");
                }

                if (member.ResetPassword == "N")
                {
                    return BadRequest("無法重設密碼, 請重新要求密碼重設信");
                }

                member.Password = Hash.HashPassword(password);
                member.ResetPassword = "N";

                await _context.SaveChangesAsync();

                return Ok("密碼已變更");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private string decodeJWT(string jwt)
        {
            jwt = jwt.Replace("Bearer ", "");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

            string? username = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;

            return username!;
        }

        [HttpPost("sign-in-with-others")]
        public async Task<IActionResult> SignInWithOthers(OuterSignInDTO user)
        {
            try
            {
                Member? member = await _memberRepository.GetMember(user.username);

                if (member == null)
                {
                    Member newUser = new Member
                    {
                        Nickname = user.nickname,
                        Username = user.username,
                        Thumbnail = user.thumbnail,
                        Password = Hash.HashPassword(user.uid),
                        ResetPassword = "N",
                        Verified = "Y",
                        RegistrationTime = DateTime.UtcNow,
                    };

                    _context.Members.Add(newUser);
                    await _context.SaveChangesAsync();
                }

                if (member != null && !Hash.VerifyHashedPassword(user.uid, member!.Password!))
                {
                    return BadRequest("錯誤，請聯絡管理員");
                }

                var token = (new JwtGenerator(_configuration)).GenerateJwtToken(user.username, "user");

                string jwt = "Bearer " + token;

                return Ok(jwt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public IEnumerable<MemberDTO> Get()
        {
            return _context.Members
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


