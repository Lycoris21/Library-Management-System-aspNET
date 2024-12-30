using System;
using System.Collections.Generic;

namespace LibraryManagementSystemASP.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CollectionDeadline { get; set; }

    public DateTime? CollectedOn { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
