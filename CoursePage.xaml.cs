using EduPro.Data;
using EduPro.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EduPro
{
    /// <summary>
    /// Логика взаимодействия для CoursePage.xaml
    /// </summary>
    public partial class CoursePage : Page
    {
        private Role _role;
        public CoursePage(Role role)
        {
            InitializeComponent();
            _role = role;
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new EduProContext())
            {
                CourseItemControl.ItemsSource = context.Courses
                    .Include(r => r.CourseDirection)
                    .Include(r => r.PrepodType)
                    .ToList();
            }
        }
    }
}
