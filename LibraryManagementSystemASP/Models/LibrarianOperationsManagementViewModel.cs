using System.Collections.Generic;
using LibraryManagementSystemASP.Models;

namespace LibraryManagementSystemASP.Models
{
    public class LibrarianOperationsManagementViewModel
    {
        public List<Reservation> Reservations { get; set; }
        public List<Borrowing> Borrowings { get; set; }

        public LibrarianOperationsManagementViewModel()
        {
            Reservations = new List<Reservation>();
            Borrowings = new List<Borrowing>();
        }

    }
}
