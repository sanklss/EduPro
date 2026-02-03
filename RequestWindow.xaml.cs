using EduPro.Data;
using EduPro.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace EduPro
{
    public partial class RequestWindow : Window
    {
        private Request _request;
        private bool _isEditMode = false;

        public RequestWindow(Request request)
        {
            try
            {
                InitializeComponent();
                _request = request;
                _isEditMode = request.Id > 0;

                LoadData();
                LoadRequest();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации окна: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateFreeSeatsInfo();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new EduProContext())
                {
                    var users = context.Users
                        .Where(u => u.RoleId == 3) 
                        .OrderBy(u => u.FullName)
                        .ToList();
                    UserComboBox.ItemsSource = users;

                    var courses = context.Courses
                        .Include(c => c.CourseDirection)
                        .OrderBy(c => c.Name)
                        .ToList();
                    CourseComboBox.ItemsSource = courses;

                    var statuses = context.RequestStatuses
                        .OrderBy(s => s.Id)
                        .ToList();
                    StatusComboBox.ItemsSource = statuses;

                    if (_isEditMode)
                    {
                        if (_request.UserId > 0)
                        {
                            UserComboBox.SelectedItem = users.FirstOrDefault(u => u.Id == _request.UserId);
                        }
                        if (_request.CourseId > 0)
                        {
                            CourseComboBox.SelectedItem = courses.FirstOrDefault(c => c.Id == _request.CourseId);
                        }
                        if (_request.RequestStatusId > 0)
                        {
                            StatusComboBox.SelectedItem = statuses.FirstOrDefault(s => s.Id == _request.RequestStatusId);
                        }
                    }
                    else
                    {
                        StatusComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRequest()
        {
            try
            {
                if (_isEditMode)
                {
                    TitleText.Text = $"Заявка №{_request.Id}";
                    DeleteButton.Visibility = Visibility.Visible;
                    DatePicker.SelectedDate = new DateTime(_request.Date.Year, _request.Date.Month, _request.Date.Day);
                }
                else
                {
                    TitleText.Text = "Новая заявка";
                    DatePicker.SelectedDate = DateTime.Today;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных заявки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFreeSeatsInfo()
        {
            try
            {
                if (CourseComboBox.SelectedItem is Course selectedCourse)
                {
                    FreeSeatsText.Text = $"Свободных мест на курсе: {selectedCourse.FreeSeat}";

                    if (selectedCourse.FreeSeat < 10)
                    {
                        FreeSeatsText.Foreground = System.Windows.Media.Brushes.Red;
                        FreeSeatsText.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        FreeSeatsText.Foreground = System.Windows.Media.Brushes.Green;
                        FreeSeatsText.FontWeight = FontWeights.Normal;
                    }
                }
                else
                {
                    FreeSeatsText.Text = "Выберите курс для отображения информации";
                    FreeSeatsText.Foreground = System.Windows.Media.Brushes.Gray;
                }
            }
            catch (Exception ex)
            {
                FreeSeatsText.Text = "Ошибка получения информации о курсе";
                FreeSeatsText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void CourseComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateFreeSeatsInfo();
        }

        private bool ValidateForm()
        {
            if (UserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (CourseComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите курс!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату подачи заявки!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус заявки!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                var selectedUser = UserComboBox.SelectedItem as User;
                var selectedCourse = CourseComboBox.SelectedItem as Course;
                var selectedStatus = StatusComboBox.SelectedItem as RequestStatus;

                if (selectedStatus.Name == "Подтверждена" && selectedCourse.FreeSeat <= 0)
                {
                    MessageBox.Show("Нельзя подтвердить заявку! Нет свободных мест на курсе.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var context = new EduProContext())
                {
                    Request requestToSave;

                    if (_isEditMode)
                    {
                        requestToSave = context.Requests.Find(_request.Id);
                        if (requestToSave == null)
                        {
                            MessageBox.Show("Заявка не найдена в базе данных!",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (requestToSave.CourseId != selectedCourse.Id)
                        {
                            var oldCourse = context.Courses.Find(requestToSave.CourseId);
                            if (oldCourse != null)
                            {
                                oldCourse.FreeSeat++;
                            }
                        }
                    }
                    else
                    {
                        requestToSave = new Request();
                        context.Requests.Add(requestToSave);
                    }

                    requestToSave.UserId = selectedUser.Id;
                    requestToSave.CourseId = selectedCourse.Id;
                    requestToSave.RequestStatusId = selectedStatus.Id;
                    DateTime selectedDateTime = DatePicker.SelectedDate.Value;
                    requestToSave.Date = new DateOnly(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day);

                    if (selectedStatus.Name == "Подтверждена")
                    {
                        selectedCourse.FreeSeat--;
                        context.Entry(selectedCourse).State = EntityState.Modified;
                    }
                    else if (selectedStatus.Name == "Отменена" && _isEditMode)
                    {
                        var oldStatus = context.RequestStatuses.Find(_request.RequestStatusId);
                        if (oldStatus?.Name == "Подтверждена")
                        {
                            selectedCourse.FreeSeat++;
                            context.Entry(selectedCourse).State = EntityState.Modified;
                        }
                    }

                    context.SaveChanges();

                    MessageBox.Show("Заявка успешно сохранена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка сохранения в базу данных: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту заявку?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var context = new EduProContext())
                    {
                        var requestToDelete = context.Requests.Find(_request.Id);
                        if (requestToDelete != null)
                        {
                            if (requestToDelete.RequestStatus?.Name == "Подтверждена")
                            {
                                var course = context.Courses.Find(requestToDelete.CourseId);
                                if (course != null)
                                {
                                    course.FreeSeat++;
                                }
                            }

                            context.Requests.Remove(requestToDelete);
                            context.SaveChanges();
                        }
                    }

                    MessageBox.Show("Заявка успешно удалена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка закрытия окна: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}