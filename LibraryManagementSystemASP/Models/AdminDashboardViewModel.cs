namespace LibraryManagementSystemASP.Models
{
    public class AdminDashboardViewModel
    {
        public int BooksCount { get; set; }
        public int UsersCount { get; set; }
        public int ReservationsCount { get; set; }
        public int BorrowingCount { get; set; }
        public int OverdueBooksCount { get; set; }
        public int TotalBorrowedCount { get; set; }
    }
}
