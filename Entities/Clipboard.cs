namespace MemoryboardAPI.Entities
{
    public class Clipboard
    {
        public int Id { get; set; }
        public List<string> Items { get; set; } = new();
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
