using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Controls;


namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для Dispatcher_Interface_In_To_Face.xaml
    /// </summary>
    public partial class Dispatcher_Interface_In_To_Face : UserControl
    {
        public MySqlConnection Connected { get; private set; }
        public MySqlConnection SetConnected { set { Connected = value; } }
        private string Login = "";
        public string SetLogin { set { Login = value; } }
        private ApplicationLogic AL = new ApplicationLogic();
        private DataRowView TMPGridRow = null;
        private bool ConvertCheck = true;

        public Dispatcher_Interface_In_To_Face()
        {
            InitializeComponent();
        }

        private void ShowTrains_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AL.ShowTrainsTableFill(Connected, ShowTrains);
        }
        private void ShowTrains_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                var r = e.Key.ToString();
                if (r == "Delete")
                {
                    var t = (DataRowView)ShowTrains.CurrentItem;
                    string[] args = { t[0].ToString() };
                    var res = AL.MagicUniversalControlData("call DISPATCHER_DropTrain", args, "DeleteTrain", Connected);
                    poselki.BestErrors BE = new poselki.BestErrors();
                    BE.CatchError(res);
                    AL.ShowTrainsTableFill(Connected, ShowTrains);
                }
                else if (r == "Return")
                {
                    var t1 = TMPGridRow;
                    var t = (DataRowView)ShowTrains.CurrentItem;
                    if (t1 != null)
                    {
                        if (ConvertCheck)
                        {
                            AL.MagicUserControl(Connected, t1, "call DISPATCHER_UpdateTrain", "UpdateTrain");
                        }
                        AL.ShowTrainsTableFill(Connected, ShowTrains);
                    }
                    else
                    {
                        AL.MessageErrorShow("Номера поездов не подлежат редактированию", "Ошибка");
                        AL.ShowTrainsTableFill(Connected, ShowTrains);
                    }
                }
                else if (r == "Escape")
                {
                    AL.ShowTrainsTableFill(Connected, ShowTrains);
                }
            }
            catch(Exception)
            { }
        }
        private void ShowTrains_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConvertCheck = true;
            int[] arr = { 1 };
            ConvertCheck = AL.ConvertCheck(sender, e, arr);
            int[] itemarr = { 0 };
            TMPGridRow = AL.BlockUpdate(sender, e, itemarr);
        }
    }
}
