using BoulevardOfBrokenDreams.Hubs;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectInfoController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHubContext<CommentsHub> _hubContext;

        public ProjectInfoController(MumuDbContext db, IHubContext<CommentsHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
        }

        // GET api/<ProjectInfoController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var project = await _db.Projects
                .Include(x => x.Products)
                .Include(x => x.Member)
                .FirstOrDefaultAsync(proj => proj.ProjectId == id);

            if (project == null) return NotFound("Project not found.");

            var totalDonate = await _db.Orders
                           .Where(o => _db.OrderDetails
                           .Any(od => od.ProjectId == id && od.OrderId == o.OrderId))
                           .SumAsync(o => o.Donate);

            var totalPrice = await _db.OrderDetails
                                    .Where(od => od.ProjectId == id)
                                    .SumAsync(od => od.Price);

            var total = (totalDonate + totalPrice).Value;

            var p = new VMProjectInfo()
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectThumbnail =  project.Thumbnail,
                ProjectGoal = project.ProjectGoal,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                MemberName = project.Member.Username,
                ProjectTotal = total,

                Products = project.Products.Select(p => new DTOProduct
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductDescription = p.ProductDescription,
                    ProductThumbnail = p.Thumbnail,
                    InitialStock = p.InitialStock,
                    CurrentStock = p.CurrentStock
                }).ToList()
                //MemberThumbnail = "https://" + HttpContext.Request.Host.Value + "/resources/mumuThumbnail/members_Thumbnail/" + project.Member.Thumbnail
            };

            // 如果不小心登入了
            string? jwt = Request.Headers.Authorization;
            if (!string.IsNullOrEmpty(jwt))
            {
                int currentMemberId = DecodeJwtToMemberId(jwt);
                // 不小心按讚
                bool isliked = await _db.Likes
                    .Include(l => l.LikeDetails)
                    .AnyAsync(l => l.ProjectId == id && l.LikeDetails.Any(ld => ld.MemberId == currentMemberId));
                p.IsLiked = isliked;

            }

            return Ok(p);
        }

        // 審核中的專案不能買

        // POST api/<ProjectInfoController>/sendComment

        #region 留言相關
        [HttpPost("SendComment")]
        public async Task<IActionResult> SendComment([FromBody] CommentDto commentDto)
        {
            if (commentDto == null || commentDto.CommentMsg == null) return BadRequest("Comment is null.");

            var comment = new Comment()
            {
                CommentMsg = commentDto.CommentMsg,
                Date = DateTime.Now,
                ProjectId = commentDto.ProjectId,
                MemberId = commentDto.MemberId
            };
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();

            // 通知所有客戶端
            commentDto.CommentId = comment.CommentId;
            commentDto.Date = comment.Date;
            await _hubContext.Clients.All.SendAsync("ReceiveComment", commentDto);

            return Ok("Comment sent.");
        }

        

        // GET api/<ProjectInfoController>/GetComments
        [HttpGet("GetComments")]
        public async Task<IActionResult> GetComments(int projectId)
        {
            var comments = await _db.Comments
                .Where(c => c.ProjectId == projectId)
                .ToListAsync();

            if (comments == null) return NotFound("No comments found.");
            return Ok(comments);
        }

        #endregion

        #region 收藏相關

        // POST api/<ProjectInfoController>/like
        [HttpPost("Like"), Authorize(Roles = "user")]
        public async Task<IActionResult> Like(int projectId)
        {
            string? jwt = Request.Headers.Authorization;
            int memberId = DecodeJwtToMemberId(jwt);

            // 已經按過讚
            bool isLiked = await _db.Likes
                .Include(l => l.LikeDetails)
                .AnyAsync(l => l.ProjectId == projectId && l.LikeDetails.Any(ld => ld.MemberId == DecodeJwtToMemberId(Request.Headers.Authorization)));
            if (isLiked) return Ok("Already liked.");

            var newLike = new Like()
            {
                ProjectId = projectId
            };
            await _db.Likes.AddAsync(newLike);
            await _db.SaveChangesAsync();

            var newLikeDetail = new LikeDetail()
            {
                MemberId = memberId,
                LikeId = newLike.LikeId
            };
            await _db.LikeDetails.AddAsync(newLikeDetail);
            await _db.SaveChangesAsync();

            return Ok("Liked.");
        }

        // DELETE api/<ProjectInfoController>/like
        [HttpDelete("Like"), Authorize(Roles = "user")]
        public async Task<IActionResult> Unlike(int projectId)
        {
            string? jwt = Request.Headers.Authorization;
            int memberId = DecodeJwtToMemberId(jwt);

            // 用 memberId 跟 projectId 找到要刪除的 likeDetail
            var removeLikeDetail = await _db.LikeDetails.Include(ld => ld.Like)
                .FirstOrDefaultAsync(ld => ld.MemberId == memberId && ld.Like.ProjectId == projectId);

            if (removeLikeDetail == null) return NotFound("Like not found.");
            _db.LikeDetails.Remove(removeLikeDetail);

            var removeLike = await _db.Likes.FindAsync(removeLikeDetail.LikeId);
            if (removeLike == null) return NotFound("Like not found.");


            _db.Likes.Remove(removeLike);
            await _db.SaveChangesAsync();

            return Ok("Unliked.");
        }

        #endregion

        private static int DecodeJwtToMemberId(string? jwt)
        {
            jwt = jwt.Replace("Bearer ", "");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);
            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(id!);
        }

        // GET api/<ProjectInfoController>/5
        [HttpGet("Member")]
        public async Task<IActionResult> GetMemberById(int memberId)
        {
            var member = await _db.Members.SingleOrDefaultAsync(m=>m.MemberId == memberId);
            if(member == null) return NotFound("Member not found.");
            var dtoMember = new DTOMember()
            {
                MemberId = member.MemberId,
                Username = member.Username,
                Thumbnail = member.Thumbnail
            };

            return Ok(dtoMember);
        }
    }
}
