using MySql.Data.MySqlClient;
using System.Windows;

namespace RZDKursovoy
{
    public partial class Admin_AddUser : Window
    {
        public MySqlConnection Connected { get; private set; }
        public MySqlConnection SetConnected { set { Connected = value; } }
        public Admin_Interface_In_To_Face Admin_Interface { get; private set; }
        public Admin_Interface_In_To_Face SetInterface{ set { Admin_Interface= value; } }
        private ApplicationLogic AL = new ApplicationLogic();

        public Admin_AddUser()
        {
            InitializeComponent();
        }
        private void AddUserMenu_Loaded(object sender, RoutedEventArgs e)
        {
            UserRole_BOX.Items.Clear();
            string[] args = { "null" };
            var r = AL.CatchStringListResult(Connected, "call ADMIN_ThrowRoles", args);
            var TMPResult = AL.En_To_Ru_Roles(r);
            for (int i = 0; i < TMPResult.Count; i++)
            {
                UserRole_BOX.Items.Add(TMPResult[i]);
            }
        }
        private void AddUser_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            string[] args = { Username_BOX.Text, UserPassword.Text, AL.Ru_To_En_Roles(UserRole_BOX.Text) };
            if (AL.TextChecking(args))
            {
                AL.MagicUniversalControlDataCatched("call ADMIN_CreateUser", args, "CreateUser", Connected);
                Admin_Interface.FillUserTable();
                Admin_Interface.LoadUserList();
            }
        }

        private void Username_BOX_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(Username_BOX, e);
        }

        private void UserPassword_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(UserPassword, e);
        }

        private void Username_BOX_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.EN_InputLoginWordsProtector(Username_BOX, e);
        }

        private void UserPassword_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.EN_InputLoginWordsProtector(UserPassword, e);
        }
    }
}
