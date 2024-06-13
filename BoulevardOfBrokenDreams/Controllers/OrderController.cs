using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Web;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
       
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static bool _paymentResponseReceived = false;
        private static readonly SemaphoreSlim _paymentResponseLock = new SemaphoreSlim(1);

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
        [HttpPost("CalculateChecksum")]
    public IActionResult CalculateChecksum([FromBody] Dictionary<string, string> data)
    {

        var param = data.Keys.OrderBy(x => x).Select(key => key + "=" + data[key]).ToList();
        var checkValue = string.Join("&", param);
        //測試用的 HashKey
        var hashKey = "pwFHCqoQZGmho4w6";
        //測試用的 HashIV
        var HashIV = "EkRm7iFT261dpevs";
        checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
        checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
        checkValue = GetSHA256(checkValue);
       string  checksum = checkValue.ToUpper();
        return Ok(new { Checksum = checksum });



    }

    private string GetSHA256(string value)
    {
        var result = new StringBuilder();
        var sha256 = SHA256.Create();
        var bts = Encoding.UTF8.GetBytes(value);
        var hash = sha256.ComputeHash(bts);
        for (int i = 0; i < hash.Length; i++)
        {
            result.Append(hash[i].ToString("X2"));
        }
        return result.ToString();

    }


        private async Task WaitForPaymentResponse()
        {
            while (true)
            {
                await _paymentResponseLock.WaitAsync();
                try
                {
                    if (_paymentResponseReceived)
                    {
                        return;
                    }
                }
                finally
                {
                    _paymentResponseLock.Release();
                }
                await Task.Delay(100); // 每次等待 500 毫秒
            }
        }

        [HttpPost("ECPayResponseMessage")]
        public async Task<IActionResult> ECPayResponseMessage([FromForm] Dictionary<string, string> requestData)
        {
            await _paymentResponseLock.WaitAsync();
            try
            {
                bool isSuccess = true;
                _paymentResponseReceived = isSuccess;

                return Ok("1|OK");
            }
            finally
            {
                _paymentResponseLock.Release();
            }
        }

   [HttpPost("CreateOrder")]
        public async Task<string> CreateOrder([FromBody] CreateOrderDTO orderDTO)
        {
           //await WaitForPaymentResponse();

            try
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

                //從購物車中尋找是否有符合的商品，如果有就對該購物車商品進行數量修改
                var memberCartId = _db.Carts.FirstOrDefault(m => m.MemberId == orderDTO.MemberId)?.CartId;
                if (memberCartId == 0)
                    return "找不到使用者購物車";
                orderDTO.ProductData.ForEach(product =>
                {

                    int productId = int.Parse(product.ProductId);
                    var cartHasProduct = _db.CartDetails.FirstOrDefault(c => c.CartId == memberCartId && c.ProductId == productId);
                    if (cartHasProduct != null)
                    {
                        cartHasProduct.Count -= product.Count;

                        // 如果 product.Count 大於購物車中的產品數量，則刪除該產品
                        if (cartHasProduct.Count <= 0)
                        {
                            _db.CartDetails.Remove(cartHasProduct);
                        }
                    }

                });
                _db.SaveChanges();

                return "訂單完成";
            }
            catch (Exception ex)
            {
                // 处理异常
                return "訂單失敗";
            }
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
                                 Thumbnail = o.Member.Thumbnail,
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
        private string decodeJwtId(string jwt)
        {
            jwt = jwt.Replace("Bearer ", "");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(jwt);

            string? id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            return id!;
        }

        [HttpGet("UserOrder/list")]
        public IEnumerable<ProjectDTO> GetUserOrderList()
        {
            string? jwt = HttpContext.Request.Headers["Authorization"];

            if (jwt == null || jwt == "") return null;

            string id = decodeJwtId(jwt);
            int mId = int.Parse(id);

            var ProjectDTO = from p in _db.Projects.Where(p=>p.MemberId == mId)
                             select new ProjectDTO
                             {
                                 ProjectId = p.ProjectId,
                                 ProjectName = p.ProjectName,
                                 GroupId = p.GroupId,
                                 StatusId = p.StatusId,
                                 Thumbnail = p.Thumbnail,
                                 ProjectDescription = p.ProjectDescription,
                                 OrderCount = (from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.Count).Sum(),
                                 SponsorCount = (from orderDetail in _db.OrderDetails
                                                 where orderDetail.ProjectId == p.ProjectId
                                                 select orderDetail.OrderId).Count(),
                             };
            return ProjectDTO;
        }
    }
}