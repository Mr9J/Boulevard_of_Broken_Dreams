using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly MumuDbContext _context;
        public PostController(MumuDbContext context)
        {
            this._context = context;
        }

        [HttpPost("create-post"), Authorize(Roles = "user")]
        public async Task<IActionResult> CreatePost(NewPostDTO newPost)
        {
            string validationRes = PostValidation(newPost);

            if (!string.IsNullOrEmpty(validationRes))
            {
                return BadRequest(validationRes);
            }

            Post post = new Post
            {
                MemberId = int.Parse(newPost.userId),
                Caption = newPost.caption,
                ImgUrl = newPost.file,
                Location = newPost.location,
                Tags = newPost.tags,
                PostTime = DateTime.UtcNow,
            };

            //_context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok("新增成功");
        }

        private string PostValidation(NewPostDTO newPost)
        {
            if (newPost == null)
            {
                return "請輸入內容";
            }

            if (newPost.caption == null || newPost.caption == "")
            {
                return "caption 不能為空";
            }

            if (newPost.file == null || newPost.file == "")
            {
                return "file 不能為空";
            }

            if (newPost.location == null || newPost.location == "")
            {
                return "location 不能為空";
            }

            if (newPost.tags == null || newPost.tags == "")
            {
                return "tags 不能為空";
            }

            if (newPost.userId == null || newPost.userId == "")
            {
                return "userId 不能為空";
            }

            return string.Empty;
        }
    }
}
