using BoulevardOfBrokenDreams.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly MumuDbContext _context;

        public CouponsController(MumuDbContext context)
        {
            this._context = context;
        }


        // GET api/<CouponsController>/5
        [HttpGet("{couponsId}/{projectId}")]
        public IActionResult getCoupons(string couponsId ,int projectId)
        {

            var getcoupons = _context.Coupons.FirstOrDefault(cc => cc.Code == couponsId&&cc.ProjectId==projectId);
            if (getcoupons == null||getcoupons.CurrentStock==0||getcoupons.StatusId==10)
            {
                return Ok(0);
            }       
            return Ok(getcoupons.Discount);
        }

    }
}
    