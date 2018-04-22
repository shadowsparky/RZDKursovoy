using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;


namespace RZDKursovoy
{
    public partial class MainWindow : Window
    {
        public MySqlConnection Connected { get; private set; }
        public MySqlConnection SetConnected { set { Connected= value; } } 
        
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
