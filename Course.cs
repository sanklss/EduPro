using System;
using System.Collections.Generic;

namespace EduPro;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CourseDirectionId { get; set; }

    public int Duration { get; set; }

    public DateOnly Date { get; set; }

    public int Price { get; set; }

    public int PrepodTypeId { get; set; }

    public int Capacity { get; set; }

    public int FreeSeat { get; set; }

    public string Image { get; set; } = null!;

    public virtual CourseDirection CourseDirection { get; set; } = null!;

    public virtual PrepodType PrepodType { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
