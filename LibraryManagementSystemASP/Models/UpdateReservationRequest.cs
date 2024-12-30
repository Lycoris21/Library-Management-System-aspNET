namespace LibraryManagementSystemASP.Models
{
    public class UpdateReservationRequest
    {
        public int ReservationId { get; set; }
        public string NewStatus { get; set; }
    }
}
