using BoulevardOfBrokenDreams.Models.DTO;
using BoulevardOfBrokenDreams.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikeController(MumuDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{userId}")]
        public IActionResult Like(int userId)
        {
            var likes = _db.LikeDetails
                        .Where(x => x.MemberId == userId)
                        .Include(x => x.Like)
                        .ThenInclude(lk => lk.Project)
                        .Select(x => new LikeDTO
                        {

                            LikePrjName = x.Like.Project.ProjectName,
                            LikePrjThumb = x.Like.Project.Thumbnail,
                            LikeDetailId = x.LikeDetailId,
                            LikePrjId=x.Like.ProjectId
                        }).ToList();


            if (likes == null)
            {
                return NotFound("無資料");
            }

            return Ok(likes);
        }

        [HttpDelete("{LikeDetailId}")]
        public async Task<IActionResult> DeleteLike(int LikeDetailId)
        {
            var delLikeDetail = await _db.LikeDetails.FindAsync(LikeDetailId);
            if (delLikeDetail == null)
            {
                return NotFound();
            }
            var delLike = await _db.Likes.FirstOrDefaultAsync(x => x.LikeId == delLikeDetail.LikeId);

            _db.LikeDetails.Remove(delLikeDetail);

            if (delLike != null)
            {
                _db.Likes.Remove(delLike);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
