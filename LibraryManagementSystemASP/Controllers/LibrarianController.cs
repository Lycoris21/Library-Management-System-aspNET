using System.Linq;
using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemASP.Services;

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
        public IActionResult GetReservationDetails(int reservationId)
        {
            var reservation = _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            return PartialView("_ReservationDetailss", reservation);
        }

        [HttpGet]
        public IActionResult GetBorrowingDetails(int borrowingId)
        {
            var borrowing = _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefault(b => b.BorrowId == borrowingId);

            if (borrowing == null)
            {
                return NotFound();
            }

            return PartialView("_BorrowingDetailss", borrowing);
        }

        [HttpPost]
        public IActionResult AddBorrowing([FromBody] BorrowingRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || request.BookId <= 0)
            {
                return BadRequest(new { message = "Invalid input." });
            }

            // Check if the user exists
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null)
            {
                return BadRequest(new { message = "User  not found." });
            }

            // Check if the book is available
            var book = _context.Books.FirstOrDefault(b => b.BookId == request.BookId && b.Status == "Available" && b.Quantity > 0);
            if (book == null)
            {
                return BadRequest(new { message = "Selected book is not available." });
            }

            // Create a new reservation
            var reservation = new Reservation
            {
                UserId = user.UserId, // Now we can safely access UserId
                BookId = request.BookId,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // Update the book status if necessary
            book.Status = "Reserved"; // or any other logic you want to apply
            _context.SaveChanges();

            return Ok(new { message = "Borrowing has been added successfully." });
        }

        [HttpGet]
        public IActionResult GetAvailableBooks()
        {
            var availableBooks = _context.Books
                .Where(b => b.Status == "Available" && b.Quantity > 0)
                .Select(b => new
                {
                    BookId = b.BookId,
                    Title = b.Title
                })
                .ToList();

            return Json(availableBooks);
        }
    }
}
