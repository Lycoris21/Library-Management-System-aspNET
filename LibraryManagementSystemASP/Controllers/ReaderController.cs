using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Models;
using LibraryManagementSystemASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LibraryManagementSystemASP.Controllers
{
    public class ReaderController : Controller
    {

        private readonly LmsDbContext _context;

        public ReaderController(LmsDbContext context)
        {
            _context = context;
        }
        public IActionResult ReaderDashboard()
        {
            return View();
        }

        public IActionResult ReaderBrowseBooks()
        {
            return View();
        }

        public IActionResult ReaderReservations()
        {
            return View();
        }

        public IActionResult ReaderBorrowing()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReaderProfile()
        {
            var user = UserSession.GetInstance().CurrentUser;
            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.Username = user.Username;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string username, string currentPassword, string newPassword, string confirmNewPassword)
        {
            var currentUser = UserSession.GetInstance().CurrentUser;

            if(currentUser != null && currentPassword == null)
            {
                ViewBag.ErrorMessage = "Please enter your password to confirm changes";
                return View("ReaderProfile");
            }else if(currentUser != null && currentPassword!= null && !PasswordHasher.VerifyPassword(currentPassword, currentUser.Password))
            {
                ViewBag.ErrorMessage = "The current password is incorrect.";
                return View("ReaderProfile");
            }

            if (!string.IsNullOrEmpty(newPassword) && newPassword != confirmNewPassword)
            {
                ViewBag.ErrorMessage = "The new password and confirm password do not match.";
                return View("ReaderProfile");
            }

            if (_context.Users.Any(u => u.Username == username && u.UserId != currentUser.UserId))
            {
                ViewBag.ErrorMessage = "The username is already taken.";
                return View("ReaderProfile");
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                currentUser.Password = PasswordHasher.HashPassword(newPassword);
            }

            currentUser.Username = username;
            _context.Users.Update(currentUser);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Changes saved.";

            return View("ReaderProfile");
        }

    }
}
