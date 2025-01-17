using noteApp.backend.Dtos;
using noteApp.backend.Models;

namespace noteApp.backend.Data
{
    public interface IUserRepository
    {
        User? GetById(Guid id);
        User? GetByUsername(string username);
        void Create(User user);
    }
}
