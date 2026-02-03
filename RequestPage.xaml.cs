using EduPro.Data;
using EduPro.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EduPro
{
    public partial class RequestPage : Page
    {
        private bool _isAscendingSort = false;
        private Role _currentRole;

        public RequestPage(Role role)
        {
            try
            {
                InitializeComponent();
                _currentRole = role;

                RequestitemControl.Tag = _currentRole;

                AdjustInterface();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации страницы: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadRequests();
                LoadStatuses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdjustInterface()
        {
            try
            {
                if (_currentRole == null || _currentRole.Id != 2)
                {
                    CreateButton.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка настройки интерфейса: {ex.Message}",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadRequests()
        {
            try
            {
                using (var context = new EduProContext())
                {
                    var requests = context.Requests
                        .Include(r => r.User)
                        .Include(r => r.RequestStatus)
                        .Include(r => r.Course)
                        .ToList();

                    RequestitemControl.ItemsSource = requests;
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка базы данных при загрузке заявок: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatuses()
        {
            try
            {
                using (var context = new EduProContext())
                {
                    var statuses = context.RequestStatuses.ToList();

                    statuses.Insert(0, new RequestStatus
                    {
                        Id = 0,
                        Name = "Все статусы"
                    });

                    StatusFilterComboBox.ItemsSource = statuses;
                    StatusFilterComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статусов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isAscendingSort = !_isAscendingSort;
                SortButton.Content = _isAscendingSort ? "По дате " : "По дате ";
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сортировки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ApplyFilters()
        {
            try
            {
                using (var context = new EduProContext())
                {
                    var query = context.Requests
                        .Include(r => r.User)
                        .Include(r => r.RequestStatus)
                        .Include(r => r.Course)
                        .AsQueryable();

                    string searchText = SearchTextBox.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        query = query.Where(r =>
                            (r.User != null && r.User.FullName.Contains(searchText)) ||
                            r.Id.ToString().Contains(searchText));
                    }

                    var selectedStatus = StatusFilterComboBox.SelectedItem as RequestStatus;
                    if (selectedStatus != null && selectedStatus.Id != 0)
                    {
                        query = query.Where(r => r.RequestStatusId == selectedStatus.Id);
                    }

                    query = _isAscendingSort
                        ? query.OrderBy(r => r.Date)
                        : query.OrderByDescending(r => r.Date);

                    RequestitemControl.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}\n\nДетали: {ex.InnerException?.Message}",
                    "Ошибка фильтрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentRole?.Id != 2)
                {
                    MessageBox.Show("Только менеджеры могут создавать заявки!",
                        "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newRequest = new Request();
                var requestWindow = new RequestWindow(newRequest);

                if (requestWindow.ShowDialog() == true)
                {
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания заявки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}