using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Models;
using LibraryManagementSystemASP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemASP.Controllers
{
    public class SharedController : Controller
    {
        private readonly LmsDbContext _context;
        private readonly UserService _userService;

        public SharedController(LmsDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }
        public IActionResult Records()
        {
            var reservationRecords = from user in _context.Users
                                     join reservation in _context.Reservations on user.UserId equals reservation.UserId
                                     join book in _context.Books on reservation.BookId equals book.BookId
                                     where reservation.Status != "Collected"
                                     select new RecordsViewModel
                                     {
                                         Username = user.Username,
                                         Title = book.Title,
                                         Status = reservation.Status,
                                         LastUpdated = (DateTime)reservation.UpdatedAt,
                                         RecordType = "Reservation",
                                         RecordId = reservation.ReservationId
                                     };

            var borrowingRecords = from user in _context.Users
                                   join borrowing in _context.Borrowings on user.UserId equals borrowing.UserId
                                   join book in _context.Books on borrowing.BookId equals book.BookId
                                   select new RecordsViewModel
                                   {
                                       Username = user.Username,
                                       Title = book.Title,
                                       Status = borrowing.Status,
                                       LastUpdated = (DateTime)borrowing.UpdatedAt,
                                       RecordType = "Borrowing",
                                       RecordId = borrowing.BorrowId
                                   };

            var records = reservationRecords.Union(borrowingRecords)
                                            .OrderByDescending(r => r.LastUpdated)
                                            .ToList();

            return View(records);
        }

        public IActionResult GetRecordDetails(string recordType, string username, string title)
        {
            if (recordType == "Reservation")
            {
                var reservation = _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Book)
                    .FirstOrDefault(r => r.User.Username == username && r.Book.Title == title);

                if (reservation != null)
                {
                    return PartialView("_Details", reservation);
                }
            }
            else if (recordType == "Borrowing")
            {
                var borrowing = _context.Borrowings
                    .Include(b => b.User)
                    .Include(b => b.Book)
                    .FirstOrDefault(b => b.User.Username == username && b.Book.Title == title);

                if (borrowing != null)
                {
                    return PartialView("_Details", borrowing);
                }
            }

            return NotFound();
        }

        public IActionResult BookListings()
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
    }
}
