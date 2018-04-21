using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;


namespace RZDKursovoy
{
    public partial class MainWindow : Window
    {
        public MySqlConnection Connected { get; }
        
        public MainWindow()
        {
            InitializeComponent();
            Connected = new MySqlConnection("Database = RZD; DataSource = 127.0.0.1; User Id = root; charset=cp866; Password = 1111");
            try
            {
                Connected.Open();
            }
            catch (MySqlException)
            {
                MessageBox.Show("При подключении к базе произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                FindTrain_BUTTON.IsEnabled = false;
            }
            Arrival_TB.Text = "Тестовая";
            Departure_TB.Text = "Узбекистан";
            Arrival_Date.Text = "07.04.2018";
        }

        private void FindTrain_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            if ((Arrival_TB.Text != "") || (Departure_TB.Text != "") || (Arrival_Date.Text != ""))
            {
                ApplicationLogic AL = new ApplicationLogic();
                string[] Data = { Arrival_TB.Text, Departure_TB.Text, Arrival_Date.Text };
                List<string> Routs = AL.FindRout(Connected, Data);
                if (Routs.Count > 0)
                {
                    ReservationForm RF = new ReservationForm();
                    RF.SetConnection = Connected;
                    RF.SetArrival = Arrival_TB.Text;
                    RF.SetDeparture = Departure_TB.Text;
                    RF.SetDate = Arrival_Date.Text;
                    RF.SetRouts = Routs;
                    this.Close();
                    RF.Show();
                }
            } 
            else
            {
                MessageBox.Show("Вы не заполнили данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
