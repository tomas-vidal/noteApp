using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace noteApp.backend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        [JsonIgnore] public string Email { get; set; }
        public string Username { get; set; }
        [JsonIgnore] public string Password { get; set; }
        ICollection<Note>? Notes { get; set; }
    }
}
