using System.Linq;
using LibraryManagementSystemASP.Data;
using LibraryManagementSystemASP.Models;

namespace LibraryManagementSystemASP.Services
{
    public class UserService
    {

        private readonly LmsDbContext _context;

        public UserService(LmsDbContext context)
        {
            _context = context;
        }

        public List<AdminUserManagementViewModel> GetUsersWithBorrowingInfo()
        {
            var query = from user in _context.Users
                        join borrowing in _context.Borrowings on user.UserId equals borrowing.UserId into borrowed
                        from borrow in borrowed.DefaultIfEmpty()
                        join reservation in _context.Reservations on user.UserId equals reservation.UserId into reserved
                        from res in reserved.DefaultIfEmpty()
                        group new { Borrow = borrow, Reservation = res } by new { user.UserId, user.Username, user.Role } into g
                        select new AdminUserManagementViewModel
                        {
                            UserId = g.Key.UserId,
                            Username = g.Key.Username,
                            Role = g.Key.Role,
                            CurrentlyReserved = g.Count(x => x.Reservation != null && x.Reservation.Status == "Pending"),
                            CurrentlyBorrowed = g.Count(x => x.Borrow != null && x.Borrow.Status == "Borrowed"),
                            Overdues = g.Count(x => x.Borrow != null && x.Borrow.Status == "Overdue"),
                            TotalBorrowed = g.Count(x => x.Borrow != null) // This counts all borrowings regardless of status
                        };

            return query.ToList();
        }

    }
}
