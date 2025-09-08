using TaskAssignmentSystem.Models.Users;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface IAuthService
    {
        User Register(string username, string password, string fullName, Role role, string email, string department, int? year, string section);
        User Login(string username, string password);
        User? GetById(int id);
        List<User> GetAll();

        // New names used by AdminController and implemented in AuthService
        List<User> GetPending();
        bool Approve(int userId);
        bool SetRole(int userId, Role role);

        bool Update(User user);
        List<Notification> GetNotifications(int userId);
        Notification? GetNotificationById(int id);
        bool MarkNotificationAsRead(int id);

    }
}