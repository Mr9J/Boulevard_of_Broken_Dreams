using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private MumuDbContext _db;
        public CouponController(MumuDbContext db)
        {
            _db = db;
        }

        // GET: api/<CouponController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
            _db.Coupons.Add(Coupon);
            _db.SaveChanges();
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

        [HttpGet("couponList"), Authorize(Roles = "user")]
        public IActionResult GetCouponList()
        {
            string? jwt = HttpContext.Request.Headers["Authorization"];

            if (jwt == null || jwt == "") return BadRequest();

            string id = decodeJwtId(jwt);
            int mId = int.Parse(id);
            var coupons = (from project in _db.Projects
                          where project.MemberId == mId
                          join coupon in _db.Coupons on project.ProjectId equals coupon.ProjectId
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
                          }).ToList();
            return Ok(coupons);
        }
    }
}