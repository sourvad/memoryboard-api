using MemoryboardAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace MemoryboardAPI.SignalR
{
    [Authorize]
    public class ClipboardHub(AppDbContext context) : Hub
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _copyLocks = [];

        private readonly AppDbContext _context = context;

        public async Task Copy(byte[] encryptedBytes)
        {
            var userId = Context.UserIdentifier;
            var userIdInt = int.Parse(userId);
            var userClipboard = await _context.Clipboards.FirstOrDefaultAsync(c => c.UserId == userIdInt);

            var userSemaphore = _copyLocks.GetOrAdd(userId, _ =>  new SemaphoreSlim(1, 1));

            await userSemaphore.WaitAsync();

            try
            {

                if (userClipboard.Items.Count == 0 || !userClipboard.Items[0].SequenceEqual(encryptedBytes))
                {
                    if (userClipboard.Items.Count == 50)
                    {
                        userClipboard.Items.RemoveAt(49);
                    }

                    userClipboard.Items.Insert(0, encryptedBytes);

                    await _context.SaveChangesAsync();
                    await Clients.OthersInGroup(userId).SendAsync("BroadcastCopy", encryptedBytes);
                }
            }
            finally
            {
                userSemaphore.Release();
            }
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
