using EduPro.Data;
using EduPro.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EduPro
{
    public partial class RequestCard : UserControl
    {
        private Role _currentRole;

        public RequestCard()
        {
            InitializeComponent();
            this.Loaded += RequestCard_Loaded;
        }

        private void RequestCard_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var itemsControl = FindParentItemsControl(this);
                _currentRole = itemsControl?.Tag as Role;

                if (_currentRole == null || _currentRole.Id != 2) 
                {
                    RedactButton.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки карточки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ItemsControl FindParentItemsControl(DependencyObject child)
        {
            try
            {
                DependencyObject parent = VisualTreeHelper.GetParent(child);
                while (parent != null && !(parent is ItemsControl))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                return parent as ItemsControl;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Page FindParentPage(DependencyObject child)
        {
            try
            {
                DependencyObject parent = VisualTreeHelper.GetParent(child);
                while (parent != null && !(parent is Page))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                return parent as Page;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void RedactButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentRole?.Id != 2)
                {
                    MessageBox.Show("Только менеджеры могут редактировать заявки!",
                        "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DataContext is Request request)
                {
                    var requestWindow = new RequestWindow(request);
                    bool? result = requestWindow.ShowDialog();

                    if (result == true)
                    {
                        var page = FindParentPage(this);
                        if (page is RequestPage requestPage)
                        {
                            requestPage.ApplyFilters();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия заявки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}