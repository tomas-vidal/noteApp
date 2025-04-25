using noteApp.backend.Models;

namespace noteApp.backend.Data
{
    public interface INoteRepository
    {
        ICollection<Note> GetByUserId(Guid userId);
        void Delete(int id);
        void Update(int noteId, string title, string content, string tag);
        void Create(Note note);
        Note? GetById(int id);
    }
}
