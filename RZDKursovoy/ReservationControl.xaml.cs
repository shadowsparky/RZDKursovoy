﻿using MySql.Data.MySqlClient;
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
        private Menu _menu;
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
        public Menu SetMenu { set { _menu = value; } }
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
                for (int j = 0; j < TrainsList.Count; j++)
                {
                    if (TrainsList[j] != "-1")
                    {
                        string[] Args = { ArrivalStation, Routs[i], TrainsList[j] };
                        Arrival_ID = AL.CatchIntResult(_connection, "select GetArrivalID", Args);
                        string[] args = { DepartureStation, Routs[i], TrainsList[j] };
                        Departure_ID = AL.CatchIntResult(_connection, "select GetDepartureID", args);
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
                AL.MessageErrorShow("При загрузке поездов произошла ошибка. Сообщите об этом администратору", "Ошибка");
            }
        }
        private void ChooseTrainNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTrainNumber != "")
            {
                string[] Args = { ArrivalStation, CurrentRoutID.ToString(), CurrentTrainNumber };
                Arrival_ID = AL.CatchIntResult(_connection, "select GetArrivalID", Args);
                string[] args = { DepartureStation, CurrentRoutID.ToString(), CurrentTrainNumber };
                Departure_ID = AL.CatchIntResult(_connection, "select GetDepartureID", args);
                ChooseTrainBox.Visibility = Visibility.Hidden;
                ChooseTrainTypeBox.Visibility = Visibility.Visible;
                AddToRailcarTypesBox();
            }
            else
            {
                AL.MessageShow("Вы должны выбрать поезд", "Предупреждение");
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
                string[] args = { CurrentTrainNumber, ChoosedRailcarType };
                var Data = AL.CatchStringListResult(_connection, "call ThrowTrainNumbersList", args);//ThrowTrainNumbersList(_connection, CurrentTrainNumber, ChoosedRailcarType);
                for (int i = 0; i < Data.Count; i++)
                {
                    ChooseRailcarNumberBox.Items.Add(Data[i]);
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
                AL.MessageShow("Вы должны выбрать тип вагона", "Предупреждение");
            }
        }
        private void ChooseRailcarNumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SeatChooseNumberBox.Items.Clear();
                Railcar_Number = Convert.ToInt32(ChooseRailcarNumberBox.SelectedItem.ToString());
                StepThreePlusGrid.IsEnabled = true;
                string[] data = { CurrentTrainNumber, Railcar_Number.ToString(), Arrival_ID.ToString(), Departure_ID.ToString() };
                var k = AL.CatchStringListResult(_connection, "call Available_For_Planting_Seats", data); //AL.Available_For_Planting_Seats(_connection, CurrentTrainNumber, Railcar_Number, Arrival_ID, Departure_ID);
                for (int i = 0; i < k.Count; i++)
                    SeatChooseNumberBox.Items.Add(k[i]);
            }
            catch (Exception)
            {
                AL.MessageShow("Вы должны выбрать номер вагона", "Предупреждение");
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
                AL.MessageShow("Вы должны выбрать место", "Предупреждение");
            }

        }
        private void InputData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var QueryString = "call EmployPlaces";
                string[] args = { RegPassSeries.Text, RegPassNumber.Text, Properties.PersonalData.Default.KeySi };
                Passenger_Number = AL.CatchIntResult(_connection, "SELECT FindPassenger", args); //FindPassenger(_connection, Convert.ToInt32(RegPassSeries.Text), Convert.ToInt32(RegPassNumber.Text));
                if ((RegNameBox.Text != "") && (RegFamBox.Text != "") && (RegPassSeries.Text != "") && (RegPassNumber.Text != ""))
                {
                    if (Passenger_Number == -1)
                    {
                        if ((RegPassSeries.Text.Length == 4) && (RegPassNumber.Text.Length == 6))
                        {
                            string[] Args = { RegFamBox.Text, RegNameBox.Text, RegPathrBox.Text, RegPassSeries.Text, RegPassNumber.Text, _maskedTextBox.Text, Properties.PersonalData.Default.KeySi };
                            var PasNewNum = AL.CatchIntResult(_connection, "select PassengerAddToDB", Args);
                            string[] data = { CurrentTrainNumber, Railcar_Number.ToString(), ChoosedSeatNumber.ToString(), PasNewNum.ToString(), Arrival_ID.ToString(), Departure_ID.ToString() };
                            var res = AL.MagicUniversalControlData(QueryString, data, "Reservation", _connection);
                            poselki.BestErrors BE = new poselki.BestErrors();
                            BE.CatchError(res);
                        }
                        else
                        {
                            AL.MessageErrorShow("Вы неправильно заполнили паспортные данные", "Ошибка");
                            return;
                        }
                    }
                    else
                    {
                        var ExistsData = AL.FindPassengerWithPersonalData(_connection, RegPassSeries.Text, RegPassNumber.Text);
                        if ((RegFamBox.Text == ExistsData[0]) && (RegNameBox.Text == ExistsData[1]) && (RegPathrBox.Text == ExistsData[2]))
                        {
                            if ((RegPassSeries.Text.Length == 4) && (RegPassNumber.Text.Length == 6))
                            {
                                string[] data = { CurrentTrainNumber, Railcar_Number.ToString(), ChoosedSeatNumber.ToString(), Passenger_Number.ToString(), Arrival_ID.ToString(), Departure_ID.ToString() };
                                var res = AL.MagicUniversalControlData(QueryString, data, "Reservation", _connection);
                                poselki.BestErrors BE = new poselki.BestErrors();
                                BE.CatchError(res);
                            }
                            else
                            {
                                AL.MessageErrorShow("Вы неправильно заполнили паспортные данные", "Ошибка");
                                return;
                            }
                        }
                        else
                        {
                            AL.MessageErrorShow("Информация о существующем в базе пассажире заполнена неверно. Полиция уже рядом", "Ошибка");
                            return;
                        }
                    }
                }
                else
                {
                    AL.MessageErrorShow("Вы не заполнили одно или несколько полей, необходимых для регистрации", "Ошибка");
                    return;
                }
            }
            catch (Exception)
            {
                AL.MessageErrorShow("Вы неверно заполнили серию или номер паспорта", "Ошибка");
                return;
            }
            _menu.CheckActivateCabinet();
            _menu.PerfectReflectionGRID.Children.Remove(this);
            _menu.Reflector.IsEnabled = true;
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
                string[] args = { CurrentTrainNumber };
                CurrentRoutID = AL.CatchIntResult(_connection, "select ThrowRoutID", args);//ThrowRoutID(_connection, CurrentTrainNumber);
            }
            catch (Exception)
            {
                AL.MessageErrorShow("При выборе поезда произошла ошибка. Попробуйте выбрать другой поезд.", "Ошибка");
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

        private void RegFamBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(RegFamBox, e);
        }

        private void RegNameBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(RegNameBox, e);
        }

        private void RegPathrBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(RegPathrBox, e);
        }

        private void RegPassSeries_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(RegPassSeries, e);
        }

        private void RegPassNumber_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(RegPassNumber, e);
        }
    }
}
