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
        private Dispatcher_Interface_In_To_Face DIITF;
        public Dispatcher_Interface_In_To_Face SetInterface
        {
            set { DIITF = value; }
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
        private void AddTrain_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            string[] args = { TrainNumber_BOX.Text, RailcarCount_BOX.Text, TrainType_BOX.Text, TrainRout_BOX.Text };
            if (AL.TextChecking(args))
            {
                AL.MagicUniversalControlDataCatched("call DISPATCHER_AddTrain", args, "AddTrain", _connected);
                DIITF.TryLoadingTables();
            }
        }
        private void TrainType_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowTrainTypes", TrainType_BOX))
            {
                AL.MessageErrorShow("При загрузке типов вагонов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }
        private void TrainRout_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowRoutsNames", TrainRout_BOX))
            {
                AL.MessageErrorShow("При загрузке маршрутов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }

        private void TrainNumber_BOX_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(TrainNumber_BOX, e);
        }

        private void TrainType_BOX_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (TrainType_BOX.Text.Length < 30)
            {
                AL.InputWordsProtector(e);
            }
            else
                e.Handled = true;
        }

        private void TrainType_BOX_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(TrainType_BOX, e);
        }
    }
}
