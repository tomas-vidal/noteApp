using System.Text.Json.Serialization;

namespace noteApp.backend.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string? Title {  get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate {  get; set; }
        public string Tag { get; set; } = string.Empty;
        [JsonIgnore] public Guid UserId { get; set; }
    }
}
