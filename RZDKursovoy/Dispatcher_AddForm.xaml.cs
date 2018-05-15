using MySql.Data.MySqlClient;
using System.Windows;

namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для Dispatcher_AddForm.xaml
    /// </summary>
    public partial class Dispatcher_AddForm : Window
    {
        private ApplicationLogic AL = new ApplicationLogic();
        private MySqlConnection _connected;
        public MySqlConnection SetConnection
        {
            set { _connected = value; }
        }
        private Dispatcher_Interface_In_To_Face DIITF;
        public Dispatcher_Interface_In_To_Face SetInterface
        {
            set { DIITF = value; }
        }
        public void ShowControl(int ControlNumber)
        {
            switch(ControlNumber)
            {
                case 0:
                    Dispatcher_AddTrain DAT = new Dispatcher_AddTrain();
                    DAT.SetConnection = _connected;
                    DAT.SetInterface = DIITF;
                    BestWindow.Content = DAT; 
                    break;
                case 1:
                    Dispatcher_AddRailcar DAR = new Dispatcher_AddRailcar();
                    DAR.SetConnection = _connected;
                    DAR.SetInterface = DIITF;
                    BestWindow.Content = DAR;
                    break;
                case 2:
                    Dispatcher_AddRout _DAR = new Dispatcher_AddRout();
                    _DAR.SetConnection = _connected;
                    _DAR.SetInterface = DIITF;
                    BestWindow.Content = _DAR;
                    break;
                case 3:
                    Dispatcher_AddStop DAS = new Dispatcher_AddStop();
                    DAS.SetConnection = _connected;
                    DAS.SetInterface = DIITF;
                    BestWindow.Content = DAS;
                    break;
                case 4:
                    Dispatcher_AddArrival DAA = new Dispatcher_AddArrival();
                    DAA.SetConnection = _connected;
                    DAA.SetInterface = DIITF;
                    BestWindow.Content = DAA;
                    break;
                case 5:
                    Dispatcher_AddDeparture DAD = new Dispatcher_AddDeparture();
                    DAD.SetConnection = _connected;
                    DAD.SetInterface = DIITF;
                    BestWindow.Content = DAD;
                    break;
            }
        }
        public Dispatcher_AddForm()
        {
            InitializeComponent();
        }
    }
}
