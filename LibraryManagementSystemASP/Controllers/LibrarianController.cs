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

        [HttpGet]
        public JsonResult GetReservationDetails(int reservationId)
        {
            var reservation = _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                return Json(new { success = false, message = "Reservation not found." });
            }

            return Json(new
            {
                reservationId = reservation.ReservationId,
                userId = reservation.UserId,
                username = reservation.User.Username,
                bookId = reservation.BookId,
                bookTitle = reservation.Book.Title,
                status = reservation.Status,
                reservedOn = reservation.CreatedAt,
                collectionDeadline = reservation.CollectionDeadline,
                lastUpdated = reservation.UpdatedAt
            });
        }

        [HttpGet]
        public JsonResult GetBorrowingDetails(int borrowingId)
        {
            var borrowing = _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefault(b => b.BorrowId == borrowingId);

            if (borrowing == null)
            {
                return Json(new { success = false, message = "Borrowing not found." });
            }

            return Json(new
            {
                borrowId = borrowing.BorrowId,
                userId = borrowing.UserId,
                username = borrowing.User.Username,
                bookId = borrowing.BookId,
                bookTitle = borrowing.Book.Title,
                status = borrowing.Status,
                borrowedOn = borrowing.BorrowDate,
                supposedReturnDate = borrowing.SupposedReturnDate,
                actualReturnDate = borrowing.ActualReturnDate,
                lastUpdated = borrowing.UpdatedAt
            });
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

        [HttpGet]
        public JsonResult GetAvailableBooks()
        {
            var availableBooks = _context.Books
                .Where(b => b.Status == "available" && b.Quantity > 0)
                .Select(b => new { b.BookId, b.Title, b.Quantity })
                .ToList();
            return Json(availableBooks);
        }

        [HttpPost]
        public JsonResult AddBorrowing([FromBody] Reservation request)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            var book = _context.Books.FirstOrDefault(b => b.BookId == request.BookId);

            if (user == null || book == null || book.Quantity <= 0)
            {
                return Json(new { success = false, message = "Invalid user or book selection." });
            }

            // Create a pending reservation
            var reservation = new Reservation
            {
                UserId = user.UserId,
                BookId = book.BookId,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                CollectionDeadline = DateTime.Now.AddDays(14),
                UpdatedAt = DateTime.Now
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // Update the reservation status to collected
            reservation.Status = "Collected";
            reservation.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, message = "Borrowing added successfully." });
        }
    }
}
