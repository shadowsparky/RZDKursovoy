using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RZDKursovoy
{
    /// <summary>
    /// Логика взаимодействия для ReservationControl.xaml
    /// </summary>
    public partial class ReservationControl : UserControl
    {
        /*Переменные*/
        private MainWindow MW;
        private string ArrivalStation = "###";
        private string DepartureStation = "###";
        private string ArrivalDate = "###";
        private List<string> Routs = new List<string>();
        private List<string> TrainsList = new List<string>();
        private MySqlConnection _connection;
        private int Arrival_ID = new int();
        private int Departure_ID = new int();
        private int Railcar_Number = new int();
        private int CurrentRoutID = new int();
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
        public List<string> SetTrainsList { set { TrainsList = value; } }
        public MySqlConnection SetConnection { set { _connection = value; } }
        public MainWindow SetMainWindow { set { MW = value; } }
        /*Процедуры*/
        public ReservationControl()
        {
            InitializeComponent();
            ChooseTrainBox.Visibility = Visibility.Visible;
            ChooseTrainTypeBox.Visibility = Visibility.Hidden;
            InputPersonalDataBox.Visibility = Visibility.Hidden;
            StepTwoGrid.IsEnabled = false;
            StepThreePlusGrid.IsEnabled = false;
            StepThreeGrid.IsEnabled = false;
        }
        private void ThrowTrainListToTable()
        {
            var test = new List<TableFillKostil>();
            for (int i = 0; i < Routs.Count; i++)
            {
                //1ая строка
                //var TrainNum = AL.newFindTrainList(_connection, Routs[i], ArrivalStation, ArrivalDate);
                for (int j = 0; j < TrainsList.Count; j++)
                {
                    if (TrainsList[j] != "-1")
                    {
                        Arrival_ID = AL.GetArrivalID(_connection, ArrivalStation, Convert.ToInt32(Routs[i]), TrainsList[j]);
                        Departure_ID = AL.GetDepartureID(_connection, DepartureStation, Convert.ToInt32(Routs[i]), TrainsList[j]);
                        if ((Arrival_ID != -1) && (Departure_ID != -1))
                        {
                            var TrainData = AL.TrainInfo(_connection, TrainsList[j], Arrival_ID, Departure_ID);
                            test.Add(new TableFillKostil(TrainData[0], TrainData[1], TrainData[2], TrainData[3], TrainData[4], TrainData[5]));
                        }
                    }
                }
            }
            if (test.Count != 0)
            {
                ChooseTrainGRID.ItemsSource = test;
            }
            else
            {
                MessageBox.Show("При загрузке поездов произошла ошибка. Сообщите об этом администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ChooseTrainNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTrainNumber != "")
            {
                Arrival_ID = AL.GetArrivalID(_connection, ArrivalStation, CurrentRoutID, CurrentTrainNumber);
                Departure_ID = AL.GetDepartureID(_connection, DepartureStation, CurrentRoutID, CurrentTrainNumber);
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
            catch (Exception)
            {
                MessageBox.Show("Вы должны выбрать место", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Hand);
            }

        }
        private void InputData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var QueryString = "call EmployPlaces";
                Passenger_Number = AL.FindPassenger(_connection, Convert.ToInt32(RegPassSeries.Text), Convert.ToInt32(RegPassNumber.Text));
                if ((RegNameBox.Text != "") && (RegFamBox.Text != "") && (RegPassSeries.Text != "") && (RegPassNumber.Text != ""))
                {
                    if (Passenger_Number == -1)
                    {
                        if ((RegPassSeries.Text.Length == 6) && (RegPassNumber.Text.Length == 4))
                        {
                            var PasNewNum = AL.PassengerAddToDB(_connection, RegFamBox.Text, RegNameBox.Text, RegPathrBox.Text, Convert.ToInt32(RegPassSeries.Text), Convert.ToInt32(RegPassNumber.Text), _maskedTextBox.Text);
                            string[] data = { CurrentTrainNumber, Railcar_Number.ToString(), ChoosedSeatNumber.ToString(), PasNewNum.ToString(), Arrival_ID.ToString(), Departure_ID.ToString() };
                            AL.MagicUniversalControlData(QueryString, data, "Reservation", _connection);
                        }
                        else
                        {
                            MessageBox.Show("Вы неправильно заполнили паспортные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        var ExistsData = AL.FindPassengerWithPersonalData(_connection, Convert.ToInt32(RegPassSeries.Text), Convert.ToInt32(RegPassNumber.Text));
                        if ((RegFamBox.Text == ExistsData[0]) && (RegNameBox.Text == ExistsData[1]) && (RegPathrBox.Text == ExistsData[2]))
                        {
                            if ((RegPassSeries.Text.Length == 6) && (RegPassNumber.Text.Length == 4))
                            {
                                string[] data = { CurrentTrainNumber, Railcar_Number.ToString(), ChoosedSeatNumber.ToString(), Passenger_Number.ToString(), Arrival_ID.ToString(), Departure_ID.ToString() };
                                AL.MagicUniversalControlData(QueryString, data, "Reservation", _connection);
                            }
                            else
                            {
                                MessageBox.Show("Вы неправильно заполнили паспортные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Информация о существующем в базе пассажире заполнена неверно. Полиция уже рядом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Вы не заполнили одно или несколько полей, необходимых для регистрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Вы неверно заполнили серию или номер паспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MW.CheckActivateCabinet();
            MW.PerfectReflectionGRID.Children.Remove(this);
            InputData.Focusable = false;
        }
        private void _maskedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _maskedTextBox.Mask = "+7(000)000-0000";
        }
        private void ChooseTrainGRID_Loaded(object sender, RoutedEventArgs e)
        {
            ThrowTrainListToTable();
        }
        private void ChooseTrainGRID_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                TableFillKostil TFK = (TableFillKostil)ChooseTrainGRID.SelectedItem;
                CurrentTrainNumber = TFK.Par1;
                CurrentRoutID = AL.ThrowRoutID(_connection, CurrentTrainNumber);
            }
            catch (Exception)
            {
                MessageBox.Show("При выборе поезда произошла ошибка. Попробуйте выбрать другой поезд.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;
            if (pd != null)
            {
                // Check for DisplayName attribute and set the column header accordingly
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && displayName != DisplayNameAttribute.Default)
                {
                    return displayName.DisplayName;
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;
                if (pi != null)
                {
                    Object[] attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        var displayName = attributes[i] as DisplayNameAttribute;
                        if (displayName != null && displayName != DisplayNameAttribute.Default)
                        {
                            return displayName.DisplayName;
                        }
                    }
                }
            }
            return null;
        }
        private void ChooseTrainGRID_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var displayName = GetPropertyDisplayName(e.PropertyDescriptor);

            if (!string.IsNullOrEmpty(displayName))
            {
                e.Column.Header = displayName;
            }
        }
        private void RegFamBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.InputPersonalDataProtector(RegFamBox, e);
        }
        private void RegPassSeries_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.InputNumbersDataProtector(RegPassSeries, e);
        }
        private void RegNameBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.InputPersonalDataProtector(RegNameBox, e);
        }
        private void RegPathrBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.InputPersonalDataProtector(RegPathrBox, e);
        }
        private void RegPassNumber_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            AL.InputNumbersDataProtector(RegPassNumber, e);
        }
        private void _maskedTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            bool error = new bool();
            for (int i = 0; i < _maskedTextBox.Text.Length; i++)
                if (_maskedTextBox.Text[i] == '_')
                    error = true;
            if (error)
            {
                _maskedTextBox.Text = "";
                _maskedTextBox.Mask = "";
            }
        }
    }
}
