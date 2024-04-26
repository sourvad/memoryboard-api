using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MemoryboardAPI.Data;
using MemoryboardAPI.Entities;
using MemoryboardAPI.DTOs;
using MemoryboardAPI.Services;

namespace MemoryboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(AppDbContext context, PasswordService passwordService, TokenService tokenService) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly PasswordService _passwordService = passwordService;
        private readonly TokenService _tokenService = tokenService;

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterDto userRegistrationDto)
        {
            User user = await FindUserByEmail(userRegistrationDto.Email);

            if (user != default)
            {
                return BadRequest("User already exists");
            }

            byte[] salt = _passwordService.GenerateSalt();
            string passwordHash = _passwordService.HashPassword(userRegistrationDto.Password, salt);

            User newUser = new()
            {
                Email = userRegistrationDto.Email.ToLower(),
                PasswordSalt = salt,
                PasswordHash = passwordHash
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            Clipboard newClipboard = new()
            {
                UserId = newUser.Id,
                Items = []
            };

            _context.Clipboards.Add(newClipboard);
            await _context.SaveChangesAsync();
            
            string token = _tokenService.GenerateToken(newUser.Id);

            return Ok(token);
        }

        // api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            User user = await FindUserByEmail(loginDto.Email);

            if (user == default) 
            {
                return BadRequest("Invalid username or password");
            }

            string hashedPassord = _passwordService.HashPassword(loginDto.Password, user.PasswordSalt);

            if (user.PasswordHash != hashedPassord)
            {
                return BadRequest("Invalid username or password");
            }

            string token = _tokenService.GenerateToken(user.Id);
            
            return Ok(token);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons", Justification = "EF Error")]
        private async Task<User> FindUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }
    }
}
