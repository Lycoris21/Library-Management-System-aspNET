namespace LibraryManagementSystemASP.Models
{
    public class AdminUserManagementViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int CurrentlyReserved { get; set; }
        public int CurrentlyBorrowed { get; set; }
        public int Overdues { get; set; }
        public int TotalBorrowed { get; set; }
    }
}
