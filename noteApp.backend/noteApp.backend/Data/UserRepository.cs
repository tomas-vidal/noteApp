using noteApp.backend.Dtos;
using noteApp.backend.Models;

namespace noteApp.backend.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly dbContext _userContext;
        public UserRepository(dbContext userContext) 
        {
            _userContext = userContext;
        }
        public void Create(User user)
        {
            _userContext.Add(user);
            _userContext.SaveChanges();
        }

        public User? GetById(Guid id)
        {
            return _userContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetByUsername(string username)
        {
            return _userContext.Users.FirstOrDefault(u => u.Username == username);
        }
    }
}
