// Models/UserSession.cs
using LibraryManagementSystemASP.Models;

namespace LibraryManagementSystemASP.Models
{
    public class UserSession
    {
        private static UserSession _instance;
        public User CurrentUser { get; private set; }

        private UserSession() { }

        public static UserSession GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UserSession();
            }
            return _instance;
        }

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }
    }
}