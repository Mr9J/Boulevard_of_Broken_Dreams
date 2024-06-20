using Azure.Core;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace BoulevardOfBrokenDreams.Hubs
{
    public class CommentsHub:Hub
    {
        private readonly MumuDbContext _context;

        public CommentsHub(MumuDbContext mumuDbContext)
        {
            _context = mumuDbContext;
        }
        public async Task SendMessage(CommentDto comment)
        {
            // 取得傳送訊息者資料
            var httpContext = Context.GetHttpContext();
            if (httpContext == null) return;
            string? jwt = httpContext.Request.Headers.Authorization;
            if (string.IsNullOrEmpty(jwt)) return;
            int memberId = DecodeJwtToMemberId(jwt);
            
            if(comment.CommentMsg == null) return;

            // 存入這筆留言到資料庫
            var dbComment = new Comment
            {
                CommentMsg = comment.CommentMsg,
                ProjectId = comment.ProjectId,
                MemberId = memberId,
                Date = new DateTime(),
                ParentId = comment.ParentId??null
            };
            await _context.Comments.AddAsync(dbComment);

            // 發送到所有客戶端
            await Clients.All.SendAsync("ReceiveComment", comment);
        }

        private static int DecodeJwtToMemberId(string? jwt)
        {
            jwt = jwt.Replace("Bearer ", "");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);
            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(id!);
        }
    }
}
