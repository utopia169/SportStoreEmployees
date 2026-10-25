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
using System.Windows.Shapes;

namespace SportStoreEmployees
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private SportStoreEmployees1Entities db = new SportStoreEmployees1Entities();
        public AddWindow()
        {
            InitializeComponent();

            LoadRolesToComboBox();
        }
        private void LoadRolesToComboBox()
        {
            try
            {
                var rolesList = db.Roles.ToList();
                cmbRole.ItemsSource = rolesList;

                cmbRole.DisplayMemberPath = "Name";
                cmbRole.SelectedValuePath = "Id";
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка при загрузке должностей из БД: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButnAdd_Click(object sender, RoutedEventArgs e)
        {
            string lastName = txtLastName.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string phoneStr = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show($"Заполните Имя или Фамилию сотрудника", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbRole.SelectedValue == null) 
            {
                MessageBox.Show($"Выберите должность сотрудника", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(phoneStr) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show($"Введите номер телефона или эллектронную почту сотрудника", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string digitsOnlyPhone = phoneStr.Replace("+", "").Replace("-", "").Replace(" ", "");
            if (!int.TryParse(digitsOnlyPhone, out int phoneAsInt))
            {
                MessageBox.Show("Номер телефона должен состоять только из цифр!", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (db.Users.Any(u => u.Login == email))
            {
                MessageBox.Show($"Пользователь с такой эллектронной почтой уже зарегестрирован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int selectedRoleId = Convert.ToInt32(cmbRole.SelectedValue);

            using (var transaction = db.Database.BeginTransaction()) 
            {
                try
                {
                    string defaultPassword = "123";

                    var newUser = new Users
                    {
                        Login = email,
                        Password = defaultPassword,
                        Email = email,
                        Phone = phoneAsInt
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    var newEmployee = new Employees
                    {
                        UserId = newUser.Id,
                        RoleId = selectedRoleId,
                        LastName = lastName,
                        FirstName = firstName,
                        Email = email,
                        Phone = phoneAsInt,
                        DateInvite = DateTime.Now
                    };
                    db.Employees.Add(newEmployee);
                    db.SaveChanges();

                    transaction.Commit();

                    MessageBox.Show($"Работник успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    MessageBox.Show($"Критическая ошибка базы данных: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ButnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
