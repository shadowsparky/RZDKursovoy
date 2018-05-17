using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для Dispatcher_AddRout.xaml
    /// </summary>
    public partial class Dispatcher_AddRout : UserControl
    {
        private MySqlConnection _connected;
        public MySqlConnection SetConnection
        {
            set { _connected = value; }
        }
        private ApplicationLogic AL = new ApplicationLogic();
        private Dispatcher_Interface_In_To_Face DIITF;
        public Dispatcher_Interface_In_To_Face SetInterface
        {
            set { DIITF = value; }
        }
        public Dispatcher_AddRout()
        {
            InitializeComponent();
        }

        private void TrainRout_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowMaxStopCount", TrainRout_BOX))
            {
                AL.MessageErrorShow("При загрузке количества остановок произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }
        private void AddRout_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            string[] args = { TrainRout_BOX.Text, RoutName_BOX.Text};
            if (AL.TextChecking(args))
            {
                AL.MagicUniversalControlDataCatched("SELECT DISPATCHER_AddRout", args, "AddRout", _connected);
                DIITF.TryLoadingTables();
            }
        }
        private void RoutName_BOX_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
        }
    }
}
