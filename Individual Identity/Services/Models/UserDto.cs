namespace Individual_Identity.Services.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public IList<string> Roles { get; set; }
        public string Token { get; set; }
    }
}
