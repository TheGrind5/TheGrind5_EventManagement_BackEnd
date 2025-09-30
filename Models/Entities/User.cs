namespace TheGrind5_EventManagement.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Password { get; set; } //Bắt buộc
        public string? Role {  get; set; } //Có thể null
    }
}
