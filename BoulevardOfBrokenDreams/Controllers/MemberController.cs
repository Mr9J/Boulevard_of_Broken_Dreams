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
                bool isAdmin = false;

                if (member == null) return BadRequest("帳號或密碼錯誤");

                if (member != null && member.StatusId == 8)
                {
                    return BadRequest("帳號已被停權");
                }

                var admin = await _context.Admins.AnyAsync(a => a.MemberId == member!.MemberId);

                //var hobby = await _context.Hobbies.AnyAsync(h => h.MemberId == member!.MemberId);

                bool hobby = member?.Hobbies.Count > 0;

                if (member != null && admin)
                {
                    isAdmin = true;
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

                SignInSuccessDTO signInSuccessDTO = new SignInSuccessDTO
                {
                    jwt = jwt,
                    isAdmin = isAdmin,
                };


                return Ok(signInSuccessDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
                    thumbnail = member.Thumbnail ?? string.Empty,
                    authenticationProvider = member.AuthenticationProvider ?? "N",
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
                        AuthenticationProvider = "Y",
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
            var adminMemberIds = _context.Admins.Select(a => a.MemberId).ToList();
            return _context.Members
                .Where(m => !adminMemberIds.Contains(m.MemberId))
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
            var adminMemberIds = _context.Admins.Select(a => a.MemberId).ToList();
            int activeMemberCount = _context.Members.Where(p => p.StatusId == 7 && !adminMemberIds.Contains(p.MemberId)).Count();
            int inactiveMemberCount = _context.Members.Where(p => p.StatusId == 8 && !adminMemberIds.Contains(p.MemberId)).Count();
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

        [HttpGet("get-member-by-id/{id}")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            try
            {
                var foundMember = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == id);

                if (foundMember == null)
                {
                    return NotFound("Member not found.");
                }



                MemberDTO memberDTO = new MemberDTO
                {
                    MemberId = foundMember.MemberId,
                    Username = foundMember.Username,
                    Nickname = foundMember.Nickname,
                    Thumbnail = foundMember.Thumbnail,
                    Email = foundMember.Email,
                    Address = foundMember.Address,
                    MemberIntroduction = foundMember.MemberIntroduction,
                    Phone = foundMember.Phone,
                    RegistrationTime = foundMember.RegistrationTime,
                };

                return Ok(memberDTO);
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
                var foundMember = await _context.Members.Include(m => m.Projects).Include(m => m.Posts).Include(m => m.FollowerFollowings).FirstOrDefaultAsync(m => m.MemberId == int.Parse(id));

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
                    address = foundMember.Address ?? string.Empty,
                    phone = foundMember.Phone ?? string.Empty,
                    description = foundMember.MemberIntroduction ?? string.Empty,
                    avatar = foundMember.Thumbnail ?? string.Empty,
                    time = foundMember.RegistrationTime ?? DateTime.Now,
                    memberStatus = foundMember.StatusId ?? 0,
                    banner = foundMember.Banner ?? string.Empty,
                    authenticationProvider = foundMember.AuthenticationProvider ?? string.Empty,
                    showContactInfo = foundMember.ShowContactInfo ?? string.Empty,
                    postCount = foundMember.Posts.Count,
                    followCount = foundMember.FollowerFollowings.Count,
                    //projects = await _context.Projects.OrderByDescending(p => p.StartDate).Where(p => p.MemberId == foundMember.MemberId)
                    //    .Select(p => new GetProjectDTO
                    //    {
                    //        projectId = p.ProjectId,
                    //        projectName = p.ProjectName ?? string.Empty,
                    //        projectDescription = p.ProjectDescription ?? string.Empty,
                    //        projectGoal = p.ProjectGoal,
                    //        projectStartDate = p.StartDate,
                    //        projectEndDate = p.EndDate,
                    //        projectGroupId = p.GroupId ?? 0,
                    //        projectThumbnail = p.Thumbnail ?? string.Empty,
                    //        projectStatusId = p.StatusId
                    //    }).ToArrayAsync()
                    projects = foundMember.Projects.Select(p => new GetProjectDTO
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
                    }).ToArray()
                };

                return Ok(memberInfoDTOs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-member-sponsored/{id}"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> GetMemberSponsored(int id)
        {
            try
            {
                var projects = await _context.Orders.Where(o => o.MemberId == id).OrderByDescending(o => o.OrderDate)
                    .Join(_context.OrderDetails, o => o.OrderId, od => od.OrderDetailId, (o, od) => new
                    {
                        o.MemberId,
                        od.ProjectId
                    }).Join(_context.Projects, t => t.ProjectId, p => p.ProjectId, (t, p) => new
                    {
                        t.ProjectId,
                        p.ProjectName,
                        p.ProjectDescription,
                        p.ProjectGoal,
                        p.StartDate,
                        p.EndDate,
                        p.GroupId,
                        p.Thumbnail,
                        p.StatusId,
                    }).Distinct().ToListAsync();

                if (projects == null)
                {
                    return NotFound("No projects found.");
                }

                List<GetProjectDTO> projectDTOs = new List<GetProjectDTO>();
                foreach (var project in projects)
                {
                    projectDTOs.Add(new GetProjectDTO
                    {
                        projectId = project.ProjectId,
                        projectName = project.ProjectName ?? string.Empty,
                        projectDescription = project.ProjectDescription ?? string.Empty,
                        projectGoal = project.ProjectGoal,
                        projectStartDate = project.StartDate,
                        projectEndDate = project.EndDate,
                        projectGroupId = project.GroupId ?? 0,
                        projectThumbnail = project.Thumbnail ?? string.Empty,
                        projectStatusId = project.StatusId
                    });
                }

                return Ok(projectDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPatch("update-member-profile"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> UpdateMemberProfile(MemberProfileDTO x)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                string jwtId = decodeJwtId(jwt);


                if (jwtId != x.id.ToString())
                {
                    return BadRequest("權限異常，請聯絡客服");
                }

                var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == x.id);

                if (member == null)
                {
                    return NotFound("Member not found.");
                }

                if (!string.IsNullOrEmpty(x.nickname))
                {
                    member.Nickname = x.nickname;
                }

                if (!string.IsNullOrEmpty(x.password) && member.AuthenticationProvider != "Y")
                {
                    member.Password = Hash.HashPassword(x.password);
                }

                if (!string.IsNullOrEmpty(x.address))
                {
                    member.Address = x.address;
                }

                if (!string.IsNullOrEmpty(x.thumbnail))
                {
                    member.Thumbnail = x.thumbnail;
                }

                if (!string.IsNullOrEmpty(x.memberIntroduction))
                {
                    member.MemberIntroduction = x.memberIntroduction;
                }

                if (!string.IsNullOrEmpty(x.phone))
                {
                    member.Phone = x.phone;
                }

                await _context.SaveChangesAsync();

                return Ok("更新成功");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-member-profile/{id}"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> GetMemberProfile(int id)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                string jwtId = decodeJwtId(jwt);

                if (jwtId != id.ToString())
                {
                    return BadRequest("權限異常，請聯絡客服");
                }

                var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == id);

                if (member == null)
                {
                    return NotFound("Member not found.");
                }

                MemberProfileDTO memberProfileDTO = new MemberProfileDTO
                {
                    id = member.MemberId,
                    username = member.Username,
                    nickname = member.Nickname ?? string.Empty,
                    thumbnail = member.Thumbnail ?? string.Empty,
                    email = member.Email ?? string.Empty,
                    address = member.Address ?? string.Empty,
                    memberIntroduction = member.MemberIntroduction ?? string.Empty,
                    phone = member.Phone ?? string.Empty,
                    verified = member.Verified ?? "N",
                    status = member.StatusId ?? 7,
                    banner = member.Banner ?? string.Empty,
                    authenticationProvider = member.AuthenticationProvider ?? string.Empty,
                    showContactInfo = member.ShowContactInfo ?? string.Empty
                };

                return Ok(memberProfileDTO);
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

        [HttpPatch("update-member-banner"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> UpdateMemberBanner(string banner, string id)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                int jwtId = int.Parse(decodeJwtId(jwt));

                if (jwtId != int.Parse(id))
                {
                    return BadRequest("權限異常，請聯絡客服");
                }

                var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == jwtId);

                if (member == null)
                {
                    return NotFound("Member not found.");
                }

                member.Banner = banner;

                _context.SaveChanges();

                return Ok("修改完成");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-project-group/{projectId}"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> GetProjectGroup(int projectId)
        {
            try
            {
                var project = await _context.Projects.Include(p => p.Group).FirstOrDefaultAsync(p => p.ProjectId == projectId);

                if (project == null)
                {
                    return NotFound("Project not found.");
                }

                var group = await _context.GroupDetails.Include(gd => gd.Member).Where(gd => gd.GroupId == project.GroupId).ToListAsync();

                var GroupDTOs = new GroupDTO
                {
                    groupId = project.GroupId ?? null,
                    groupName = project.Group?.GroupName ?? string.Empty,
                    users = group.Select(gd => new SimpleUserDTO
                    {
                        memberId = gd.MemberId,
                        username = gd.Member.Username,
                        nickname = gd.Member.Nickname ?? string.Empty,
                        thumbnail = gd.Member?.Thumbnail ?? string.Empty,
                        authStatus = gd.AuthStatusId
                    }).ToArray()
                };

                return Ok(GroupDTOs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("select-group"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> SelectGroup()
        {
            string? jwt = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(jwt)) return BadRequest();

            int jwtId = int.Parse(decodeJwtId(jwt));

            var groupDetails = await _context.GroupDetails.Include(gd => gd.Group)
                .Where(gd => gd.MemberId == jwtId).ToListAsync();

            var grouplist = groupDetails.Select(
                gd => new
                {
                    groupName = gd.Group.GroupName,
                });

            return Ok(grouplist);
        }

        [HttpPost("update-project-group"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> UpdateProjectGroup(GroupUpdateDTO x)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(jwt))
                    return BadRequest();

                int jwtId = int.Parse(decodeJwtId(jwt));

                if (x.action == "create")
                {
                    var group = new Group
                    {
                        GroupName = x.groupName
                    };

                    _context.Groups.Add(group);
                    await _context.SaveChangesAsync();


                    var groupDetail = new GroupDetail
                    {
                        GroupId = group.GroupId,
                        MemberId = jwtId,
                        AuthStatusId = 1
                    };

                    var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == x.projectId);

                    if (project != null)
                    {
                        project.GroupId = group.GroupId;
                    }

                    _context.GroupDetails.Add(groupDetail);
                    await _context.SaveChangesAsync();

                    return Ok("新增完成");
                }
                else if (x.action == "update")
                {
                    var group = await _context.Groups
                        .Include(g => g.GroupDetails)
                        .FirstOrDefaultAsync(g => g.GroupId == x.groupId);

                    if (group == null) { return NotFound("Group not found."); }

                    group.GroupName = x.groupName;

                    bool userAdd = !string.IsNullOrEmpty(x.username);

                    if (userAdd)
                    {
                        var memberId = await _context.Members.FirstOrDefaultAsync(m => m.Username == x.username);
                        if (memberId == null) { return NotFound("Member not found."); }

                        var groupUserExist = await _context.GroupDetails.AnyAsync(
                            gd => gd.GroupId == x.groupId && gd.MemberId == memberId.MemberId);

                        if (groupUserExist) { return BadRequest("User already in the group."); }

                        var groupDetail = new GroupDetail
                        {
                            GroupId = x.groupId,
                            MemberId = memberId.MemberId,
                            AuthStatusId = 2
                        };
                        _context.GroupDetails.Add(groupDetail);
                    }

                    await _context.SaveChangesAsync();

                    return Ok("更新完成");
                }
                else if (x.action == "delete")
                {
                    if (string.IsNullOrEmpty(x.username)) return BadRequest("請輸入欲刪除的用戶資訊");

                    var memberId = await _context.Members.FirstOrDefaultAsync(m => m.Username == x.username);

                    if (memberId == null) { return NotFound("Member not found."); }

                    var group = await _context.Groups
                        .Include(g => g.GroupDetails)
                        .FirstOrDefaultAsync(g => g.GroupId == x.groupId);

                    if (group == null) { return NotFound("Group not found."); }

                    var groupDetail = await _context.GroupDetails.FirstOrDefaultAsync(
                        gd => gd.GroupId == x.groupId && gd.MemberId == memberId.MemberId);

                    if (groupDetail == null) { return NotFound("Group detail not found."); }

                    _context.GroupDetails.Remove(groupDetail);

                    await _context.SaveChangesAsync();

                    return Ok("刪除完成");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet("set-contact-info/{status}"), Authorize(Roles = "user, admin")]
        public async Task<IActionResult> SetContactInfo(string status)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                int jwtId = int.Parse(decodeJwtId(jwt));

                var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == jwtId);

                if (member == null)
                {
                    return NotFound("Member not found.");
                }

                member.ShowContactInfo = status;

                _context.SaveChanges();

                return Ok("修改完成");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
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


