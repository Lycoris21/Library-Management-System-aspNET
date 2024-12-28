using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Models;
using LibraryManagementSystemASP.Services;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystemASP.Utilities;

namespace LibraryManagementSystemASP.Controllers
{
    public class AdminController : Controller
    {
        private readonly LmsDbContext _context;
        private readonly UserService _userService;

        public AdminController(LmsDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public IActionResult AdminDashboard()
        {
            // Get the count of each entity
            var booksCount = _context.Books.Count(b => b.Status == "Available");
            var usersCount = _context.Users.Count();
            var reservationsCount = _context.Reservations.Count(r => r.Status == "Pending");
            var borrowingCount = _context.Borrowings.Count(b => b.Status == "Borrowed");
            var overdueBooksCount = _context.Borrowings.Count(b => b.Status == "Overdue");
            var totalBorrowedCount = _context.Borrowings.Count();

            // Create a view model to pass to the view
            var viewModel = new AdminDashboardViewModel
            {
                BooksCount = booksCount,
                UsersCount = usersCount,
                ReservationsCount = reservationsCount,
                BorrowingCount = borrowingCount,
                OverdueBooksCount = overdueBooksCount,
                TotalBorrowedCount = totalBorrowedCount
            };
            return View(viewModel);
        }

        public IActionResult AdminBookManagement()
        {
            var books = _context.Books.ToList();
            return View(books);
        }
        [HttpPost]
        public IActionResult AddBook(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("AdminBookManagement");
        }

        [HttpPost]
        public IActionResult EditBook(Book book)
        {
            var existingBook = _context.Books.FirstOrDefault(b => b.BookId == book.BookId);

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Category = book.Category;
            existingBook.Isbn = book.Isbn;
            existingBook.Publisher = book.Publisher;
            existingBook.PublishedYear = book.PublishedYear;
            existingBook.Quantity = book.Quantity;
            existingBook.Status = book.Status;
            existingBook.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return RedirectToAction("AdminBookManagement");
        }

        [HttpPost]
        public IActionResult DeleteBook(int bookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction("AdminBookManagement");
        }

        public IActionResult AdminUserManagement()
        {
            var users = _context.Users.Select(user => new AdminUserManagementViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                CurrentlyReserved = user.Reservations.Count(r => r.Status == "Pending"),
                CurrentlyBorrowed = user.Borrowings.Count(b => b.Status == "Borrowed"),
                Overdues = user.Borrowings.Count(b => b.Status == "Overdue"),
                TotalBorrowed = user.Borrowings.Count()
            }).ToList();

            return View(users);
        }

        // Add User
        [HttpPost]
        public IActionResult AddUser(AdminUserManagementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    Role = model.Role,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("AdminUserManagement");
            }

            return View("AdminUserManagement", _context.Users.ToList());
        }

        // Edit User
        [HttpPost]
        public IActionResult EditUser(AdminUserManagementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
                if (user != null)
                {
                    user.Username = model.Username;
                    user.Role = model.Role;
                    user.UpdatedAt = DateTime.Now;

                    // Check if the password has changed
                    if (!PasswordHasher.VerifyPassword(model.Password, user.Password))
                    {
                        user.Password = PasswordHasher.HashPassword(model.Password);
                    }

                    _context.Users.Update(user);
                    _context.SaveChanges();
                }

                return RedirectToAction("AdminUserManagement");
            }

            return View("AdminUserManagement", _context.Users.ToList());
        }

        // Delete User
        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("AdminUserManagement");
        }

        public IActionResult AdminRecords()
        {
            return View();
        }
    }
}
