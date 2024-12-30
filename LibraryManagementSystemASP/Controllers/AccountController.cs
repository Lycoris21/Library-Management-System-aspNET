using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystemASP.Models;
using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Utilities;
using System.Linq;

namespace LibraryManagementSystemASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly LmsDbContext _context;

        public AccountController(LmsDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                // Find the user by username
                var existingUser = _context.Users.SingleOrDefault(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    // Verify password
                    if (PasswordHasher.VerifyPassword(model.Password, existingUser.Password))
                    {
                        // Set the current user in the session
                        UserSession.GetInstance().SetCurrentUser(existingUser);

                        // Redirect based on role
                        if (existingUser.Role == "Admin")
                        {
                            return RedirectToAction("AdminDashboard", "Admin");
                        }
                        else if (existingUser.Role == "Librarian")
                        {
                            return RedirectToAction("LibrarianDashboard", "Librarian");
                        }
                        else if (existingUser.Role == "Reader")
                        {
                            return RedirectToAction("ReaderDashboard", "Reader");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Register
        public IActionResult Register()
        {
            ViewBag.RegistrationSuccess = false;
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                var existingUser = _context.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username already exists.");
                    return View(model);
                }

                // Check if passwords match
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                // Hash the password (implement your hashing logic)
                var hashedPassword = PasswordHasher.HashPassword(model.Password);

                // Create a new user
                var newUser = new User
                {
                    Username = model.Username,
                    Password = hashedPassword,
                    Role = "Reader" // Set default role or modify as needed
                };

                // Add the new user to the database
                _context.Users.Add(newUser);
                _context.SaveChanges();

                // Set the success flag
                ViewBag.RegistrationSuccess = true;

                // Optionally, redirect to a login page or dashboard
                return View("Register", model);
            }

            // If we got this far, something failed; redisplay the form
            return View(model);
        }

        [HttpGet]
        public IActionResult Profile()
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

            if (currentUser != null && currentPassword == null)
            {
                ViewBag.ErrorMessage = "Please enter your password to confirm changes";
                return View("Profile");
            }
            else if (currentUser != null && currentPassword != null && !PasswordHasher.VerifyPassword(currentPassword, currentUser.Password))
            {
                ViewBag.ErrorMessage = "The current password is incorrect.";
                return View("Profile");
            }

            if (!string.IsNullOrEmpty(newPassword) && newPassword != confirmNewPassword)
            {
                ViewBag.ErrorMessage = "The new password and confirm password do not match.";
                return View("Profile");
            }

            if (_context.Users.Any(u => u.Username == username && u.UserId != currentUser.UserId))
            {
                ViewBag.ErrorMessage = "The username is already taken.";
                return View("Profile");
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                currentUser.Password = PasswordHasher.HashPassword(newPassword);
            }

            currentUser.Username = username;
            _context.Users.Update(currentUser);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Changes saved.";

            return View("Profile");
        }
    }
}