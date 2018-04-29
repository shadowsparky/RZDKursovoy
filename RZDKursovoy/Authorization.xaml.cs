using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace RZDKursovoy
{
    public partial class Authorization : Window
    {
        private MySqlConnection _connection;
        private ApplicationLogic AL = new ApplicationLogic();
        private string UserLogin = "";
        public string ThrowLogin { get { return UserLogin; } }
        public MySqlConnection Connected { get { return _connection; } }
        public Authorization()
        {
            InitializeComponent();
            RegGrid.Visibility = Visibility.Hidden;
            AuthGrid.Visibility = Visibility.Visible;
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserLogin = loginBox.Text;
                _connection = new MySqlConnection("Database = RZD; DataSource = 127.0.0.1; User Id = " + UserLogin + "; charset=cp866; Password =" + passBox.Password);
                _connection.Open();
                string CheckRole = "#####";
                MySqlCommand checkrolecommand = new MySqlCommand("Select current_role", _connection);
                MySqlDataReader r = checkrolecommand.ExecuteReader();
                r.Read();
                try
                {
                    CheckRole = r.GetString(0);
                    r.Close();
                }
                catch (System.Exception)
                {
                }
                if (CheckRole == "user")
                {
                    MainWindow MW = new MainWindow();
                    MW.SetConnected = _connection;
                    MW.SetLogin = UserLogin;
                    this.Close();
                    MW.Show();
                    return;
                }
                else if (CheckRole == "Blocked")
                {
                    MessageBox.Show("Ваш аккаунт заблокирован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Ваш аккаунт неправильно настроен. Обратитесь к администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
            bool OK = false;
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
            if ((RegLoginBox.Text != "") && (RegPassBox.Text != ""))
            {
                if (CheckProcessingPersonalDataBox.IsChecked.Value)
                {
                    try
                    {
                        var QueryString = "call register_createuser";
                        string[] data = { RegLoginBox.Text, RegPassBox.Text };
                        AL.MagicUniversalControlData(QueryString, data, "RegAdd", FastConnect);
                        OK = true;

                    }
                    catch (MySqlException)
                    {
                        MessageBox.Show("В настоящее время сервис регистрации недоступен. Пожалуйста, попробуйте позже", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Для продолжения работы с приложением вы должны дать согласие на обработку Ваших персональных данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Вы не заполнили поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            FastConnect.Close();
            if (OK)
            { 
                RegGrid.Visibility = Visibility.Hidden;
                AuthGrid.Visibility = Visibility.Visible;
            }
        }

        private void RegExit_Click(object sender, RoutedEventArgs e)
        {
            RegGrid.Visibility = Visibility.Hidden;
            AuthGrid.Visibility = Visibility.Visible;
        }
        private void RegLoginBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.EN_InputLoginWordsProtector(RegLoginBox, e);
        }
        private void RegPassBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }
        private void loginBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.EN_InputLoginWordsProtector(loginBox, e);
        }
        private void passBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
        }
        private void loginBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }
    }
}
