using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public OrderDetailController(MumuDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: api/<DashboardController>
        [HttpGet]
        public  IEnumerable<OrderDetailDTO> Get()
        {

            var orderDetails = from od in _db.OrderDetails
                               join pj in _db.Projects on od.ProjectId equals pj.ProjectId
                               join o in _db.Orders on od.OrderId equals o.OrderId
                               select new OrderDetailDTO
                               {
                                   OrderDetailId = od.OrderDetailId,
                                   OrderId = od.OrderId,
                                   ProjectId = od.ProjectId,
                                   ProductId = od.ProductId,
                                   Count = od.Count,
                                   Price = od.Price,
                                   ProjectName = pj.ProjectName,
                                   OrderDate = o.OrderDate.ToString("yyyy-MM-dd")
                               };
            return orderDetails.ToList();
        }

        // GET api/<DashboardController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DashboardController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DashboardController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DashboardController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
