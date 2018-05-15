using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;


namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для Dispatcher_AddTrain.xaml
    /// </summary>
    public partial class Dispatcher_AddTrain : UserControl
    {
        private MySqlConnection _connected;
        private ApplicationLogic AL = new ApplicationLogic();
        public MySqlConnection SetConnection
        {
            set { _connected = value; }
        }

        public Dispatcher_AddTrain()
        {
            InitializeComponent();
        }
        private void RailcarCount_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowRailcarListCount", RailcarCount_BOX))
            {
                AL.MessageErrorShow("При загрузке количества вагонов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }
        private void RailcarRout_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowRoutsNames", RailcarRout_BOX))
            {
                AL.MessageErrorShow("При загрузке маршрутов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }
        private void RailcarType_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowTrainTypes", RailcarType_BOX))
            {
                AL.MessageErrorShow("При загрузке типов вагонов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }

        private void AddTrain_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            string[] args = { TrainNumber_BOX.Text, RailcarCount_BOX.Text, RailcarRout_BOX.Text, RailcarType_BOX.Text };
            if (AL.TextChecking(args))
            {
                AL.MagicUniversalControlDataCatched("call DISPATCHER_AddTrain", args, "AddTrain", _connected);
            }
        }
    }
}
