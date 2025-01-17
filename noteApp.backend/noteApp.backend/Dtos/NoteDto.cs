namespace noteApp.backend.Dtos
{
    public class NoteDto
    {
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
