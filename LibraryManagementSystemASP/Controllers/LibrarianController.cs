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
        
        public IActionResult LibrarianOperationsManagement()
        {
            var reservations = _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .Where(r => r.Status == "Pending")
            .OrderByDescending(r => r.UpdatedAt)
            .ToList();

            var borrowings = _context.Borrowings
                .Include(b => b.Book)
                .Include(r => r.User)
                .Where(b => b.Status == "Borrowed")
                .OrderByDescending(b => b.UpdatedAt)
                .ToList();

            var viewModel = new LibrarianOperationsManagementViewModel
            {
                Reservations = reservations,
                Borrowings = borrowings
            };

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult UpdateReservationStatus(int reservationId, string newStatus)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation != null)
            {
                reservation.Status = newStatus;
                _context.SaveChanges();
                return Json(new { success = true, message = $"Reservation status updated to '{newStatus}'." });
            }
            return Json(new { success = false, message = "Reservation not found." });
        }

        [HttpPost]
        public JsonResult UpdateBorrowingStatus(int borrowingId)
        {
            var borrowing = _context.Borrowings.FirstOrDefault(b => b.BorrowId == borrowingId);
            if (borrowing != null)
            {
                borrowing.Status = "Returned";
                _context.SaveChanges();
                return Json(new { success = true, message = "Borrowing status updated to 'Returned'." });
            }
            return Json(new { success = false, message = "Borrowing not found." });
        }
    }
}
