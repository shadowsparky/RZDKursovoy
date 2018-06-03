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
                    "User Id = " + ThrowLogin + "; charset=cp866; SslMode=none; Password =" + passBox.Password);
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
                    Menu menu = new Menu();
                    menu.SetConnected = Connected;
                    menu.SetLogin = ThrowLogin;
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(menu);
                    menu.BoxesFiling();
                    return;
                }
                else if (CheckRole == "Blocked")
                {
                    AL.MessageErrorShow("Ваш аккаунт заблокирован", "Ошибка");
                    return;
                }
                else if (CheckRole == "RZD_Dispatcher")
                {
                    Dispatcher_Interface_In_To_Face DITF = new Dispatcher_Interface_In_To_Face();
                    DITF.SetConnected = Connected;
                    DITF.SetLogin = ThrowLogin;
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(DITF);
                    AL.MessageShow("Привет, диспетчер " + ThrowLogin, "Привет!");
                    return;
                }
                else if (CheckRole == "Admin")
                {
                    Admin_Interface_In_To_Face AIITF = new Admin_Interface_In_To_Face();
                    AIITF.SetConnected = Connected;
                    AIITF.SetLogin = ThrowLogin;
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(AIITF);
                    AL.MessageShow("Здравствуйте, администратор " + ThrowLogin, "Здравствуйте");
                    return;
                }
                else
                {
                    AL.MessageErrorShow("Ваш аккаунт неправильно настроен. Обратитесь к администратору", "Ошибка");
                    return;
                }
            }
            catch (MySqlException ex)
            {
                AL.MessageErrorShow("Вы ввели неправильный логин или пароль", "Ошибка");
                return;
            }
        }
        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            AuthGrid.Visibility = Visibility.Hidden;
            RegGrid.Visibility = Visibility.Visible;
        }
        private void RegLoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //AL.InputProtector(e, RegLoginBox);
        }
        private void RegPassBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        } 
        private void RegRegButtonBEEP_Click(object sender, RoutedEventArgs e)
        {
            bool OK = false;
            MySqlConnection FastConnect = new MySqlConnection("Database = " + Properties.PersonalData.Default.Database + "; " +
                    "DataSource = " + Properties.PersonalData.Default.DataSource + ";  " +
                    "User Id = " + Properties.PersonalData.Default.RegLogin + "; charset=cp866; SslMode=none; Password = " + Properties.PersonalData.Default.RegPassword);
            try
            {
                FastConnect.Open();
            }
            catch (MySqlException)
            {
                AL.MessageErrorShow("Сервер не отвечает", "Ошибка");
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
                        var res = AL.MagicUniversalControlData(QueryString, data, "RegAdd", FastConnect);
                        poselki.BestErrors BE = new poselki.BestErrors();
                        BE.CatchError(res);
                        OK = true;

                    }
                    catch (MySqlException)
                    {
                        AL.MessageErrorShow("В настоящее время сервис регистрации недоступен. Пожалуйста, попробуйте позже", "Ошибка");
                    }
                    finally
                    {
                        FastConnect.Close();
                    }
                }
                else
                {
                    AL.MessageErrorShow("Для продолжения работы с приложением вы должны дать согласие на обработку Ваших персональных данных", "Ошибка");
                }
            }
            else
            {
                AL.MessageErrorShow("Вы не заполнили поля", "Ошибка");
            }
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

        private void RegLoginBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(RegLoginBox, e);
        }

        private void RegLoginBox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
        }

        private void RegPassBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
           AL.DontCtrlVAndSpace(RegPassBox, e);
        }

        private void loginBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(loginBox, e);
        }

        private void passBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        { 
        }
    }
}
