using System.Collections.Generic;

namespace LibraryManagementSystemASP.Models
{
    public class DashboardViewModel
    {
        public int ReservationCount { get; set; }
        public int BorrowingCount { get; set; }
        public int OverdueCount { get; set; }
        public List<Book> MostBorrowedBooks { get; set; }
        public List<Book> NewestBooks { get; set; }
        public List<TransactionModel> LatestTransactions { get; set; }

    }
}
