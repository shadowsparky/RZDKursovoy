using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;


namespace RZDKursovoy
{
    public partial class MainWindow : Window
    {
        public MySqlConnection Connected { get; private set; }
        public MySqlConnection SetConnected { set { Connected= value; } }
        private ApplicationLogic AL = new ApplicationLogic();
        
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
