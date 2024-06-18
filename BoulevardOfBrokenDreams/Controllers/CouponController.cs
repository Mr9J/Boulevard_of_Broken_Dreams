using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
    }
}
