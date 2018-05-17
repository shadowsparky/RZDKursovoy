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
    /// Логика взаимодействия для Dispatcher_AddArrival.xaml
    /// </summary>
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
        private void StopName_BOX_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void RoutName_BOX_LostFocus(object sender, RoutedEventArgs e)
        {
            // Добавление 
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
    }
}
