using Azure.Core;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;
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
            string? jwt = httpContext.Request.Query["access_token"];
            if (string.IsNullOrEmpty(jwt)) return;
            int memberId = DecodeJwtToMemberId(jwt);

            // 如果會員不存在或留言為空則不處理
            var member = await _context.Members.FindAsync(memberId);
            if(member == null) return;
            if(comment.CommentMsg == null) return;

            // 存入這筆留言到資料庫
            var dbComment = new Comment
            {
                CommentMsg = comment.CommentMsg,
                ProjectId = comment.ProjectId,
                Member = member,
                Date = DateTime.Now,
                ParentId = comment.ParentId??null
            };
            await _context.Comments.AddAsync(dbComment);
            await _context.SaveChangesAsync();

            // 將新增的這筆留言發送到所有客戶端
            var distributeComment = new CommentDto
            {
                CommentId = dbComment.CommentId,
                CommentMsg = dbComment.CommentMsg,
                ProjectId = dbComment.ProjectId,
                MemberId = dbComment.MemberId,
                Date = dbComment.Date,
                ParentId = dbComment.ParentId??null,
                Member = new DTOMember
                {
                    MemberId = memberId,
                    Username = dbComment.Member?.Nickname,
                    Thumbnail = dbComment.Member?.Thumbnail
                }
                
            };
            await Clients.All.SendAsync("ReceiveComment", distributeComment);
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
