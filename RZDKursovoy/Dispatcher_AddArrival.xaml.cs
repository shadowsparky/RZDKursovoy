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
    public partial class Dispatcher_AddArrival : UserControl
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
        public Dispatcher_AddArrival()
        {
            InitializeComponent();
        }
        private void RoutName_BOX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RoutName_BOX.Text != "")
            {
                StopName_BOX.Items.Clear();
                Train_Number_BOX.Items.Clear();
                string[] args = { RoutName_BOX.Text };
                try
                {
                    var r = AL.CatchStringListResult(_connected, "call _DISPATCHER_ThrowAvailableStopsFindByRout", args);
                    for (int i = 0; i < r.Count; i++)
                    {
                        StopName_BOX.Items.Add(r[i]);
                    }
                }
                catch (Exception)
                {
                    AL.MessageErrorShow("При загрузке остановок произошла ошибка", "Ошибка");
                    this.IsEnabled = false;
                    return;
                }
                try
                {
                    var r2 = AL.CatchStringListResult(_connected, "call _DISPATCHER_ThrowAvailableTrainsFindByRout", args);
                    for (int i = 0; i < r2.Count; i++)
                    {
                        Train_Number_BOX.Items.Add(r2[i]);
                    }
                }
                catch (Exception)
                {
                    AL.MessageErrorShow("При загрузке поездов произошла ошибка", "Ошибка");
                    this.IsEnabled = false;
                    return;
                }
            }
        }
        private void RoutName_BOX_Loaded(object sender, RoutedEventArgs e)
        {
            RoutName_BOX.Items.Clear();
            if (!AL.ComboboxFiling(_connected, "call _DISPATCHER_ThrowRoutsNames", RoutName_BOX))
            {
                AL.MessageErrorShow("При загрузке маршрутов произошла ошибка", "Ошибка");
                this.IsEnabled = false;
            }
        }
        private void AddArrival_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            string[] args = { RoutName_BOX.Text, StopName_BOX.Text, Train_Number_BOX.Text, ArrivalTime.Text, ArrivalDate.Text };
            if (AL.TextChecking(args))
            {
                AL.MagicUniversalControlDataCatched("call DISPATCHER_AddArrival", args, "AddArrival", _connected);
                DIITF.TryLoadingTables();
            }
        }
    }
}
