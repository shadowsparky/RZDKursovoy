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
                    AL.MagicUniversalControlData("call DISPATCHER_DropTrain", args, "DeleteTrain", Connected);
                }
                else if (r == "Return")
                {
                    var t = (DataRowView) (sender as DataGrid).CurrentItem;
                    var t1 = TMPGridRow;
                    if (t1[0].ToString() == t[0].ToString())
                    {
                        string[] args = new string[t.Row.ItemArray.Length];
                        for (int i = 0; i < t.Row.ItemArray.Length; i++)
                            args[i] = t.Row.ItemArray[i].ToString();
                        AL.MagicUniversalControlData("call DISPATCHER_UpdateTrain", args, "UpdateTrain", Connected);
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
            try
            {
                TMPGridRow = (DataRowView)ShowTrains.CurrentItem;
            }
            catch (Exception)
            { }
        }
    }
}
