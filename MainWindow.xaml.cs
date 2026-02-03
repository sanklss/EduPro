using EduPro.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EduPro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Role _role;
        public MainWindow(Role role)
        {
            InitializeComponent();
            _role = role;
            mainFrame.Navigated += mainFrame_Navigated;
            mainFrame.Navigate(new CoursePage(role));
        }

        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                if (_role == null)
                {
                    this.Title = $"EduPro Гость  {page.Title}";
                }
                else
                {
                    switch (_role.Id)
                    {
                        case 1:
                            this.Title = $"EduPro Администратор  {page.Title}";
                            break;
                        case 2:
                            this.Title = $"EduPro Менеджер  {page.Title}";
                            break;
                        case 3:
                            this.Title = $"EduPro Клиент  {page.Title}";
                            break;
                    }
                }
            }
            else
            {
                this.Title = "EduPro";
            }
        }
    }
}