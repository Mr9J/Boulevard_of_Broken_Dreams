using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

        [HttpGet("{memberId}")]
        public async Task<ActionResult<PurchasehistoryDTO>> GetPurchasHistory(int memberId)
        {
            var orders = await _db.Orders
                .Where(m => m.MemberId == memberId)
                .OrderByDescending(d => d.OrderDate)
                .Include(od => od.OrderDetails)
                .ThenInclude(od => od.Product) // Assuming there is a Product navigation property
                .Include(od => od.OrderDetails)
                .ThenInclude(od => od.Project) // Assuming there is a Project navigation property
                .Include(o=>o.Coupon)
                .ToListAsync();


            var projectCards = orders.Select(order => new
            {
                Donate = order.Donate,
                OrderDate = order.OrderDate.ToString("yyyy年M月d日 HH:mm", CultureInfo.GetCultureInfo("zh-TW")),
                Discount =order.Coupon !=null ? order.Coupon.Discount : 0,
                Projects = order.OrderDetails   
                    .GroupBy(od => od.Project)
                    .Select(g => new ProjectCardDTO
                    {
                        ProjectId = g.Key.ProjectId,
                        ProjectName = g.Key.ProjectName,
                        Thumbnail = g.Key.Thumbnail,
                        Products = g.Select(od => new ProductCardDTO
                        {
                            ProductName = od.Product.ProductName,
                            ProductPrice = od.Product.ProductPrice,
                            ProductId = od.Product.ProductId,
                            Thumbnail = od.Product.Thumbnail,
                            Count = od.Count
                        }).ToList()
                    }).ToList()
            }).ToList();



            return Ok(projectCards);
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
