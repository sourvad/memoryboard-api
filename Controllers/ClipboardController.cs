using MemoryboardAPI.Data;
using MemoryboardAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MemoryboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClipboardController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // api/clipboard
        [HttpGet]
        public async Task<ActionResult<List<byte[]>>> GetUserClipboard()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return BadRequest("Invalid user details");
            }

            int userIdInt = int.Parse(userId);
            Clipboard userClipboard = await _context.Clipboards.FirstOrDefaultAsync(c => c.UserId == userIdInt);

            return Ok(userClipboard.Items);
        }
    }
}
