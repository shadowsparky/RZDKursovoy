using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace RZDKursovoy
{
    public partial class Authorization : Window
    {
        private MySqlConnection _connection;
        private ApplicationLogic AL = new ApplicationLogic();
        public MySqlConnection Connected { get { return _connection; } }
        public Authorization()
        {
            InitializeComponent();
            RegGrid.Visibility = Visibility.Hidden;
            AuthGrid.Visibility = Visibility.Visible;
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            _connection = new MySqlConnection("Database = RZD; DataSource = 127.0.0.1; User Id = "+ loginBox.Text + "; charset=cp866; Password =" + passBox.Password);
            try
            {
                Connected.Open();
                MainWindow MW = new MainWindow();
                MW.SetConnected = _connection;
                this.Hide();
                MW.Show();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Вы ввели неправильный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            AuthGrid.Visibility = Visibility.Hidden;
            RegGrid.Visibility = Visibility.Visible;
        }
        private void RegLoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AL.InputProtector(e, RegLoginBox);
        }
        private void RegPassBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AL.InputProtector(e, RegPassBox);
        } 
        private void RegRegButtonBEEP_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection FastConnect = new MySqlConnection("Database = RZD; DataSource = 127.0.0.1; User Id = RegMaster; charset=cp866; Password = RegMasterPassword");
            try
            {
                FastConnect.Open();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Сервер не отвечает", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if ((RegLoginBox.Text != "") || (RegPassBox.Text != ""))
            {
                try
                {
                    var QueryString = "call register_createuser";
                    string[] data = { RegLoginBox.Text, RegPassBox.Text };
                    AL.MagicUniversalControlData(QueryString, data, "RegAdd", FastConnect);
                }
                catch(MySqlException)
                { return; }
            }
            else
            {
                MessageBox.Show("Вы не заполнили поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            RegGrid.Visibility = Visibility.Hidden;
            AuthGrid.Visibility = Visibility.Visible;
        }
    }
}
