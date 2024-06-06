using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Mumu.Models.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(MumuDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return _db.Orders;
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OrderController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("CreateOrder")]
        public string CreateOrder([FromBody] CreateOrderDTO orderDTO)
        {
            var newOrder = new Order
            {
                OrderDate = DateTime.Now,
                MemberId = orderDTO.MemberId,
                ShipDate = DateTime.Now.AddDays(7),
                ShipmentStatusId = 1,
                PaymentMethodId = orderDTO.PaymentMethodId,
                PaymentStatusId = 1,
                Donate = orderDTO.Donate
            };

            _db.Orders.Add(newOrder);
            _db.SaveChanges();
            //取得剛新增的OrderID
            int orderId = newOrder.OrderId;
            orderDTO.ProductData.ForEach(product =>
            {
                if (product.Count == 0)
                { return; }
                //productId迭代price
                int productId = int.Parse(product.ProductId);
                decimal price = _db.Products.FirstOrDefault(od => od.ProductId == productId)?.ProductPrice ?? 0;

                decimal total = price * product.Count;

                var newOrderDetails = new OrderDetail
                {
                    OrderId = orderId,
                    ProjectId = orderDTO.ProjectId,
                    ProductId = productId,
                    Count = product.Count,
                    Price = total
                };

                _db.OrderDetails.Add(newOrderDetails);
            });
            _db.SaveChanges();
            return "代單完成";
        }


        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return "damedesuyo";
        }
        [HttpGet("list")]
        public IEnumerable<ProjectDTO> GetOrderList()
        {
            var ProjectDTO = from p in _db.Projects
                             select new ProjectDTO
                             {
                                 ProjectId = p.ProjectId,
                                 ProjectName = p.ProjectName,
                                 GroupId = p.GroupId,
                                 StatusId= p.StatusId,
                                 Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,
                                 OrderCount = (from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.Count).Sum(),
                                 SponsorCount = (from orderDetail in _db.OrderDetails
                                                 where orderDetail.ProjectId == p.ProjectId
                                                 select orderDetail.OrderId).Count(),
                             };
            return ProjectDTO;
        }

        [HttpGet("Project/{projectId}")]
        public IActionResult GetByProjectId(int projectId)
        {
            if (!_db.OrderDetails.Any(p => p.ProjectId == projectId))
            {
                return NotFound("No projects found for the given member ID.");
            }
            var orders = from o in _db.Orders
                         join od in _db.OrderDetails on o.OrderId equals od.OrderId
                         join p in _db.Projects on od.ProjectId equals p.ProjectId
                         where od.ProjectId == projectId
                         select new OrderDTO
                         {
                             OrderId = o.OrderId,
                             OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                             MemberId = o.MemberId,
                             ShipDate = o.ShipDate,
                             ShipmentStatusId = o.ShipmentStatusId,
                             PaymentMethodId = o.PaymentMethodId,
                             PaymentStatusId = o.PaymentStatusId,
                             Donate = o.Donate,
                             Member = new MemberDTO
                             {
                                 MemberId = o.Member.MemberId,
                                 Username = o.Member.Username,
                                 Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/members_Thumbnail/" + o.Member.Thumbnail,
                             },
                             OrderDetails =new OrderDetailDTO
                             {
                                 OrderDetailId = od.OrderDetailId,
                                 OrderId = od.OrderId,
                                 ProjectId = od.ProjectId,
                                 ProductId = od.ProductId,
                                 ProjectName = (from projects in _db.Projects
                                                 where projects.ProjectId == od.ProjectId
                                                select projects.ProjectName).FirstOrDefault(),
                                 Count = od.Count,
                                 Price = od.Price
                             },
                             //ProjectId = projectId
                         };

            return Ok(orders.ToList());
        }
    }
}