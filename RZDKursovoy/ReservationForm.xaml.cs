using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RZDKursovoy
{
    public partial class ReservationForm : Window
    {
        /*Переменные*/
        private string ArrivalStation = "###";
        private string DepartureStation = "###";
        private string ArrivalDate = "###";
        private int Arrival_Stop_ID = new int();
        private List<string> Routs = new List<string>();
        private MySqlConnection _connection;
        private int Arrival_ID = new int();
        private int Departure_ID = new int();
        private int Railcar_Number = new int();
        private string CurrentTrainNumber = "";
        private string ChoosedRailcarType = "";
        private int ChoosedSeatNumber = new int();
        private int Passenger_Number = new int();
        private ApplicationLogic AL = new ApplicationLogic();
        private poselki.BestErrors Errors = new poselki.BestErrors();
        /*Проперти*/
        public string SetArrival { set { ArrivalStation = value; } }
        public string SetDeparture { set { DepartureStation = value; } }
        public string SetDate { set { ArrivalDate = value; } }
        public List<string> SetRouts { set { Routs = value; } }
        public MySqlConnection SetConnection { set { _connection = value; } }
        /*Процедуры*/
        public ReservationForm()
        {
            InitializeComponent();
            ChooseTrainBox.Visibility = Visibility.Visible;
            ChooseTrainTypeBox.Visibility = Visibility.Hidden;
            InputPersonalDataBox.Visibility = Visibility.Hidden;
            StepTwoGrid.IsEnabled = false;
            StepThreePlusGrid.IsEnabled = false;
            StepThreeGrid.IsEnabled = false;
        }
        private bool ThrowTrainListToTable()
        {
            try
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
                        MySqlCommand BestCommand = new MySqlCommand("call TrainInfoTwo(@TrainNumber, @Arrival_ID_IN, @Departure_ID_IN)", _connection);
                        BestCommand.Parameters.AddWithValue("TrainNumber", TrainNum);
                        BestCommand.Parameters.AddWithValue("Arrival_ID_IN", Arrival_ID);
                        BestCommand.Parameters.AddWithValue("Departure_ID_IN", Departure_ID);
                        MySqlDataAdapter ad = new MySqlDataAdapter();
                        ad.SelectCommand = BestCommand;
                        System.Data.DataTable table = new System.Data.DataTable();
                        ad.Fill(table);
                        table.Columns[0].ColumnName = "Номер поезда";
                        table.Columns[1].ColumnName = "Время отправления";
                        table.Columns[2].ColumnName = "Время прибытия";
                        table.Columns[3].ColumnName = "Дата прибытия";
                        table.Columns[4].ColumnName = "Начальная остановка";
                        table.Columns[5].ColumnName = "Конечная остановка";
                        ChooseTrainGRID.ItemsSource = table.DefaultView;
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        private void ChooseTrainListBox_Loaded(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < Routs.Count; i++)
            //{
            //    var GetASI = new MySqlCommand("select ThrowArrivalStopID(" + Routs[i] + ")", _connection);
            //    var r = GetASI.ExecuteReader();
            //    r.Read();
            //    Arrival_Stop_ID = r.GetInt32(0);
            //    r.Close();
            //    var TrainNum = AL.FindTrain(_connection, Routs[i], Arrival_Stop_ID, ArrivalDate);
            //    if (TrainNum != "-1")
            //    {
            //        Arrival_ID = AL.GetArrivalID(_connection, ArrivalStation, Convert.ToInt32(Routs[i]), TrainNum);
            //        Departure_ID = AL.GetDepartureID(_connection, DepartureStation, Convert.ToInt32(Routs[i]), TrainNum);
            //        var TrainData = AL.TrainInfo(_connection, TrainNum, Arrival_ID, Departure_ID);
            //        //ChooseTrainListBox.Items.Add("№ поезда - "+ TrainNum + ". Время отправления - " + TrainData[0] + ". Дата прибытия - " + TrainData[2] + ", время прибытия - " + TrainData[1]);
            //    }
            //}
        }
        private void ChooseTrainNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTrainNumber != "")
            {
                ChooseTrainBox.Visibility = Visibility.Hidden;
                ChooseTrainTypeBox.Visibility = Visibility.Visible;
                AddToRailcarTypesBox();
            }
            else
            {
                MessageBox.Show("Вы должны выбрать поезд", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
        private void AddToRailcarTypesBox()
        {
            var Data = AL.Available_Railcar_Types(_connection, CurrentTrainNumber);
            for (int i = 0; i < Data.Count; i++)
            {
                ChooseRailcarType.Items.Add(Data[i]);
            }
        }
        private void ChooseRailcarTypeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChooseRailcarNumberBox.Items.Clear();
                SeatChooseNumberBox.Items.Clear();
                var r = ChooseRailcarType.SelectedItem.ToString().Split(' ');
                ChoosedRailcarType = r[0];
                var Data = AL.ThrowTrainNumbersList(_connection, CurrentTrainNumber, ChoosedRailcarType);
                for (int i = 0; i < Data.Count; i++)
                {
                    ChooseRailcarNumberBox.Items.Add(Convert.ToString(Data[i]));
                }
                if (ChoosedRailcarType == "Плацкартный")
                {
                    TrainCardImage.Source = new BitmapImage(new Uri(@"/Images/PlackartScheme.png", UriKind.Relative));
                }
                else if (ChoosedRailcarType == "Купе")
                {
                    TrainCardImage.Source = new BitmapImage(new Uri(@"/Images/CupeScheme.png", UriKind.Relative));
                } 
                else if (ChoosedRailcarType == "СВ")
                {
                    TrainCardImage.Source = new BitmapImage(new Uri(@"/Images/SvScheme.png", UriKind.Relative));
                }
                StepTwoGrid.IsEnabled = true;
                StepThreeGrid.IsEnabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Вы должны выбрать тип вагона", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
        private void ChooseRailcarNumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SeatChooseNumberBox.Items.Clear();
                Railcar_Number = Convert.ToInt32(ChooseRailcarNumberBox.SelectedItem.ToString());
                StepThreePlusGrid.IsEnabled = true;
                var k = AL.Available_For_Planting_Seats(_connection, CurrentTrainNumber, Railcar_Number, Arrival_ID, Departure_ID);
                for (int i = 0; i < k.Count; i++)
                    SeatChooseNumberBox.Items.Add(k[i]);
            }
            catch (Exception)
            {
                MessageBox.Show("Вы должны выбрать номер вагона", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
        private void SeatChooseNumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChoosedSeatNumber = Convert.ToInt32(SeatChooseNumberBox.SelectedItem.ToString());
                ChooseTrainTypeBox.Visibility = Visibility.Hidden;
                InputPersonalDataBox.Visibility = Visibility.Visible;
            }
            catch(Exception)
            {
                MessageBox.Show("Вы должны выбрать место", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            
        }
        private void InputData_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                //var QueryString = "call EmployPlaces";
                //Passenger_Number = AL.FindPassenger(_connection, Convert.ToInt32(PassSeries.Text), Convert.ToInt32(PassNumber.Text));
                //if ((RegNameBox.Text != "") && (RegFamBox.Text != "") && (PassSeries.Text != "") && (PassNumber.Text != ""))
                //{
                //    if (Passenger_Number == -1)
                //    {
                //        var PasNewNum = AL.PassengerAddToDB(_connection, RegFamBox.Text, RegNameBox.Text, RegPathBox.Text, Convert.ToInt32(PassSeries.Text), Convert.ToInt32(PassNumber.Text), RegPhoneBox.Text);
                //        string[] data = { CurrentTrainNumber, Railcar_Number.ToString(), ChoosedSeatNumber.ToString(), PasNewNum.ToString(), Arrival_ID.ToString(), Departure_ID.ToString() };
                //        AL.MagicUniversalControlData(QueryString, data, "Reservation", _connection);
                //    }
                //    else
                //    {
                //        var ExistsData = AL.FindPassengerWithPersonalData(_connection, Convert.ToInt32(PassSeries.Text), Convert.ToInt32(PassNumber.Text));
                //        if ((RegFamBox.Text == ExistsData[0]) && (RegNameBox.Text == ExistsData[1]) && (RegPathBox.Text == ExistsData[2]))
                //        {
                //            string[] data = { CurrentTrainNumber, Railcar_Number.ToString(), ChoosedSeatNumber.ToString(), Passenger_Number.ToString(), Arrival_ID.ToString(), Departure_ID.ToString() };
                //            AL.MagicUniversalControlData(QueryString, data, "Reservation", _connection);
                //        }
                //        else
                //        {
                //            MessageBox.Show("Информация о существующем в базе пассажире заполнена неверно. Полиция уже рядом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //        }
                //    }
                MessageBox.Show("test");
                //}
                //else
                //{
                //    MessageBox.Show("Вы не заполнили одно или несколько полей, необходимых для регистрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            //}
            //catch(Exception)
            //{
            //    MessageBox.Show("Вы неверно заполнили серию или номер паспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        
        private void RegNameBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // AL.InputOneUpAndLowKeysProtector(RegNameBox, e);
        }

        private void _maskedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _maskedTextBox.Mask = "+0(000)000-0000";
        }
        private void ChooseTrainGRID_Loaded(object sender, RoutedEventArgs e)
        {
            ThrowTrainListToTable();
        }
        private void ChooseTrainGRID_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var items = (System.Data.DataRowView)ChooseTrainGRID.CurrentItem;
                CurrentTrainNumber = items.Row[0].ToString();
            }
            catch(Exception)
            {
                MessageBox.Show("При выборе поезда произошла ошибка. Попробуйте выбрать другой поезд.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
