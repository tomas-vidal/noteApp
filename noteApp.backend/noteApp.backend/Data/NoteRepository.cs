using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using noteApp.backend.Helpers;
using noteApp.backend.Models;

namespace noteApp.backend.Data
{
    public class NoteRepository : INoteRepository
    {
        private readonly dbContext _noteContext;

        public NoteRepository(dbContext noteContext)
        {
            _noteContext = noteContext;
        }
        public void Create(Note note)
        {
            _noteContext.Notes.Add(note);

            _noteContext.SaveChanges();
        }

        public void Update(int id, string title, string content, string tag)
        {
            Note note = _noteContext.Notes.First(note => note.Id == id);
            note.Title = title;
            note.Content = content;
            note.Tag = tag;

            _noteContext.SaveChanges();
        }
        public void Delete(int noteId)
        {
            Note? delete = GetById(noteId);
            if (delete != null)
            {
                _noteContext.Notes.Remove(delete);
            }
            _noteContext.SaveChanges();
        }

        public ICollection<Note> GetByUserId(Guid userId)
        {
            return _noteContext.Notes.Where(note => note.UserId == userId).ToList();
        }

        public Note? GetById(int id)
        {
            return _noteContext.Notes.FirstOrDefault(note => note.Id == id);
        }
    }
}
