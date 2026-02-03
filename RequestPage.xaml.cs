using EduPro.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System.Windows.Data;

namespace EduPro
{
    /// <summary>
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        public RequestPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new EduProContext())
            {
                RequestitemControl.ItemsSource = context.Requests
                    .Include(r => r.User)
                    .Include(r => r.RequestStatus)
                    .Include(r => r.Course)
                    .ToList();

            }
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
