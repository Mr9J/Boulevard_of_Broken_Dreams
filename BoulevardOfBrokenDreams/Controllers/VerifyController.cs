using BoulevardOfBrokenDreams.Interface;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public VerifyController(MumuDbContext context, IConfiguration configuration, IEmailSender emailSender)
        {
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [HttpGet("verify-email/{username}/{eid}")]
        public async Task<IActionResult> VerifyEmail(string username, string eid)
        {
            Member? member = await _context.Members.FirstOrDefaultAsync(m => m.Username == username);

            if (member == null)
            {
                return BadRequest("無此用戶");
            }

            if (member.Eid.ToString() != eid)
            {
                return BadRequest("驗證碼錯誤");
            }

            member.Verified = "Y";

            var cart = await _context.Carts.AnyAsync(c => c.MemberId == member.MemberId);

            if (!cart)
            {
                _context.Carts.Add(new Cart { MemberId = member.MemberId });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("change-email"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> ChangeEmail(string email)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "")
                {
                    return BadRequest("未登入");
                }

                string id = (new JwtGenerator(_configuration)).decodeJwtId(jwt);

                Member? member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == int.Parse(id));

                var emailExists = await _context.Members.AnyAsync(m => m.Email == email);

                if (member == null)
                {
                    return BadRequest("無此用戶");
                }

                if (emailExists)
                {
                    return BadRequest("此信箱已被使用");
                }

                member.Email = email;
                member.Eid = Guid.NewGuid();

                await _context.SaveChangesAsync();

                var receiver = member.Email;
                var subject = "Mumu 用戶註冊驗證";
                var message = "<h1 style=\"background-color: cornflowerblue; color: aliceblue\">Mumu 用戶註冊驗證</h1>";
                message += "<p>請點擊以下連結驗證您的信箱:</p>";
                message += "<a href='https://mumumsit158.com/email-verify/" + member.Username + "/" + member.Eid + "'>點擊這裡</a>進行驗證";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                return Ok("更改完成");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("resend-email"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> ResendEmail()
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "")
                {
                    return BadRequest("未登入");
                }

                string id = (new JwtGenerator(_configuration)).decodeJwtId(jwt);

                Member? member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == int.Parse(id));

                if (member == null)
                {
                    return BadRequest("無此用戶");
                }

                var receiver = member.Email;
                var subject = "Mumu 用戶信箱驗證";
                var message = "<h1 style=\"background-color: cornflowerblue; color: aliceblue\">Mumu 用戶註冊驗證</h1>";
                message += "<p>請點擊以下連結驗證您的信箱:</p>";
                message += "<a href='https://mumumsit158.com/email-verify/" + member.Username + "/" + member.Eid + "'>點擊這裡</a>進行驗證";

                await _emailSender.SendEmailAsync(receiver!, subject, message);

                return Ok("更改完成");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
