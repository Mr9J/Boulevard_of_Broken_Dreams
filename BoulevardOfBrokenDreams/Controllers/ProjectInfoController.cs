using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectInfoController : ControllerBase
    {
        private MumuDbContext _db;

        public ProjectInfoController(MumuDbContext db)
        {
            _db = db;
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

            var p = new VMProjectInfo()
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectThumbnail = "https://" + HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + project.Thumbnail,
                ProjectGoal = project.ProjectGoal,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                MemberId = project.MemberId
            };

            return Ok(p);
        }

        // 審核中的專案不能買

        // POST api/<ProjectInfoController>/sendComment
        [HttpPost("SendComment")]
        public async Task<IActionResult> SendComment([FromBody] CommentDto commentDto)
        {
            if (commentDto == null || commentDto.CommentMsg==null) return BadRequest("Comment is null.");

            var comment = new Comment()
            {
                CommentMsg = commentDto.CommentMsg,
                Date = DateTime.Now,
                ProjectId = commentDto.ProjectId,
                MemberId = commentDto.MemberId
            };
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();

            return Ok("Comment sent.");
        }

        public class CommentDto
        {
            public string? CommentMsg { get; set; }
            public int ProjectId { get; set; }
            public int MemberId { get; set; }
        }
    }
}
