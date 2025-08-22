using TaskAssignmentSystem.Models.Users;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface IAuthService
    {
        User Register(string username, string password, string fullName, Role role);
        User Login(string username, string password);
        User? GetById(int id);
        List<User> GetAll();
        List<User> GetPending();
        bool Approve(int userId);
        bool SetRole(int userId, Role role);
    }
}
