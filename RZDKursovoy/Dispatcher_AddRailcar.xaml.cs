using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;


namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для Dispatcher_AddRailcar.xaml
    /// </summary>
    public partial class Dispatcher_AddRailcar : UserControl
    {
        private MySqlConnection _connected;
        public MySqlConnection SetConnection
        {
            set { _connected = value; }
        }
        private Dispatcher_Interface_In_To_Face DIITF;
        private ApplicationLogic AL = new ApplicationLogic();
        public Dispatcher_Interface_In_To_Face SetInterface
        {
            set { DIITF = value; }
        }
        public Dispatcher_AddRailcar()
        {
            InitializeComponent();
        }

        private void TrainNumber_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowTrainList", TrainNumber_BOX))
            {
                AL.MessageErrorShow("При загрузке списка поездов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }
        private void RailcarType_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowRailcarTypesList", RailcarType_BOX))
            {
                AL.MessageErrorShow("При загрузке типов вагонов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }
        private void AddRailcar_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            string[] args = { TrainNumber_BOX.Text, RailcarNumber_BOX.Text, RailcarType_BOX.Text };
            if (AL.TextChecking(args))
            {
                AL.MagicUniversalControlDataCatched("call DISPATCHER_AddRailcar", args, "AddRailcar", _connected);
                DIITF.TryLoadingTables();
            }
        }
        private void TrainNumber_BOX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TrainNumber_BOX.Text != "")
            {
                RailcarNumber_BOX.Items.Clear();
                string[] args = { TrainNumber_BOX.Text };
                var r = AL.CatchStringListResult(_connected, "call _DISPATCHER_ThrowAvailableRailcarList", args);
                for (int i = 0; i < r.Count; i++)
                {
                    RailcarNumber_BOX.Items.Add(r[i]);
                }
            }
        }
        private void TrainNumber_BOX_GotFocus(object sender, RoutedEventArgs e)
        {
            RailcarNumber_BOX.Text = "";
        }
    }
}
