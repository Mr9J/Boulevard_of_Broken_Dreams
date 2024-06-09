using BoulevardOfBrokenDreams.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly MumuDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CartController(MumuDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        //仔入購物車頁面
        [HttpGet("{memberId}")]
        public async Task<ActionResult<IEnumerable<CartDetailDTO>>> GetCartsDetailData(int memberId)
        {//還沒寫:如果購物車沒有資料 顯示為空頁面
            try
            {
                var path = "https://" + httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/";

                var memberCart = await context.Carts.FirstOrDefaultAsync(m => m.MemberId == memberId);
                if (memberCart == null)
                {
                    return NotFound(); // 如果找不到對應的會員購物車，返回404
                }

                var cartDetailsData = await context.CartDetails
                    .Where(cd => cd.CartId == memberCart.CartId)
                    .GroupBy(cd => cd.ProjectId) // 按照 ProjectId 分組
                    .Select(group => new
                    {
                        ProjectId = group.Key, // 每個分組的 ProjectId
                        ProductIds = group.Select(cd => cd.ProductId).ToList(), // 每個 ProjectId 對應的 ProductIds
                                                                                //ProductCount = group.Select(cd => cd.ProductId)
                    })
                    .ToListAsync();

                List<CartDetailDTO> dataList = new List<CartDetailDTO>(); // 定義一個列表來存儲結果
                foreach (var detailData in cartDetailsData)
                {
                    //var count = detailData.ProductCount;
                    var data = await context.Projects
                        .Where(x => x.ProjectId == detailData.ProjectId)
                        .Include(p => p.Products)
                        .Select(p => new CartDetailDTO
                        {
                            ProjectId = p.ProjectId,
                            ProjectName = p.ProjectName,
                            Thumbnail = path + p.Thumbnail,
                            Products = p.Products
                                .Join(context.CartDetails,
                      product => product.ProductId,
                      cartDetail => cartDetail.ProductId,
                      (product, cartDetail) => new { Product = product, CartDetail = cartDetail })
                .Where(joined => detailData.ProductIds.Contains(joined.Product.ProductId))
                .Select(joined => new ProductDataInCartDTO
                {
                    ProductId = joined.Product.ProductId,
                    ProductName = joined.Product.ProductName,
                    ProductPrice = joined.Product.ProductPrice,
                    CurrentStock = joined.Product.CurrentStock,
                    Thumbnail = path + joined.Product.Thumbnail,
                    Count = joined.CartDetail.Count
                })
                .ToList()
                          
                        })
                        .FirstOrDefaultAsync(); // 在這裡使用FirstOrDefaultAsync而不是ToListAsync

                    dataList.Add(data); // 將每個結果添加到列表中
                }

                return Ok(dataList); // 返回整個列表
            }
            catch (Exception ex)
            {
                // 記錄異常信息
                return StatusCode(500); // 返回500表示內部服務器錯誤
            }
        }


        [HttpPost("{productId}/{projectId}/{memberId}")]
        //加入購物車
        public async Task<IActionResult> AddToCart(int productId, int projectId, int memberId)
        {

            var user = context.Carts.FirstOrDefault(m => m.MemberId == memberId);

            if (user == null)
            {
                //如果找不到使用者沒有CartId 幫他建
                var newCart = new Cart
                {
                    MemberId = memberId,
                };
                context.Carts.Add(newCart);
                await context.SaveChangesAsync();

                int newCartId = newCart.CartId;
                decimal price = context.Products.FirstOrDefault(pt => pt.ProductId == productId)?.ProductPrice ?? 0;

                var newCartDetail = new CartDetail
                {
                    CartId = newCartId,
                    ProjectId = projectId,
                    ProductId = productId,
                    Count = 1,
                    Price = price,
                    StatusId = 1
                };

                context.CartDetails.Add(newCartDetail);

            }
            //如果有Id
            else
            {
                var memberCart = context.Carts.FirstOrDefault(m => m.MemberId == memberId);
                decimal price = context.Products.FirstOrDefault(pt => pt.ProductId == productId)?.ProductPrice ?? 0;
                var alreadyinCart = context.CartDetails.FirstOrDefault(pt => pt.ProductId == productId && pt.CartId == memberCart.CartId);
                if (alreadyinCart != null)
                {
                    alreadyinCart.Count += 1;
                }
                //if (price == 0)
                //{ return BadRequest(); }
                else
                {
                    var addCartDetail = new CartDetail
                    {
                        CartId = memberCart.CartId,
                        ProjectId = projectId,
                        ProductId = productId,
                        Count = 1,
                        Price = price,
                        StatusId = 1
                    };

                    context.CartDetails.Add(addCartDetail);
                }
            }




            await context.SaveChangesAsync();

            return Ok();
        }

        //增減購物車
        [HttpPut("{productId}/{memberId}/{incrementOrdecrement}")]
        public async Task<IActionResult> PutProductFromCart(int productId, int memberId , string incrementOrdecrement)
        {
            var user = await context.Carts.FirstOrDefaultAsync(m => m.MemberId == memberId);
            if (user == null)
            {
                //如果找不到使用者
                return NotFound();
            }

            var userCartCartDetail = await context.CartDetails.FirstOrDefaultAsync(cid => cid.CartId == user.CartId && cid.ProductId == productId);
            if (userCartCartDetail == null)
            {
                return NotFound();
            }
            if(incrementOrdecrement == "Increment")
            {
                userCartCartDetail.Count++;
            }
            else if (incrementOrdecrement == "Decrement")
            {
                if (userCartCartDetail.Count > 0)
                {
                    userCartCartDetail.Count--; // 减少数量，确保不会减为负值
                }
            }

            await context.SaveChangesAsync();
            return Ok();



        }


        //刪除購物車
        //DELETE api/<ProjectController>/5
        [HttpDelete("{productId}/{memberId}")]
        public async Task<IActionResult> DeleteProductFromCart(int productId, int memberId)
        {
            var user = await context.Carts.FirstOrDefaultAsync(m => m.MemberId == memberId);
            if (user == null)
            {
                //如果找不到使用者
                return NotFound();
            }

            var userCartCartDetail = await context.CartDetails.FirstOrDefaultAsync(cid => cid.CartId == user.CartId && cid.ProductId == productId);
            if (userCartCartDetail == null)
            {
                return NotFound();
            }

            context.CartDetails.Remove(userCartCartDetail);
            await context.SaveChangesAsync();
            return Ok();



        }
    }
}
