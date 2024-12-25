using System;
using System.Collections.Generic;

namespace LibraryManagementSystemASP.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime ReservedUntil { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
