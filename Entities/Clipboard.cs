namespace MemoryboardAPI.Entities
{
    public class Clipboard
    {
        public int Id { get; set; }
        public List<byte[]> Items { get; set; } = [];
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
