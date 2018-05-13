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

        /*Key Up Events*/
        private void ShowTrains_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (AL.KeyUpInside(Connected, sender, e, ShowTrains, TMPGridRow, ConvertCheck, "call DISPATCHER_DropTrain", "call DISPATCHER_UpdateTrain", "DeleteTrain", "UpdateTrain", 
                "При редактировании произошла ошибка. Редактировать номер поезда запрещено"))
                TryLoadingTables();
        }
        private void ShowRailcars_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
        private void ShowRoutes_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
        private void ShowStops_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
        private void ShowArrivals_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
        private void ShowDepartures_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
        /*Cell Edit Ending Events*/
        private void ShowTrains_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConvertCheck = true;
            int[] arr = { 1 };
            ConvertCheck = AL.ConvertCheck(sender, e, arr);
            int[] itemarr = { 0 };
            TMPGridRow = AL.BlockUpdate(sender, e, itemarr);
        }
        private void ShowRailcars_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
        private void ShowRoutes_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
        private void ShowStops_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
        private void ShowArrivals_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
        private void ShowDepartures_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
        /*Загрузка данных из таблиц*/
        private void TryLoadingTables()
        {
            string[] argsTrains = { "Номер поезда", "Кол-во вагонов", "Тип поезда", "Название маршрута" };
            string[] argsRailcars = { "Номер поезда", "Номер вагона", "Тип вагона" };
            string[] argsRoutes = { "Название маршрута", "Количество остановок" };
            string[] argsStops = { "Название маршрута", "Номер остановки", "Название остановки", "Название вокзала" };
            string[] argsArrivals = { "Название остановки", "Название маршрута", "Номер поезда", "Время прибытия", "Дата прибытия" };
            string[] argsDepartures = { "Название остановки", "Название маршрута", "Номер поезда", "Время отправления", "Дата отправления" };
            try
            {
                if (!AL.FillTable(Connected, ShowTrains, "call DISPATCHER_ShowTrains", argsTrains))
                    throw new ApplicationException();

                if (!AL.FillTable(Connected, ShowRailcars, "call DISPATCHER_ShowRailcars", argsRailcars))
                    throw new ApplicationException();

                if (!AL.FillTable(Connected, ShowRoutes, "call DISPATCHER_ShowRoutes", argsRoutes))
                    throw new ApplicationException();

                if (!AL.FillTable(Connected, ShowStops, "call DISPATCHER_ShowStops", argsStops))
                    throw new ApplicationException();

                if (!AL.FillTable(Connected, ShowArrivals, "call DISPATCHER_ShowArrivals", argsArrivals))
                    throw new ApplicationException();

                if (!AL.FillTable(Connected, ShowDepartures, "call DISPATCHER_ShowDepartures", argsDepartures))
                    throw new ApplicationException();
            }
            catch (ApplicationException)
            {
                ExecutableGrid.IsEnabled = false;
                AL.MessageErrorShow("Соединение сервером было внезапно разорвано", "Ошибка");
            }
        }
        private void TryLoading(object sender, System.Windows.RoutedEventArgs e)
        {
            TryLoadingTables();
        }
        //private void RefreshList_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    try
        //    {
        //        MySqlDataAdapter ad = new MySqlDataAdapter();
        //        ad.SelectCommand = new MySqlCommand("call ADMIN_KursovoyTable", Connected);
        //        DataTable table = new DataTable();
        //        ad.Fill(table);
        //        table.Columns[0].ColumnName = "Атрибут";
        //        table.Columns[1].ColumnName = "Описание";
        //        table.Columns[2].ColumnName = "Тип данных";
        //        table.Columns[3].ColumnName = "Дополнительно";
        //        //ShowTable.ItemsSource = table.DefaultView;
        //    }
        //    catch (Exception)
        //    {
        //        AL.MessageErrorShow("ERROR", "1337");
        //    }
        //}
    }
}
