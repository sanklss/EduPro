using System;
using System.Collections.Generic;

namespace EduPro.Models;

public partial class Request
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int UserId { get; set; }

    public DateOnly Date { get; set; }

    public int RequestStatusId { get; set; }

    public int CountSeats { get; set; }

    public int FullPrice { get; set; }

    public string? Comment { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual RequestStatus RequestStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
