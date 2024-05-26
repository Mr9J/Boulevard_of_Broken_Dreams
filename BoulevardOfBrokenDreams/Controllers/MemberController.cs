﻿using BoulevardOfBrokenDreams.DataAccess;
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
        private readonly MumuDbContext _context;
        private readonly IConfiguration _configuration;
        private MemberRepository _memberRepository;
        public MemberController(MumuDbContext _context, IConfiguration _configuration)
        {
            this._context = _context;
            this._configuration = _configuration;
            this._memberRepository = new MemberRepository(this._context);
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO user)
        {
            try
            {
                string res = await _memberRepository.CreateMember(user);

                if (res == "註冊成功")
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

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO user)
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

                Member? member = await _memberRepository.GetMember(username);

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
