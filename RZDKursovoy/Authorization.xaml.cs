using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace RZDKursovoy
{
    public partial class Authorization : Window
    {
        private ApplicationLogic AL = new ApplicationLogic();
        public string ThrowLogin { get; private set; } = "";
        public MySqlConnection Connected { get; private set; }
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
                ThrowLogin = loginBox.Text;
                Connected = new MySqlConnection("Database ="+ Properties.PersonalData.Default.Database + "; " +
                    "DataSource = " + Properties.PersonalData.Default.DataSource + ";  " +
                    "User Id = " + ThrowLogin + "; charset=cp866; Password =" + passBox.Password);
                Connected.Open();
                string CheckRole = "#####";
                MySqlCommand checkrolecommand = new MySqlCommand("Select current_role", Connected);
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
                    MW.SetConnected = Connected;
                    MW.SetLogin = ThrowLogin;
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
            MySqlConnection FastConnect = new MySqlConnection("Database = " + Properties.PersonalData.Default.Database + "; " +
                    "DataSource = " + Properties.PersonalData.Default.DataSource + ";  " +
                    "User Id = " + Properties.PersonalData.Default.RegLogin + "; charset=cp866; Password = "+ Properties.PersonalData.Default.RegPassword);
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
