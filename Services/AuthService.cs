using EduPro.Models;
using EduPro.Data;

namespace EduPro.Services
{
    public class AuthService
    {
        public Role TryAuth(string login, string password)
        {
            using (var context = new EduProContext())
            {
                User user = context.Users.FirstOrDefault(u => u.Login == login & u.Password == password);

                if (user == null)
                {
                    return null;
                }

                return context.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            }
        }
    }
}
