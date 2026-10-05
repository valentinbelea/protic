namespace DocumentPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Clear text password per requirements
        public string Role { get; set; } = "User"; // e.g. "Admin", "User"
    }
}
