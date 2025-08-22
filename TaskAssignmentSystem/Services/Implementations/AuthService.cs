using TaskAssignmentSystem.Models.Users;
using TaskAssignmentSystem.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly List<User> _users = new();
        private int _nextId = 1;

        public AuthService()
        {
            // Seed some demo users so you can test immediately
            _users.Add(new User { Id = _nextId++, Username = "admin", PasswordHash = HashPassword("admin123"), FullName = "System Admin", Role = Role.Admin });
            _users.Add(new User { Id = _nextId++, Username = "teacher", PasswordHash = HashPassword("teach123"), FullName = "Ms. Sharma", Role = Role.Teacher });
            _users.Add(new User { Id = _nextId++, Username = "student", PasswordHash = HashPassword("stud123"), FullName = "Anjali K", Role = Role.Student });
        }

        public User Register(string username, string password, string fullName, Role role)
        {
            var user = new User
            {
                Id = _nextId++,
                Username = username,
                PasswordHash = HashPassword(password),
                FullName = fullName,
                Role = role
            };
            _users.Add(user);
            return user;
        }

        public User Login(string username, string password)
        {
            var passwordHash = HashPassword(password);
            return _users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
        }

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public List<User> GetAll() => _users;

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
