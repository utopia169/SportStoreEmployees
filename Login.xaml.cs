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
    public partial class Login : Window
    {
        private SportStoreEmployees1Entities db = new SportStoreEmployees1Entities();
        public Login()
        {
            InitializeComponent();
        }

        private void ButnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show($"Неверный логин", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoginEror.Visibility = Visibility.Visible;
                return;
            }
            if (txtPassword.Text == "")
            {
                MessageBox.Show($"Введите пароль", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoginEror.Visibility = Visibility.Visible;
                return;
            }
            try
            {
                var user = db.Users.FirstOrDefault(u => u.Login == txtPassword.Text);

                if (user == null)
                {
                    MessageBox.Show($"Пользователь не найден", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoginEror.Visibility = Visibility.Visible;
                    return;
                }
                if (user.Password == txtPassword.Text)
                {
                    new MainWindow().Show();
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show($"Неверный пароль", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoginEror.Visibility = Visibility.Visible;
                    return;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка подключения к бд{ex.Message}");
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }

}
