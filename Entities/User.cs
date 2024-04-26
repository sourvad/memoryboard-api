namespace MemoryboardAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public byte[] PasswordSalt { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Clipboard Clipboard { get; set; }
    }   
}