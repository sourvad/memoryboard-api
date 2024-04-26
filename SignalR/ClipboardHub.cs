using MemoryboardAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MemoryboardAPI.SignalR
{
    [Authorize]
    public class ClipboardHub(AppDbContext context) : Hub
    {
        private readonly AppDbContext _context = context;

        public async Task Copy(string copiedText)
        {
            var userId = Context.UserIdentifier;
            var userIdInt = int.Parse(userId);
            var userClipboard = await _context.Clipboards.FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (userClipboard.Items.Count == 50)
            {
                userClipboard.Items.RemoveAt(49);
            }

            userClipboard.Items.Insert(0, copiedText);

            await _context.SaveChangesAsync();
            await Clients.OthersInGroup(userId).SendAsync("BroadcastCopy", copiedText);
        }

        public async Task Select(int selectedIndex)
        {
            var userId = Context.UserIdentifier;
            var userIdInt = int.Parse(userId);
            var userClipboard = await _context.Clipboards.FirstOrDefaultAsync(c => c.UserId == userIdInt);
            var selectedText = userClipboard.Items[selectedIndex];

            userClipboard.Items.RemoveAt(selectedIndex);
            userClipboard.Items.Insert(0, selectedText);

            await _context.SaveChangesAsync();
            await Clients.OthersInGroup(userId).SendAsync("BroadcastSelect", selectedIndex);
        }

        public async Task ClearAll()
        {
            var userId = Context.UserIdentifier;
            var userIdInt = int.Parse(userId);
            var userClipboard = await _context.Clipboards.FirstOrDefaultAsync(c => c.UserId == userIdInt);

            userClipboard.Items.Clear();

            await _context.SaveChangesAsync();
            await Clients.OthersInGroup(userId).SendAsync("BroadcastClearAll");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
