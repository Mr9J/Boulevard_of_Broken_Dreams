using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly MumuDbContext _context;
        public ProjectController(MumuDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            try
            {
                List<Project> projects = await _context.Projects.ToListAsync();

                List<ProjectDTO> projectDTOs = new List<ProjectDTO>();

                foreach (var project in projects)
                {
                    projectDTOs.Add(new ProjectDTO
                    {
                        projectId = project.ProjectId.ToString(),
                        projectName = project.ProjectName,
                        projectDescription = project.ProjectDescription!
                    });
                }

                return Ok(projectDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
