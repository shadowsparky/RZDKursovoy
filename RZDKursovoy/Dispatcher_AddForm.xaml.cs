using MySql.Data.MySqlClient;
using System.Windows;

namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для Dispatcher_AddForm.xaml
    /// </summary>
    public partial class Dispatcher_AddForm : Window
    {
        private MySqlConnection _connected;
        private ApplicationLogic AL = new ApplicationLogic();
        public MySqlConnection SetConnection
        {
            set { _connected = value; }
        }
        private int Form = -1; 

        public void ShowControl(int ControlNumber)
        {
            switch(ControlNumber)
            {
                case 0:
                    Dispatcher_AddTrain DAT = new Dispatcher_AddTrain();
                    DAT.SetConnection = _connected;
                    BestWindow.Content = DAT; 
                    break;
                case 1:
                    Dispatcher_AddRailcar DAR = new Dispatcher_AddRailcar();
                    //DAR.SetConnection = _connected;
                    BestWindow.Content = DAR;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }
        }
        public Dispatcher_AddForm()
        {
            InitializeComponent();
        }
    }
}
