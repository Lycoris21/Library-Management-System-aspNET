namespace LibraryManagementSystemASP.Models
{
    public class UpdateBorrowingRequest
    {
        public int BorrowingId { get; set; }
        public string NewStatus { get; set; }
    }
}
