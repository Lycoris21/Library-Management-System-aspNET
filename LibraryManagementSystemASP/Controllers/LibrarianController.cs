using System.Linq;
using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemASP.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly LmsDbContext _context;

        public LibrarianController(LmsDbContext context)
        {
            _context = context;
        }
        public IActionResult LibrarianDashboard()
        {
            var borrowings = _context.Borrowings.Include(b => b.Book)
                                        .OrderByDescending(b => b.UpdatedAt)
                                        .Take(10)
                                        .Select(b => new TransactionModel
                                        {
                                            Type = "Borrowing",
                                            UpdatedAt = b.UpdatedAt ?? DateTime.MinValue, 
                                            Book = b.Book,
                                            Status = b.Status
                                        }).ToList();

            // Fetch reservations but exclude those with status "Collected"
            var reservations = _context.Reservations.Include(r => r.Book)
                                                     .Where(r => r.Status != "Collected")
                                                     .OrderByDescending(r => r.UpdatedAt) 
                                                     .Take(10)
                                                     .Select(r => new TransactionModel
                                                     {
                                                         Type = "Reservation",
                                                         UpdatedAt = r.UpdatedAt ?? DateTime.MinValue,
                                                         Book = r.Book,
                                                         Status = r.Status
                                                     }).ToList();

            // Combine both borrowings and reservations
            var latestTransactions = borrowings.Concat(reservations)
                                               .OrderByDescending(t => t.UpdatedAt)
                                               .Take(10)
                                               .ToList();

            var viewModel = new DashboardViewModel
            {
                ReservationCount = _context.Reservations.Count(r => r.Status == "Pending"),
                BorrowingCount = _context.Borrowings.Count(b => b.Status == "Borrowed"),
                OverdueCount = _context.Borrowings.Count(b => b.Status == "Overdue"),
                MostBorrowedBooks = _context.Books
                    .Select(b => new
                    {
                        Book = b,
                        BorrowingCount = b.Borrowings.Count
                    })
                    .OrderByDescending(b => b.BorrowingCount)
                    .Take(10)
                    .Select(b => b.Book)
                    .ToList(),

                NewestBooks = _context.Books
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(10)
                    .ToList(),

                LatestTransactions = latestTransactions
            };

            return View(viewModel);
        }




        public IActionResult LibrarianBookListings()
        {
            var availableBooks = _context.Books
                .Where(b => b.Status == "Available")
                .ToList();
            return View(availableBooks);
        }

        [HttpPost]
        public JsonResult ReserveBook(int id)
        {

            var user = UserSession.GetInstance().CurrentUser;

            var book = _context.Books.Include(b => b.Reservations).FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return Json(new { title = "Error", message = "Book not found." });
            }

            if (book.Quantity == 0)
            {
                return Json(new { title = "Out of Stock", message = "Sorry, this book is currently out of stock." });
            }

            if (book.Reservations.Any(r => r.Status == "Pending"))
            {
                return Json(new { title = "Already Reserved", message = "You already have a pending reservation for this book." });
            }

            // Add reservation
            _context.Reservations.Add(new Reservation
            {
                UserId = user.UserId,
                BookId = book.BookId,
                Status = "Pending",
                CollectionDeadline = DateTime.Now.AddDays(7),
                UpdatedAt = DateTime.Now
            });
            _context.SaveChanges();

            return Json(new { title = "Success", message = "Book reserved successfully! You have one week to collect it." });
        }


        public IActionResult LibrarianOperationsManagement()
        {
            return View();
        }

        public IActionResult LibrarianRecords()
        {
            return View();
        }
    }
}
