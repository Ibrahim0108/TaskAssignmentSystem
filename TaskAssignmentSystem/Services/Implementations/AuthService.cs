using TaskAssignmentSystem.Models.Users;
using TaskAssignmentSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly List<User> _users = new();

        public List<User> GetAll() => _users;

        public List<User> GetPendingUsers()
        {
            return _users.Where(u => !u.IsApproved).ToList();
        }

        public bool ApproveUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            user.IsApproved = true;
            return true;
        }

        public bool RejectUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            _users.Remove(user);
            return true;
        }

        public User Register(string username, string password, string fullName, Role role)
        {
            var user = new User
            {
                Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1,
                Username = username,
                PasswordHash = password,
                FullName = fullName,
                Role = role,
                IsApproved = role != Role.Student // Students need admin approval
            };
            _users.Add(user);
            return user;
        }

        public User? Login(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password && u.IsApproved);
        }

        // ✅ Implement GetById
        public User? GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
    }
}
