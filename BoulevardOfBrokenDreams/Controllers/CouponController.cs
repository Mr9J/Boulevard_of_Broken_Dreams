using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private MumuDbContext _context;
        public CouponController(MumuDbContext context)
        {
            _context = context;
        }

        // GET: api/<CouponController>
        [HttpGet]
        public IActionResult Get()
        {
            var coupons = _context.Coupons.ToList();
            return Ok(coupons);
        }

        // GET api/<CouponController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CouponController>
        [HttpPost]
        public IActionResult Post([FromBody] CouponDTO value)
        {
            var Coupon = new Coupon
            {
                ProjectId = value.ProjectId,
                Code = value.Code,
                Discount = value.Discount,
                InitialStock = value.InitialStock,
                CurrentStock = value.InitialStock,
                Deadline = value.Deadline,
                StatusId = 9,
                //StatusId = value.StatusId,
            };
            _context.Coupons.Add(Coupon);
            _context.SaveChanges();
            return Ok(Coupon);
        }

        // PUT api/<CouponController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CouponController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        private string decodeJwtId(string jwt)
        {
            jwt = jwt.Replace("Bearer ", "");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return id!;
        }

        [HttpGet("CouponList"), Authorize(Roles = "user")]
        public IActionResult GetCouponList()
        {
            string? jwt = HttpContext.Request.Headers["Authorization"];

            if (jwt == null || jwt == "") return BadRequest();

            string id = decodeJwtId(jwt);
            int mId = int.Parse(id);
            var coupons = (from project in _context.Projects
                           where project.MemberId == mId
                           join coupon in _context.Coupons on project.ProjectId equals coupon.ProjectId
                           select new CouponDTO
                           {
                               CouponId = coupon.CouponId,
                               ProjectId = coupon.ProjectId,
                               Code = coupon.Code,
                               Discount = coupon.Discount,
                               InitialStock = coupon.InitialStock,
                               CurrentStock = coupon.CurrentStock,
                               Deadline = coupon.Deadline,
                               StatusId = coupon.StatusId,
                               ProjectName = project.ProjectName,
                               ProjectThumbnail = project.Thumbnail,
                           }).OrderByDescending(c => c.CouponId).ToList();
            return Ok(coupons);
        }

        [HttpGet("{couponsId}/{projectId}")]
        public IActionResult getCoupons(string couponsId, int projectId)
        {

            var getcoupons = _context.Coupons.FirstOrDefault(cc => cc.Code == couponsId && cc.ProjectId == projectId);
            if (getcoupons == null || getcoupons.CurrentStock == 0 || getcoupons.StatusId == 10)
            {
                return Ok(0);
            }
            return Ok(getcoupons.Discount);
        }

        [HttpGet("UsedList/{couponId}"), Authorize(Roles = "user")]
        public IActionResult getCouponsUsedList(int couponId)
        {
            var usedlist = from o in _context.Orders
                           join m in _context.Members on o.MemberId equals m.MemberId
                           where o.CouponId == couponId
                           select new MemberDTO
                           {
                               Username = m.Username,
                               Thumbnail = m.Thumbnail,
                           };

            if (!usedlist.Any())
            {
                return NotFound($"No orders found for coupon ID {couponId}.");
            }
            return Ok(usedlist.ToList());
        }
    }
}