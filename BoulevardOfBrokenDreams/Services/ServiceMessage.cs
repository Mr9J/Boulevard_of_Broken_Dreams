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

        // 根據會員ID獲取該會員的所有服務
        //返回一個List<ServiceDTO> 方法的參數是 memberId
        public async Task<List<ServiceDTO>> GetServicesByMemberIdAsync(int memberId)
        {
            //從資料庫查詢資料
            return await _context.Services
                //第一個 MemberId自資料庫表中的記錄。第二個 memberId:是參數，用來過濾使其只返回符合這個參數條件的記錄。
                .Where(s => s.MemberId == memberId)
                .Select(s => new ServiceDTO
                {
                    //將查詢資料賦值給DTO
                    ServiceId = s.ServiceId,
                    MemberId = s.MemberId,
                    AdminId = s.AdminId,
                    StatusId = s.StatusId,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                })
                //返回查詢結果
                .ToListAsync();
        }

        // 根據服務ID獲取該服務的所有訊息
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

        // 創建新服務 參數 serviceDto 是一個 ServiceDTO 類型 將傳入的DTO 導入NEW出來的實體 更新資料庫
        public async Task<ServiceDTO> CreateServiceAsync(ServiceDTO serviceDto)
        {
            // 創建新的 Service 實體對象，並設置其屬性值
            var service = new Service
            {
                MemberId = serviceDto.MemberId,
                AdminId = serviceDto.AdminId,
                StatusId = serviceDto.StatusId,
                StartDate = serviceDto.StartDate,
                EndDate = serviceDto.EndDate
            };

            // 將新創建的 Service 對象添加到資料庫上下文的 Services 集合中
            _context.Services.Add(service);

            // 異步保存所有對資料庫上下文的更改到資料庫
            await _context.SaveChangesAsync();

            // 將資料庫生成的 ServiceId 設置到 DTO 中
            serviceDto.ServiceId = service.ServiceId;

            return serviceDto;
        }


        // 創建服務訊息
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

            // 更新服務狀態 FindAsyn 用來根據主鍵（Primary Key）查找實體對象
            var service = await _context.Services.FindAsync(messageDto.ServiceId);
            if (service != null)
            {
                //根據 messageDto.ServiceId 查找對應的服務實體對象。
                //如果找到該服務 AdminId 為null，表示訊息來自客戶，則狀態設置為4（等待客服回應中）；否則設置為5（客服已回應）。
                service.StatusId = messageDto.AdminId == null ? 4 : 5; 
            }

            await _context.SaveChangesAsync();

            messageDto.MessageId = message.MessageId;
            messageDto.MessageDate = message.MessageDate; // 確保返回的 DTO 中的時間也是本地時間

            return messageDto;
        }

        // 獲取所有客戶的最新訊息
        public async Task<List<CustomerMessageDTO>> GetLatestMessagesAsync()
        {
            var groupedMessages = await _context.ServiceMessages
                //將MEMBER依照id分組
                .GroupBy(m => m.MemberId)
                //對每個分組應用 Select 方法
                .Select(g => new
                {
                    MemberId = g.Key,
                    //降序排序，然後使用 FirstOrDefault 方法獲取最新的訊息
                    LatestMessage = g.OrderByDescending(m => m.MessageDate).FirstOrDefault(),
                    MessageCount = g.Count(),
                    UnreadMessages = g.Count(m => !m.IsRead && m.AdminId == null) // 只計算由客戶發出且admin未讀的消息
                })
                .ToListAsync();
            //用於將每個分組元素轉換為一個 CustomerMessageDTO 對象
            var result = groupedMessages.Select(x => new CustomerMessageDTO
            {
                ServiceId = x.LatestMessage.ServiceId,
                MemberId = x.MemberId,
                AdminId = x.LatestMessage.AdminId,
                MessageContent = x.LatestMessage.MessageContent,
                MessageDate = x.LatestMessage.MessageDate,
                MessageCount = x.MessageCount,
                UnreadMessages = x.UnreadMessages // 正確反映未讀訊息數量
            }).ToList();

            return result;
        }

        // 關閉服務
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

        // 根據會員ID獲取該會員最新的服務ID
        public async Task<int?> GetLatestServiceIdByMemberIdAsync(int memberId)
        {
            var latestService = await _context.ServiceMessages
                .Where(m => m.MemberId == memberId)
                .OrderByDescending(m => m.MessageDate)
                .Select(m => m.ServiceId)
                .FirstOrDefaultAsync();

            return latestService;
        }
    }
}
