using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BoulevardOfBrokenDreams.Services
{
    public class ServiceMessage
    {
        private readonly MumuDbContext _context;

        public ServiceMessage(MumuDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceDTO>> GetServicesAsync()
        {
            return await _context.Services
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    MemberId = s.MemberId,
                    AdminId = s.AdminId,
                    StatusId = s.StatusId,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                })
                .ToListAsync();
        }

        public async Task<ServiceDTO?> GetServiceByIdAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return null;
            }

            return new ServiceDTO
            {
                ServiceId = service.ServiceId,
                MemberId = service.MemberId,
                AdminId = service.AdminId,
                StatusId = service.StatusId,
                StartDate = service.StartDate,
                EndDate = service.EndDate
            };
        }

        public async Task<ServiceDTO> CreateServiceAsync(ServiceDTO serviceDto)
        {
            var service = new Service
            {
                MemberId = serviceDto.MemberId,
                AdminId = serviceDto.AdminId,
                StatusId = serviceDto.StatusId,
                StartDate = serviceDto.StartDate,
                EndDate = serviceDto.EndDate
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            serviceDto.ServiceId = service.ServiceId;
            return serviceDto;
        }

        public async Task<ServiceMessageDTO> CreateServiceMessageAsync(ServiceMessageDTO messageDto)
        {
            if (string.IsNullOrEmpty(messageDto.MessageContent))
            {
                throw new ArgumentException("Message content cannot be null or empty", nameof(messageDto.MessageContent));
            }

            var message = new Models.ServiceMessage
            {
                ServiceId = messageDto.ServiceId,
                MemberId = messageDto.MemberId,
                AdminId = messageDto.AdminId,
                MessageContent = messageDto.MessageContent ?? string.Empty,
                MessageDate = DateTime.Now // 使用本地時間
            };

            _context.ServiceMessages.Add(message);

            // 更新服務狀態
            var service = await _context.Services.FindAsync(messageDto.ServiceId);
            if (service != null)
            {
                service.StatusId = messageDto.AdminId == null ? 4 : 5; // 更新狀態
            }

            await _context.SaveChangesAsync();

            messageDto.MessageId = message.MessageId;
            messageDto.MessageDate = message.MessageDate; // 確保返回的 DTO 中的時間也是本地時間

            return messageDto;
        }

        public async Task<List<ServiceMessageDTO>> GetServiceMessagesAsync(int serviceId)
        {
            return await _context.ServiceMessages
                .Where(m => m.ServiceId == serviceId)
                .Select(m => new ServiceMessageDTO
                {
                    MessageId = m.MessageId,
                    ServiceId = m.ServiceId,
                    MemberId = m.MemberId,
                    AdminId = m.AdminId,
                    MessageContent = m.MessageContent,
                    MessageDate = m.MessageDate
                })
                .ToListAsync();
        }

        public async Task<List<CustomerMessageDTO>> GetLatestMessagesAsync()
        {
            var groupedMessages = await _context.ServiceMessages
                .GroupBy(m => m.MemberId)
                .Select(g => new
                {
                    MemberId = g.Key,
                    LatestMessage = g.OrderByDescending(m => m.MessageDate).FirstOrDefault(),
                    MessageCount = g.Count()
                })
                .ToListAsync();

            var result = groupedMessages.Select(x => new CustomerMessageDTO
            {
                ServiceId = x.LatestMessage.ServiceId,
                MemberId = x.LatestMessage.MemberId,
                AdminId = x.LatestMessage.AdminId,
                MessageContent = x.LatestMessage.MessageContent,
                MessageDate = x.LatestMessage.MessageDate,
                MessageCount = x.MessageCount
            }).ToList();

            return result;
        }

        public async Task CloseServiceAsync(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service != null)
            {
                service.StatusId = 6; // 更新狀態為客服回復完畢
                service.EndDate = DateTime.Now; // 記錄結束時間
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int?> GetLatestServiceIdByMemberIdAsync(int memberId)
        {
            var latestService = await _context.ServiceMessages
                .Where(m => m.MemberId == memberId)
                .OrderByDescending(m => m.MessageDate)
                .Select(m => m.ServiceId)
                .FirstOrDefaultAsync();

            return latestService;
        }

        public async Task<List<ServiceDTO>> GetServicesByMemberIdAsync(int memberId)
        {
            return await _context.Services
                .Where(s => s.MemberId == memberId)
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    MemberId = s.MemberId,
                    AdminId = s.AdminId,
                    StatusId = s.StatusId,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                })
                .ToListAsync();
        }

        public async Task<List<ServiceMessageDTO>> GetMessagesByServiceIdAsync(int serviceId)
        {
            return await _context.ServiceMessages
                .Where(m => m.ServiceId == serviceId)
                .Select(m => new ServiceMessageDTO
                {
                    MessageId = m.MessageId,
                    ServiceId = m.ServiceId,
                    MemberId = m.MemberId,
                    AdminId = m.AdminId,
                    MessageContent = m.MessageContent,
                    MessageDate = m.MessageDate
                })
                .ToListAsync();
        }

    }
}
