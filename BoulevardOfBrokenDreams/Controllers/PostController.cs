using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok("新增成功");
        }

        [HttpGet("get-posts"), Authorize(Roles = "user")]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                var posts = await _context.Posts.OrderByDescending(p => p.PostTime).ToListAsync();

                var postDTOs = new List<PostDTO>();
                foreach (var post in posts)
                {
                    var member = await _context.Members.FindAsync(post.MemberId);
                    if (member != null)
                    {
                        var postDTO = new PostDTO
                        {
                            postId = post.PostId.ToString(),
                            userId = post.MemberId.ToString(),
                            username = member.Nickname!,
                            userImg = member.Thumbnail!,
                            caption = post.Caption!,
                            imgUrl = post.ImgUrl!,
                            location = post.Location!,
                            tags = post.Tags!,
                            postTime = post.PostTime.ToString()!,
                            isAnonymous = post.IsAnonymous!.ToString()
                        };
                        postDTOs.Add(postDTO);
                    }
                }

                return Ok(postDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
