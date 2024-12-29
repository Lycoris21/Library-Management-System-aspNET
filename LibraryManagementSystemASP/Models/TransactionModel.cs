namespace LibraryManagementSystemASP.Models
{
    public class TransactionModel
    {
        public string Type { get; set; } // "Borrowing" or "Reservation"
        public DateTime UpdatedAt { get; set; }
        public Book Book { get; set; }
        public string Status { get; set; } // "Borrowed", "Pending", etc.
    }
}
