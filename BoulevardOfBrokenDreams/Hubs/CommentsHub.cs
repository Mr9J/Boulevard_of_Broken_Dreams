using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.SignalR;
namespace BoulevardOfBrokenDreams.Hubs
{
    public class CommentsHub:Hub
    {
        public async Task SendMessage(CommentDto comment)
        {
            await Clients.All.SendAsync("ReceiveComment", comment);
        }
    }
}
