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
            this._httpContextAccessor = _httpContextAccessor;
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
                    var message = "<h1 style=\"background-color: cornflowerblue; color: aliceblue\">Mumu 用戶註冊驗證</h1>";
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

                var token = "";

                if (member == null) return BadRequest("帳號或密碼錯誤");

                if (member != null && member.StatusId == 8)
                {
                    return BadRequest("帳號已被停權");
                }

                var admin = await _context.Admins.AnyAsync(a => a.MemberId == member!.MemberId);

                if (member != null && admin)
                {
                    token = (new JwtGenerator(_configuration)).GenerateJwtToken(user.username, "admin", member!.MemberId);
                }
                else if (member != null)
                {
                    token = (new JwtGenerator(_configuration)).GenerateJwtToken(user.username, "user", member.MemberId);
                }

                string jwt = "Bearer " + token;

                // Add the token to the response headers
                Response.Headers.Append("Access-Control-Expose-Headers", "Authorization");
                Response.Headers["Authorization"] = jwt;

                return Ok(jwt);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpGet("get-current-user"), Authorize(Roles = "user, admin")]
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




        [HttpPost("resend-verify-email"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> ReSendEmail(string email)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                string id = decodeJwtId(jwt);

                var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == int.Parse(id));

                if (member == null)
                {
                    return BadRequest("無此用戶");
                }

                var receiver = member.Email;
                var subject = "Mumu 用戶註冊驗證";
                var message = "<h1 style=\"background-color: cornflowerblue; color: aliceblue\">Mumu 用戶註冊驗證</h1>";
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

                var token = (new JwtGenerator(_configuration)).GenerateJwtToken(member.Username, "guest", member.MemberId);

                message += "<a href='https://mumumsit158.com/reset-password/" + token + "'>點擊這裡</a>進行重設";

                await _emailSender.SendEmailAsync(receiver!, subject, message);

                return Ok("信件已寄出");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("change-password"), Authorize(Roles = "guest, user, admin")]
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


        //限定登入者為user
        [HttpGet("get-user-id"), Authorize(Roles = "user, admin")]
        public IActionResult GetUserId()
        {
            //前端的token資料
            string? jwt = HttpContext.Request.Headers["Authorization"];

            if (jwt == null || jwt == "") return BadRequest();

            string id = decodeJwtId(jwt);

            return Ok(id);
        }

        private string decodeJwtId(string jwt)
        {
            jwt = jwt.Replace("Bearer ", "");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return id!;
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

                    var tokenNew = (new JwtGenerator(_configuration)).GenerateJwtToken(user.username, "user", newUser.MemberId);

                    string jwtNew = "Bearer " + tokenNew;

                    // Add the token to the response headers
                    Response.Headers.Append("Access-Control-Expose-Headers", "Authorization");
                    Response.Headers["Authorization"] = jwtNew;

                    return Ok(jwtNew);
                }

                if (member != null && !Hash.VerifyHashedPassword(user.uid, member!.Password!))
                {
                    return BadRequest("錯誤，請聯絡客服");
                }

                if (member != null && member.StatusId == 8)
                {
                    return BadRequest("帳號已被停權");
                }


                var token = (new JwtGenerator(_configuration)).GenerateJwtToken(user.username, "user", member!.MemberId);

                string jwt = "Bearer " + token;

                return Ok(jwt);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
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
                  Thumbnail = m.Thumbnail,
                  Email = m.Email,
                  Address = m.Address,
                  MemberIntroduction = m.MemberIntroduction,
                  Phone = m.Phone,
                  RegistrationTime = m.RegistrationTime,
                  StatusId = m.StatusId,
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

        [HttpGet("check-admin"), Authorize(Roles = "admin")]
        public async Task<IActionResult> CheckIsAdmin()
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];
                if (jwt == null || jwt == "") return BadRequest();

                string id = decodeJwtId(jwt);

                var member = await _context.Admins.AnyAsync(a => a.MemberId == int.Parse(id));

                return Ok("管理員");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("user-info/users/{id}")]
        public async Task<IActionResult> GetUserInfo(string id)
        {
            try
            {
                //var member = await _context.Members
                //    .Where(m => m.MemberId == int.Parse(id))
                //    .Join(_context.Projects,
                //          m => m.MemberId,
                //          p => p.MemberId,
                //          (m, p) => new
                //          {
                //              m.MemberId,
                //              m.Nickname,
                //              m.Username,
                //              m.Email,
                //              m.MemberIntroduction,
                //              MemberThumbnail = m.Thumbnail,
                //              MemberStatusId = m.StatusId,
                //              m.RegistrationTime,
                //              p.ProjectId,
                //              p.ProjectName,
                //              p.ProjectDescription,
                //              p.ProjectGoal,
                //              p.EndDate,
                //              p.GroupId,
                //              ProjectThumbnail = p.Thumbnail,
                //              ProjectStatusID = p.StatusId
                //          }).ToListAsync();

                //if (member == null)
                //{
                //    return NotFound("Member not found.");
                //}

                //var memberInfoDTOs = member.Select(m => new MemberInfoDTO
                //{
                //    id = m.MemberId,
                //    nickname = m.Nickname ?? string.Empty,
                //    username = m.Username,
                //    email = m.Email ?? string.Empty,
                //    description = m.MemberIntroduction ?? string.Empty,
                //    avatar = m.MemberThumbnail ?? string.Empty,
                //    time = m.RegistrationTime.ToString() ?? string.Empty,
                //    memberStatus = m.MemberStatusId ?? 0,
                //    projects =
                //    [
                //        new GetProjectDTO
                //        {
                //            projectId = m.ProjectId,
                //            projectName = m.ProjectName ?? string.Empty,
                //            projectDescription = m.ProjectDescription ?? string.Empty,
                //            projectGoal = m.ProjectGoal,
                //            projectEndDate = m.EndDate,
                //            projectGroupId = m.GroupId ?? 0,
                //            projectThumbnail = m.ProjectThumbnail ?? string.Empty,
                //            projectStatusId = m.ProjectStatusID
                //        }
                //    ]


                //}).ToList();

                var foundMember = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == int.Parse(id));

                if (foundMember == null)
                {
                    return NotFound("Member not found.");
                }

                var memberInfoDTOs = new MemberInfoDTO
                {
                    id = foundMember.MemberId,
                    nickname = foundMember.Nickname ?? string.Empty,
                    username = foundMember.Username,
                    email = foundMember.Email ?? string.Empty,
                    description = foundMember.MemberIntroduction ?? string.Empty,
                    avatar = foundMember.Thumbnail ?? string.Empty,
                    time = foundMember.RegistrationTime ?? DateTime.Now,
                    memberStatus = foundMember.StatusId ?? 0,
                    projects = await _context.Projects.OrderByDescending(p => p.StartDate).Where(p => p.MemberId == foundMember.MemberId)
                        .Select(p => new GetProjectDTO
                        {
                            projectId = p.ProjectId,
                            projectName = p.ProjectName ?? string.Empty,
                            projectDescription = p.ProjectDescription ?? string.Empty,
                            projectGoal = p.ProjectGoal,
                            projectStartDate = p.StartDate,
                            projectEndDate = p.EndDate,
                            projectGroupId = p.GroupId ?? 0,
                            projectThumbnail = p.Thumbnail ?? string.Empty,
                            projectStatusId = p.StatusId
                        }).ToArrayAsync()
                };

                return Ok(memberInfoDTOs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
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

        [HttpGet("GetStaff"), Authorize(Roles = "user, admin")]
        public IEnumerable<MemberDTO> GetStaff()
        {
            string? jwt = HttpContext.Request.Headers["Authorization"];

            if (jwt == null || jwt == "") return null;

            string id = decodeJwtId(jwt);
            int mId = int.Parse(id);

            var groupDetails = _context.GroupDetails
                            .Where(gd => gd.MemberId == mId)
                            .Select(gd => gd.GroupId)
                            .ToList();
            if (groupDetails.Any())
            {
                // 獲取所有相關的 GroupId
                var groupIds = groupDetails.Distinct().ToList();

                // 使用這些 GroupId 查詢所有成員
                var members = from m in _context.Members
                              join gd in _context.GroupDetails on m.MemberId equals gd.MemberId
                              where groupIds.Contains(gd.GroupId)
                              select new MemberDTO
                              {
                                  MemberId = m.MemberId,
                                  Username = m.Username,
                                  Nickname = m.Nickname,
                                  Thumbnail = m.Thumbnail,
                                  Email = m.Email,
                                  Phone = m.Phone,
                                  GroupDetail = new GroupDetailDTO
                                  {
                                      AuthStatusId = gd.AuthStatusId
                                  },
                              };

                return members.ToList();
            }
            return null;
        }
    }
}


