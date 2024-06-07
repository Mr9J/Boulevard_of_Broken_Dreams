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
        private readonly ServicesServiceMessage _serviceMessage; // 使用 ServicesServiceMessage 別名

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
        // 創建新服務
        [HttpPost]
        public async Task<ActionResult<ServiceDTO>> CreateService(ServiceDTO serviceDto)
        {
            var createdService = await _serviceMessage.CreateServiceAsync(serviceDto);
            return Ok(createdService); // 或者使用 CreatedAtRoute 方法
        }


        // 創建服務訊息
        [HttpPost("{serviceId}/messages")]
        public async Task<ActionResult<ServiceMessageDTO>> CreateServiceMessage(int serviceId, ServiceMessageDTO messageDto)
        {
            messageDto.ServiceId = serviceId;
            var createdMessage = await _serviceMessage.CreateServiceMessageAsync(messageDto);
            return CreatedAtAction(nameof(GetMessagesByServiceId), new { serviceId = createdMessage.ServiceId }, createdMessage);
        }

        // 獲取所有客戶的最新訊息
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
    }
}
