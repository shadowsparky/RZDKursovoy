using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RZDKursovoy
{
    public partial class ReservationForm : Window
    {
        private string ArrivalStation = "###";
        private string DepartureStation = "###";
        private string ArrivalDate = "###";
        private int Arrival_Stop_ID = new int();
        private List<string> Routs = new List<string>();
        private MySqlConnection _connection;
        private int Arrival_ID = new int();
        private int Departure_ID = new int();
        private string CurrentTrainNumber = "";
        private ApplicationLogic AL = new ApplicationLogic();

        public string SetArrival { set { ArrivalStation = value; } }
        public string SetDeparture { set { DepartureStation = value; } }
        public string SetDate { set { ArrivalDate = value; } }
        public List<string> SetRouts { set { Routs = value; } }
        public MySqlConnection SetConnection { set { _connection = value; } }

        public ReservationForm()
        {
            InitializeComponent();
            ChooseTrainNextButton.IsEnabled = false;
        }

        private void ChooseTrainListBox_Loaded(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < Routs.Count; i++)
            {
                var GetASI = new MySqlCommand("select ThrowArrivalStopID(" + Routs[i] + ")", _connection);
                var r = GetASI.ExecuteReader();
                r.Read();
                Arrival_Stop_ID = r.GetInt32(0);
                r.Close();
                var TrainNum = AL.FindTrain(_connection, Routs[i], Arrival_Stop_ID, ArrivalDate);
                if (TrainNum != "-1")
                {
                    Arrival_ID = AL.GetArrivalID(_connection, ArrivalStation, Convert.ToInt32(Routs[i]), TrainNum);
                    Departure_ID = AL.GetDepartureID(_connection, DepartureStation, Convert.ToInt32(Routs[i]), TrainNum);
                    var TrainData = AL.TrainInfo(_connection, TrainNum, Arrival_ID, Departure_ID);
                    ChooseTrainListBox.Items.Add("№ поезда - "+ TrainNum + ". Время отправления - " + TrainData[0] + ". Дата прибытия - " + TrainData[2] + ", время прибытия - " + TrainData[1]);
                }
            }
        }

        private void ChooseTrainListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var tCurrentTrainNumber = ChooseTrainListBox.SelectedItem.ToString().Split('.');
            var TMPSplitTrainNum = tCurrentTrainNumber[0].Split(' ');
            CurrentTrainNumber = TMPSplitTrainNum[3];
            ChooseTrainNextButton.IsEnabled = true;
        }

        private void ChooseTrainNextButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseTrainBox.Visibility = Visibility.Hidden;
            ChooseTrainTypeBox.Visibility = Visibility.Visible;
            AddToRailcarTypesBox();
        }

        private void AddToRailcarTypesBox()
        {
            var Data = AL.Available_Railcar_Types(_connection, CurrentTrainNumber);
            for (int i = 0; i < Data.Count; i++)
            {
                ChooseRailcarType.Items.Add(Data[i]);
            }
        }
    }
}
