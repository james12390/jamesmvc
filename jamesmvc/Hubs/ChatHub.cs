using Microsoft.AspNetCore.SignalR;
using jamesmvc.Data;
using jamesmvc.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
namespace jamesmvc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ChatHub(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task JoinRoom(string userId)
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task SendPrivateMessage(string senderId, string receiverId, string message)
        {
            var sender = await _userManager.FindByIdAsync(senderId);
            var receiver = await _userManager.FindByIdAsync(receiverId);

            if (sender == null || receiver == null)
            {
                // 不存在就不處理
                return;
            }
            var chat = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Timestamp = DateTime.Now
            };

            _context.ChatMessages.Add(chat);
            await _context.SaveChangesAsync();

            await Clients.Group(receiverId).SendAsync("ReceiveMessage", senderId, message);
            await Clients.Group(senderId).SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
