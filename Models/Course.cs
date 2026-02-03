using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace EduPro.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CourseDirectionId { get; set; }

    public int Duration { get; set; }

    public DateOnly Date { get; set; }
    public FontWeight TitleFontWeight
    {
        get
        {
            var today = new DateOnly(2026, 6, 10);
            var daysLeft = today.DayNumber - Date.DayNumber;
            return daysLeft is > 0 and < 7 ? FontWeights.Bold : FontWeights.Normal;
        }
    }

    public int Price { get; set; }

    public int PrepodTypeId { get; set; }

    public int Capacity { get; set; }

    public int FreeSeat { get; set; }
    public Brush BackGroundColor
    {
        get
        {
            if (FreeSeat < (PrepodType.Capacity * 0.1)) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB6C1"));
            return new SolidColorBrush(Colors.White);
        }
    }

    public string Image { get; set; } = null!;
    public string ImageFullPath => !string.IsNullOrEmpty(Image) ? $"Images/{Image}" : "Images/icon.png";

    public virtual CourseDirection CourseDirection { get; set; } = null!;

    public virtual PrepodType PrepodType { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
