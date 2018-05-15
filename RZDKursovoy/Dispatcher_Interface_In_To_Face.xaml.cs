using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Security;
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
        Dispatcher_AddForm DAF = null;

        public Dispatcher_Interface_In_To_Face()
        {
            InitializeComponent();
        }

        /*Key Up Events*/
        private void ShowTrains_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int[] activity = { 0, 0 };
            string[] args = { TMPGridRow[0].ToString() };
            if (AL.KeyUpInside(Connected, sender, e, ShowTrains, TMPGridRow, ConvertCheck, "call DISPATCHER_DropTrain", "call DISPATCHER_UpdateTrain", "DeleteTrain", "UpdateTrain",
                "При редактировании произошла ошибка. Редактировать номер поезда запрещено", args, activity))
                TryLoadingTables();
        }
        private void ShowRailcars_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int[] activity = { 0, 1 };
            var t = (DataRowView)ShowRailcars.CurrentItem;
            string[] args = { t[0].ToString(), t[1].ToString() };
            if (AL.KeyUpInside(Connected, sender, e, ShowRailcars, TMPGridRow, ConvertCheck, "call DISPATCHER_DropRailcar", "-1", "DeleteRailcar", "DontWork",
                "При редактировании произошла ошибка. Редактировать номер поезда запрещено", args, activity))
            AL.MessageErrorShow("Произошла ошибка. Редактирование вагонов временно не работает", "Ошибка");
            TryLoadingTables();
        }
        private void ShowRoutes_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int[] activity = { 0, 0 };
            var t = (DataRowView)ShowRoutes.CurrentItem;
            string[] args = { t[0].ToString() };
            if (AL.KeyUpInside(Connected, sender, e, ShowRoutes, TMPGridRow, ConvertCheck, "call DISPATCHER_DropRout", "call DISPATCHER_UpdateRout", "DeleteRout", "UpdateRout",
                "При редактировании произошла ошибка. Редактировать идентификатор маршрута запрещено", args, activity))
                TryLoadingTables();
        }
        private void ShowStops_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int[] activity = { 0, 0 };
            var t = (DataRowView)ShowStops.CurrentItem;
            string[] args = { t[0].ToString(), t[1].ToString() };
            if (AL.KeyUpInside(Connected, sender, e, ShowStops, TMPGridRow, ConvertCheck, "call DISPATCHER_DropStop", "call DISPATCHER_UpdateStop", "DeleteStop", "UpdateStop",
                "При редактировании произошла ошибка. Редактировать идентификатор маршрута запрещено", args, activity))
                TryLoadingTables();
        }
        private void ShowArrivals_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int[] activity = { 0, 0 };
            var t = (DataRowView)ShowArrivals.CurrentItem;
            string[] args = { t[1].ToString(), t[0].ToString(), t[2].ToString() };
            if (AL.KeyUpInside(Connected, sender, e, ShowArrivals, TMPGridRow, ConvertCheck, "call DISPATCHER_DropArrival", "call DISPATCHER_UpdateArrival", "DeleteArrival", "UpdateArrival",
                "При редактировании произошла неизвестная ошибка", args, activity))
                TryLoadingTables();
        }
        private void ShowDepartures_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int[] activity = { 0, 0 };
            var t = (DataRowView)ShowDepartures.CurrentItem;
            string[] args = { t[1].ToString(), t[0].ToString(), t[2].ToString() };
            if (AL.KeyUpInside(Connected, sender, e, ShowDepartures, TMPGridRow, ConvertCheck, "call DISPATCHER_DropDeparture", "call DISPATCHER_UpdateDeparture", "DeleteDeparture", "UpdateDeparture",
                "При редактировании произошла неизвестная ошибка", args, activity))
                TryLoadingTables();
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
            ConvertCheck = true;
            int[] arr = { 1 };
            ConvertCheck = AL.ConvertCheck(sender, e, arr);
            int[] itemarr = { 0 };
            TMPGridRow = AL.BlockUpdate(sender, e, itemarr);
        }
        private void ShowRoutes_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConvertCheck = true;
            int[] arr = { 1 };
            ConvertCheck = AL.ConvertCheck(sender, e, arr);
            int[] itemarr = { 0 };
            TMPGridRow = AL.BlockUpdate(sender, e, itemarr);
        }
        private void ShowStops_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConvertCheck = true;
            int[] arr = { 1 };
            ConvertCheck = AL.ConvertCheck(sender, e, arr);
            int[] itemarr = { -1 };
            TMPGridRow = AL.BlockUpdate(sender, e, itemarr);
        }
        private void ShowArrivals_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConvertCheck = true;
            int[] arr = { -1 };
            ConvertCheck = AL.ConvertCheck(sender, e, arr);
            int[] itemarr = { -1 };
            TMPGridRow = AL.BlockUpdate(sender, e, itemarr);
        }
        private void ShowDepartures_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConvertCheck = true;
            int[] arr = { -1 };
            ConvertCheck = AL.ConvertCheck(sender, e, arr);
            int[] itemarr = { -1 };
            TMPGridRow = AL.BlockUpdate(sender, e, itemarr);
        }
        /*Click Button Events*/
        private void OpenAddTrainMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetControl(0);
        }
        private void OpenAddRailcarMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetControl(1);
        }
        private void OpenAddRoutMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetControl(2);
        }
        private void OpenAddStopsMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetControl(3);
        }
        private void OpenAddArrivalMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetControl(4);
        }
        private void OpenAddDepartureMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetControl(5);
        }
        private void SetControl(int ControlID)
        {
            if (DAF != null)
            {
                if (!DAF.IsVisible)
                    DAF = null;
            }
            if (DAF == null)
            {
                DAF = new Dispatcher_AddForm();
                DAF.SetInterface = this;
                DAF.SetConnection = Connected;
                DAF.ShowControl(ControlID);
                DAF.Show();
            }
            else
            {
                DAF.SetConnection = Connected;
                DAF.SetInterface = this;
                DAF.ShowControl(ControlID);
            }
        }
        /*Загрузка данных из таблиц*/
        public void TryLoadingTables()
        {
            string[] argsTrains = { "Номер поезда", "Кол-во вагонов", "Тип поезда", "Название маршрута" };
            string[] argsRailcars = { "Номер поезда", "Номер вагона", "Тип вагона" };
            string[] argsRoutes = { "Идентификатор маршрута", "Количество остановок", "Название маршрута" };
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
