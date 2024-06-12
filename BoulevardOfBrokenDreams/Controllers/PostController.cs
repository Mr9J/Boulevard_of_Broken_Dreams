﻿using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
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

        [HttpGet("get-post/{postId}"), Authorize(Roles = "user")]
        public async Task<IActionResult> GetPost(string postId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(int.Parse(postId));

                if (post == null) { return BadRequest("找不到該貼文"); }

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
                    return Ok(postDTO);
                }
                else
                {
                    return BadRequest("找不到該貼文");
                }

            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }


        [HttpGet("get-posts/{page}"), Authorize(Roles = "user")]
        public async Task<IActionResult> GetPosts(int page)
        {
            try
            {
                var posts = await _context.Posts.OrderByDescending(p => p.PostTime).Skip(page * 10).Take(10).ToListAsync();

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

        [HttpGet("like-post-check/{postId}/{userId}"), Authorize(Roles = "user")]
        public async Task<IActionResult> LikePostCheck(string postId, string userId)
        {
            try
            {
                var checkPost = await _context.Posts.AnyAsync(pl => pl.PostId == int.Parse(postId));

                if (!checkPost)
                {
                    return BadRequest("找不到該貼文");
                }

                var likeCount = await _context.PostLikeds.CountAsync(
                    pl => pl.PostId == int.Parse(postId));

                var isLiked = await _context.PostLikeds.AnyAsync(
                    pl => pl.PostId == int.Parse(postId) && pl.MemberId == int.Parse(userId));

                LikePostDTO likePostDTO = new LikePostDTO
                {
                    likeCount = likeCount.ToString(),
                    isLiked = isLiked.ToString()
                };

                return Ok(likePostDTO);

            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpPost("like-post"), Authorize(Roles = "user")]
        public async Task<IActionResult> LikePost(string postId, string userId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(int.Parse(postId));

                if (post == null)
                {
                    return BadRequest("找不到該貼文");
                }

                var foundPostLiked = await _context.PostLikeds.FirstOrDefaultAsync(
                    pl => pl.PostId == int.Parse(postId) && pl.MemberId == int.Parse(userId));

                if (foundPostLiked != null)
                {
                    _context.PostLikeds.Remove(foundPostLiked);
                }
                else
                {
                    PostLiked postLiked = new PostLiked
                    {
                        PostId = int.Parse(postId),
                        MemberId = int.Parse(userId)
                    };

                    _context.PostLikeds.Add(postLiked);
                }

                await _context.SaveChangesAsync();

                return Ok(foundPostLiked != null ? "取消完成" : "按讚完成");
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpGet("save-post-check/{postId}/{userId}"), Authorize(Roles = "user")]
        public async Task<IActionResult> SavePostCheck(string postId, string userId)
        {
            try
            {
                var checkPost = await _context.Posts.AnyAsync(pl => pl.PostId == int.Parse(postId));

                if (!checkPost)
                {
                    return BadRequest("找不到該貼文");
                }

                var isSaved = await _context.PostSaveds.AnyAsync(
                    pl => pl.PostId == int.Parse(postId) && pl.MemberId == int.Parse(userId));

                return Ok(isSaved.ToString());

            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpPost("save-post"), Authorize(Roles = "user")]
        public async Task<IActionResult> SavePost(string postId, string userId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(int.Parse(postId));

                if (post == null)
                {
                    return BadRequest("找不到該貼文");
                }

                var foundPostSaved = await _context.PostSaveds.FirstOrDefaultAsync(
                    pl => pl.PostId == int.Parse(postId) && pl.MemberId == int.Parse(userId));

                if (foundPostSaved != null)
                {
                    _context.PostSaveds.Remove(foundPostSaved);
                }
                else
                {
                    PostSaved postSaved = new PostSaved
                    {
                        PostId = int.Parse(postId),
                        MemberId = int.Parse(userId)
                    };

                    _context.PostSaveds.Add(postSaved);
                }

                await _context.SaveChangesAsync();

                return Ok(foundPostSaved != null ? "取消完成" : "儲存完成");
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpDelete("delete-post/{postId}"), Authorize(Roles = "user")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                string id = decodeJwtId(jwt);

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId && p.MemberId == int.Parse(id));

                if (post == null)
                {
                    return BadRequest("找不到該貼文或權限不足");
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                return Ok("刪除成功");
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpPatch("update-post/{postId}"), Authorize(Roles = "user")]
        public async Task<IActionResult> UpdatePost(UpdatePostDTO update)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(jwt))
                {
                    return BadRequest();
                }

                string username = decodeJWT(jwt);

                var member = await _context.Members.FirstOrDefaultAsync(m => m.Username == username);

                if (member == null)
                {
                    return BadRequest("找不到該會員");
                }

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == update.postId);


                if (post == null)
                {
                    return BadRequest("找不到該貼文");
                }

                if (member.MemberId != post.MemberId)
                {
                    return BadRequest("無法修改他人貼文");
                }

                if (!string.IsNullOrEmpty(update.caption))
                {
                    post.Caption = update.caption;
                }

                if (!string.IsNullOrEmpty(update.file))
                {
                    post.ImgUrl = update.file;
                }

                _context.Posts.Update(post);
                await _context.SaveChangesAsync();

                return Ok("更新成功");
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpPost("comment-post"), Authorize(Roles = "user")]
        public async Task<IActionResult> CommentPost(NewCommentPostDTO newComment)
        {
            try
            {
                string? jwt = HttpContext.Request.Headers["Authorization"];

                if (jwt == null || jwt == "") return BadRequest();

                string id = decodeJwtId(jwt);

                if (newComment.userId != id)
                {
                    return BadRequest("異常行為，權限錯誤");
                }

                var post = await _context.Posts.AnyAsync(p => p.PostId == int.Parse(newComment.postId));

                if (!post)
                {
                    return BadRequest("找不到該貼文");
                }

                PostComment comment = new PostComment
                {
                    PostId = int.Parse(newComment.postId),
                    MemberId = int.Parse(newComment.userId),
                    Comment = newComment.comment,
                    Time = DateTime.UtcNow
                };

                _context.PostComments.Add(comment);
                await _context.SaveChangesAsync();

                return Ok("留言完成");

            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
            }
        }

        [HttpGet("get-comments/{postId}"), Authorize(Roles = "user")]
        public async Task<IActionResult> GetComments(string postId)
        {
            try
            {
                var comments = await _context.PostComments.OrderByDescending(p => p.Time).Where(p => p.PostId == int.Parse(postId)).ToListAsync();

                if (comments.Count == 0) return Ok("沒有留言");

                var commentDTOs = new List<CommentPostDTO>();

                foreach (var comment in comments)
                {
                    var commentDTO = new CommentPostDTO
                    {
                        postId = comment.PostId,
                        userId = comment.MemberId,
                        comment = comment.Comment,
                        time = comment.Time.ToString()
                    };
                    commentDTOs.Add(commentDTO);
                }

                return Ok(commentDTOs);
            }
            catch (Exception)
            {
                return BadRequest("伺服器錯誤，請稍後再試");
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

        private string decodeJwtId(string jwt)
        {
            jwt = jwt.Replace("Bearer ", "");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return id!;
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
