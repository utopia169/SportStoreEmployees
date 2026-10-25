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

namespace SportStoreEmployees
{
    public partial class MainWindow : Window
    {
        private SportStoreEmployees1Entities db = new SportStoreEmployees1Entities();
        private Users currentUser;
        private bool isLoaded;
        public MainWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = db.Employees
                    .Include("Roles")
                    .Include("Shifts")
                    .Select(e => new
                    {
                        Id = e.Id,
                        LastName = e.LastName,
                        FirstName = e.FirstName,
                        RoleName = e.Roles.Name,
                        Phone = e.Phone,
                        IsHere = e.Shifts.Any(s => s.IsHere == true && s.CheckOut == null)
                    })
                    .ToList();
                dgEmployees.ItemsSource = employees;
                txtTotalEmployees.Text = employees.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void ButnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }


        private void ButnDltEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedEmployee = dgEmployees.SelectedItem;
                if (selectedEmployee == null)
                {
                    MessageBox.Show($"Вы не выбрали пользователя которого хотите удалить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (selectedEmployee != null)
                {
                    MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить работника?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        int idEmployee = selectedEmployee.Id;
                        var employee = db.Employees.Find(idEmployee);

                        if (employee != null)
                        {
                            var employeeShifts = db.Shifts.Where(s => s.EmployeeId == idEmployee);
                            db.Shifts.RemoveRange(employeeShifts);

                            var employeeNotifications = db.Notifications.Where(n => n.EmployeeId == idEmployee);
                            db.Notifications.RemoveRange(employeeNotifications);

                            db.Employees.Remove(employee);
                            db.SaveChanges();
                            LoadEmployees();

                            MessageBox.Show("Пользователь был удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        if (employee == null)
                        {
                            MessageBox.Show($"Пользователь не найден", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Ошибка при удалении {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButnEditEmployee_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите выйти?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                currentUser = null;
                new Login().Show();
                this.Close();
            }
            else
            {
                return;
            }

        }
    }

}
