using System;
using System.Collections.Generic;

namespace LibraryManagementSystemASP.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Category { get; set; }

    public string? Isbn { get; set; }

    public string? Publisher { get; set; }

    public int PublishedYear { get; set; }

    public int Quantity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
