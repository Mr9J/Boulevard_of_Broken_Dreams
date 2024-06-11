using BoulevardOfBrokenDreams.DataAccess;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using ServicesServiceMessage = BoulevardOfBrokenDreams.Services.ServiceMessage; // 設置別名
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly MumuDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MemberRepository _memberRepository;
        private readonly ServicesServiceMessage _serviceMessage;

        public ServiceController(MumuDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ServicesServiceMessage serviceMessage)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _memberRepository = new MemberRepository(_context);
            _serviceMessage = serviceMessage;
        }

        // 根據會員ID獲取該會員的所有服務
        [HttpGet("member/{memberId}/services")]
        public async Task<ActionResult<List<ServiceDTO>>> GetServicesByMemberId(int memberId)
        {
            var services = await _serviceMessage.GetServicesByMemberIdAsync(memberId);
            return Ok(services);
        }

        // 根據服務ID獲取該服務的所有訊息
        [HttpGet("service/{serviceId}/messages")]
        public async Task<ActionResult<List<ServiceMessageDTO>>> GetMessagesByServiceId(int serviceId)
        {
            var messages = await _serviceMessage.GetMessagesByServiceIdAsync(serviceId);
            return Ok(messages);
        }

        // 創建新服務
        [HttpPost]
        public async Task<ActionResult<ServiceDTO>> CreateService(ServiceDTO serviceDto)
        {
            var createdService = await _serviceMessage.CreateServiceAsync(serviceDto);
            return Ok(createdService);
        }

        // 創建服務訊息
        [HttpPost("{serviceId}/messages")]
        public async Task<ActionResult<ServiceMessageDTO>> CreateServiceMessage(int serviceId, ServiceMessageDTO messageDto)
        {
            messageDto.ServiceId = serviceId; 
            var createdMessage = await _serviceMessage.CreateServiceMessageAsync(messageDto);
            return CreatedAtAction(nameof(GetMessagesByServiceId), new { serviceId = createdMessage.ServiceId }, createdMessage);
        }

        //獲取所有客戶的最新訊息
        [HttpGet("latest-messages")]
        public async Task<ActionResult<List<CustomerMessageDTO>>> GetLatestMessages()
        {
            var latestMessages = await _serviceMessage.GetLatestMessagesAsync();
            return Ok(latestMessages);
        }

        // 獲取某個會員的所有訊息
        [HttpGet("member/{memberId}/messages")]
        public async Task<ActionResult<List<ServiceMessageDTO>>> GetMessagesByMemberId(int memberId)
        {
            var messages = await _context.ServiceMessages
                .Where(m => m.MemberId == memberId)
                .ToListAsync();

            return Ok(messages);
        }

        // 根據會員ID獲取該會員最新的服務ID
        [HttpGet("member/{memberId}/latest-service-id")]
        public async Task<ActionResult<int?>> GetLatestServiceIdByMemberId(int memberId)
        {
            var latestServiceId = await _serviceMessage.GetLatestServiceIdByMemberIdAsync(memberId);
            return Ok(latestServiceId);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var unreadCount = await _context.ServiceMessages.CountAsync(m => !m.IsRead);
            return Ok(unreadCount);
        }
        [HttpGet("unread-count/{memberId}")]
        public async Task<IActionResult> GetUnreadCountForMember(int memberId)
        {
            var unreadCount = await _context.ServiceMessages
                .Where(m => m.MemberId == memberId && !m.IsRead)
                .CountAsync();

            return Ok(unreadCount);
        }
        [HttpPost("mark-as-read/{memberId}")]
        public async Task<IActionResult> MarkMessagesAsReadForMember(int memberId)
        {
            var messages = await _context.ServiceMessages
                .Where(m => m.MemberId == memberId && !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("members/nicknames")]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetMembersNicknames()
        {
            var members = await _context.Members
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    Nickname = m.Nickname
                })
                .ToListAsync();

            return Ok(members);
        }
        [HttpPut("close/{serviceId}")] // 使用PUT因為這是一個更新操作
        public async Task<IActionResult> CloseService(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            service.StatusId = 6; // 假設6代表服務已結束
            service.EndDate = DateTime.UtcNow; // 使用UTC時間以避免時區問題
            await _context.SaveChangesAsync();

            return NoContent(); // 回應204 No Content表示更新成功
        }
    }
}
