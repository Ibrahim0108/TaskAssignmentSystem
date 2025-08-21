using TaskAssignmentSystem.Models.Users;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly List<User> _users = new();
        private int _nextId = 1;

        public AuthService()
        {
            // Seed some demo users so you can test immediately
            _users.Add(new User { Id = _nextId++, Username = "admin", Password = "admin123", FullName = "System Admin", Role = Role.Admin });
            _users.Add(new User { Id = _nextId++, Username = "teacher", Password = "teach123", FullName = "Ms. Sharma", Role = Role.Teacher });
            _users.Add(new User { Id = _nextId++, Username = "student", Password = "stud123", FullName = "Anjali K", Role = Role.Student });
        }

        public User Register(string username, string password, string fullName, Role role)
        {
            var user = new User
            {
                Id = _nextId++,
                Username = username,
                Password = password,
                FullName = fullName,
                Role = role
            };
            _users.Add(user);
            return user;
        }

        public User Login(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public List<User> GetAll() => _users;
    }
}
