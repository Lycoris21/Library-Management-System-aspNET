using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Models;
using LibraryManagementSystemASP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var user = UserSession.GetInstance().CurrentUser;
            if (user == null) return RedirectToAction("Login", "Account");

            var borrowings = _context.Borrowings.Include(b => b.Book)
                                        .Where(b => b.UserId == user.UserId)
                                        .OrderByDescending(b => b.UpdatedAt)
                                        .Take(10)
                                        .Select(b => new TransactionModel
                                        {
                                            Type = "Borrowing",
                                            UpdatedAt = b.UpdatedAt ?? DateTime.MinValue,
                                            Book = b.Book,
                                            Status = b.Status
                                        }).ToList();

            var reservations = _context.Reservations.Include(r => r.Book)
                                                     .Where(r => r.UserId == user.UserId && r.Status != "Collected")
                                                     .OrderByDescending(r => r.UpdatedAt)
                                                     .Take(10)
                                                     .Select(r => new TransactionModel
                                                     {
                                                         Type = "Reservation",
                                                         UpdatedAt = r.UpdatedAt ?? DateTime.MinValue,
                                                         Book = r.Book,
                                                         Status = r.Status
                                                     }).ToList();

            var latestTransactions = borrowings.Concat(reservations)
                                               .OrderByDescending(t => t.UpdatedAt)
                                               .Take(10)
                                               .ToList();

            var viewModel = new DashboardViewModel
            {
                ReservationCount = _context.Reservations.Count(r => r.UserId == user.UserId && r.Status == "Pending"),
                BorrowingCount = _context.Borrowings.Count(b => b.UserId == user.UserId && b.Status == "Borrowed"),
                OverdueCount = _context.Borrowings.Count(b => b.UserId == user.UserId && b.Status == "Overdue"),
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

    }
}
