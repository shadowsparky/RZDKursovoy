using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RZDKursovoy
{
    public partial class Dispatcher_AddStop : UserControl
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
        public Dispatcher_AddStop()
        {
            InitializeComponent();
        }

        private void RoutName_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowRoutsNames", RoutName_BOX))
            {
                AL.MessageErrorShow("При загрузке маршрутов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }

        private void RoutName_BOX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RoutName_BOX.Text != "")
            {
                StopID.Items.Clear();
                string[] args = { RoutName_BOX.Text };
                var r = AL.CatchStringListResult(_connected, "call _DISPATCHER_ThrowStopNumber", args);
                for (int i = 0; i < r.Count; i++)
                {
                    StopID.Items.Add(r[i]);
                }
            }
        }
        private void AddStop_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            string[] args = { RoutName_BOX.Text, StopID.Text, StopName.Text, Train_Station_Name.Text };
            if (AL.TextChecking(args))
            {
                AL.MagicUniversalControlDataCatched("call DISPATCHER_AddStop", args, "AddRout", _connected);
                DIITF.TryLoadingTables();
            }
        }
        private void Train_Station_Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AL.InputWordsProtector(e);
        }
        private void StopName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AL.InputWordsProtector(e);
        }
    }
}
