using EduPro.Models;
using EduPro.Services;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace EduPro
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AuthService _authService;
        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData() == false)
            {
                return;
            }

            Role role = _authService.TryAuth(LoginTextBox.Text, PasswordBox.Password);

            if (role != null)
            {
                MainWindow mainWindow = new MainWindow(role);
                mainWindow.Show();
                this.Close();
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(null);
            mainWindow.Show();
            this.Close();
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Поле 'Логин' или 'Пароль' не могут быть пустыми", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
