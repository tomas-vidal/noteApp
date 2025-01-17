namespace noteApp.backend.Dtos
{
    public class UserDto
    {
        public string? Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
