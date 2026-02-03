using System;
using System.Collections.Generic;

namespace EduPro.Models;

public partial class PrepodType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Capacity { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
