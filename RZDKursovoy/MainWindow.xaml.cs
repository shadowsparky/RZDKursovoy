using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Windows;


namespace RZDKursovoy
{
    public partial class MainWindow : Window
    {
        public MySqlConnection Connected { get; private set; }
        public MySqlConnection SetConnected { set { Connected= value; } }
        private ApplicationLogic AL = new ApplicationLogic();
        private string Login = "";
        public string SetLogin { set { Login = value; } }
        private int AvailableTicketsCount = new int();
        private int CurrentTicketID = new int();
        
        public MainWindow()
        {
            InitializeComponent();
        }
        public void CheckActivateCabinet()
        {
            AvailableTicketsCount = AL.throwCountAvailableTickets(Connected, Login + "@localhost");
            if (AvailableTicketsCount == 0)
            {
                LK_Empty.Visibility = Visibility.Visible;
                LK_NotEmpty.Visibility = Visibility.Collapsed;
            }
            else
            {
                LK_Empty.Visibility = Visibility.Collapsed;
                LK_NotEmpty.Visibility = Visibility.Visible;
                MySqlDataAdapter ad = new MySqlDataAdapter();
                string Query = "call throwAvailableTicketsWithInfo('" + Login + "@localhost')";
                ad.SelectCommand = new MySqlCommand(Query, Connected);
                System.Data.DataTable table = new System.Data.DataTable();
                ad.Fill(table);
                table.Columns[0].ColumnName = "Номер билета";
                table.Columns[1].ColumnName = "Номер поезда";
                table.Columns[2].ColumnName = "Номер вагона";
                table.Columns[3].ColumnName = "Номер места";
                table.Columns[4].ColumnName = "Время отправления";
                table.Columns[5].ColumnName = "Дата отправления";
                table.Columns[6].ColumnName = "Станция отправления";
                table.Columns[7].ColumnName = "Время прибытия";
                table.Columns[8].ColumnName = "Дата прибытия";
                table.Columns[9].ColumnName = "Станция прибытия";
                ShowBuyedTickets.ItemsSource = table.DefaultView;
            }
        }
        private void FindTrain_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            if ((Arrival_BOX.Text != "") && (Departure_BOX.Text != "") && (Departure_BOX.Text != ""))
            {
                ApplicationLogic AL = new ApplicationLogic();
                string[] Data = { Arrival_BOX.Text, Departure_BOX.Text, Arrival_Date.Text };
                List<string> Routs = AL.FindRout(Connected, Data);
                if (Routs.Count > 0)
                {
                    List<string> TrainsList = new List<string>();
                    for (int i = 0; i < Routs.Count; i++)
                    {
                         TrainsList = AL.newFindTrainList(Connected, Routs[i], Arrival_BOX.Text, Arrival_Date.Text);
                    }
                    if (TrainsList.Count > 0)
                    {
                        ReservationForm RF = new ReservationForm();
                        RF.SetConnection = Connected;
                        RF.SetArrival = Arrival_BOX.Text;
                        RF.SetDeparture = Departure_BOX.Text;
                        RF.SetDate = Arrival_Date.Text;
                        RF.SetRouts = Routs;
                        RF.SetTrainsList = TrainsList;
                        RF.SetMainWindow = this;
                        this.Hide();
                        RF.Show();
                    }
                    else
                    {
                        MessageBox.Show("К сожалению, поездов по нужному Вам маршруту в данное время нет. Попробуйте выбрать другой день.", "=(", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("К сожалению, поездов по нужному Вам маршруту в данное время нет. Попробуйте выбрать другой день.", "=(", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Вы не заполнили данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ControlMenu_Loaded(object sender, RoutedEventArgs e)
        {
            var a = AL.TrowAllStations(Connected);
            for (int i = 0; i < a.Count; i++)
            {
                Arrival_BOX.Items.Add(a[i]);
                Departure_BOX.Items.Add(a[i]);
            }
            CheckActivateCabinet();
        }
        private void Arrival_BOX_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (Arrival_BOX.Text.Length < 30)
            {
                AL.InputWordsProtector(e);
            }
            else
                e.Handled = true;
        }
        private void Departure_BOX_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (Departure_BOX.Text.Length < 30)
            {
                AL.InputWordsProtector(e);
            }
            else
                e.Handled = true;
        }
        private void PrintTicketBUTTON_Click(object sender, RoutedEventArgs e)
        {


        }
        private void CancelTripBUTTON_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTicketID != 0)
            {
                try
                {
                    string Query = "call CancelTicket";
                    string[] args = { CurrentTicketID.ToString() };
                    AL.MagicUniversalControlData(Query, args, "DeleteTicket", Connected);
                    CheckActivateCabinet();
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Во время отмены произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Для начала вы должны выбрать билет, который хотите отменить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ShowBuyedTickets_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var t = (DataRowView)ShowBuyedTickets.CurrentItem;
                CurrentTicketID = System.Convert.ToInt32(t[0].ToString());
            }
            catch(System.Exception)
            { }
        }
    }
}
