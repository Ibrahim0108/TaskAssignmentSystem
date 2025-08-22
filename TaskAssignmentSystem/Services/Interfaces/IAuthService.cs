using TaskAssignmentSystem.Models.Users;
using System.Collections.Generic;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface IAuthService
    {
        List<User> GetAll();
        List<User> GetPendingUsers();
        bool ApproveUser(int id);
        bool RejectUser(int id);

        // Register/Login methods
        User Register(string username, string password, string fullName, Role role);
        User? Login(string username, string password);

        // ✅ Add this method
        User? GetById(int id);
    }
}
