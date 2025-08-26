using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TaskAssignmentSystem.Data;
using TaskAssignmentSystem.Models.Users;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;

        public AuthService(ApplicationDbContext db)
        {
            _db = db;
            EnsureSeeded();
        }

        private void EnsureSeeded()
        {
            if (!_db.Users.Any(u => u.Role == Role.Admin))
            {
                _db.Users.Add(new User
                { 
                    Username = "superadmin",
                    PasswordHash = HashPassword("superadmin123"),
                    FullName = "Super Admin",
                    Email = "Admin@gmail.com",
                    Role = Role.Admin,
                    IsApproved = true,
                    Department = "Administration",
                    Year = null,
                    Section = null

                });
                _db.SaveChanges();
            }
        }

        public User Register(string username, string password, string fullName, Role role, string email, string department, int? year, string section)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                FullName = fullName,
                Role = role,
                Email = email,
                Department = department,
                Year = (role == Role.Student) ? year : null,
                Section = (role == Role.Student) ? section : null,
                IsApproved = role == Role.Student
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }


        public User Login(string username, string password)
        {
            var passwordHash = HashPassword(password);
            return _db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash)!;
        }

        public User? GetById(int id) => _db.Users.FirstOrDefault(u => u.Id == id);

        public List<User> GetAll() => _db.Users.OrderBy(u => u.Id).ToList();

        public List<User> GetPending() =>
            _db.Users.Where(u => !u.IsApproved && (u.Role == Role.Admin || u.Role == Role.Teacher)).ToList();

        public bool Approve(int userId)
        {
            var u = _db.Users.FirstOrDefault(x => x.Id == userId);
            if (u == null) return false;
            u.IsApproved = true;
            _db.SaveChanges();
            return true;
        }

        public bool SetRole(int userId, Role role)
        {
            var u = _db.Users.FirstOrDefault(x => x.Id == userId);
            if (u == null) return false;
            u.Role = role;
            _db.SaveChanges();
            return true;
        }

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