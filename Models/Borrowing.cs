using System;
using System.Collections.Generic;

namespace LibraryManagementSystemASP.Models;

public partial class Borrowing
{
    public int BorrowId { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public DateTime? BorrowDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public DateTime? ActualReturnDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
