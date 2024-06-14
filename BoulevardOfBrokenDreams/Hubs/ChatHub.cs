using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BoulevardOfBrokenDreams.Hubs
{
    public class ChatHub : Hub
    {
        // 發送訊息的方法，參數為 Message 物件
        public async Task SendMessage(Message message)
        {
            // 將接收到的訊息廣播給所有連接的客戶端
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        // Message 類別，用於封裝訊息資訊
        public class Message
        {
            public int Id { get; set; }
            public int ServiceId { get; set; }
            public int MemberId { get; set; }
            public int? AdminId { get; set; }
            public string? Content { get; set; }
            public DateTime Timestamp { get; set; }
            public string? Sender { get; set; }
        }
    }
}
