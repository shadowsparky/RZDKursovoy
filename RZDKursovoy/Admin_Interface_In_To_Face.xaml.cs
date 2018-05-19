using MySql.Data.MySqlClient;
using System.Windows.Controls;

namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для Admin_Interface_In_To_Face.xaml
    /// </summary>
    public partial class Admin_Interface_In_To_Face : UserControl
    {
        public MySqlConnection Connected { get; private set; }
        public MySqlConnection SetConnected { set { Connected = value; } }
        private string Login = "";
        public string SetLogin { set { Login = value; } }
        private ApplicationLogic AL = new ApplicationLogic();

        public Admin_Interface_In_To_Face()
        {
            InitializeComponent();
        }

        public void FillUserTable()
        {
            string[] args = { "Логин", "Роль" };
            AL.FillTable(Connected, UserShow, "call ADMIN_ThrowUsers", args);
        }

        private void ControlMenu_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            FillUserTable();
        }

        private void AddUserBUTTON_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Admin_AddUser AAU = new Admin_AddUser();
            AAU.SetConnected = Connected;
            AAU.SetInterface = this;
            AAU.Show();
        }

        public void LoadUserList()
        {
            Username_BOX.Items.Clear();
            if (!AL.ComboboxFiling(Connected, "call ADMIN_ThrowUsers", Username_BOX))
            {
                AL.MessageErrorShow("При загрузке пользователей произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
            UserRole_BOX.Items.Clear();
            string[] args = { "null" };
            var r = AL.CatchStringListResult(Connected, "call ADMIN_ThrowRoles", args);
            var TMPResult = AL.En_To_Ru_Roles(r);
            for (int i = 0; i < TMPResult.Count; i++)
            {
                UserRole_BOX.Items.Add(TMPResult[i]);
            }
        }

        private void AccessTab_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadUserList();
        }

        private void UserEdit_BUTTON_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] args = { Username_BOX.Text, AL.Ru_To_En_Roles(UserRole_BOX.Text) };
            AL.MagicUniversalControlDataCatched("call ADMIN_ChangeRole", args, "UpdateUser", Connected);
            FillUserTable();
        }
    }
}
