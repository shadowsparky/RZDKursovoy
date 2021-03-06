﻿using MySql.Data.MySqlClient;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace RZDKursovoy
{
    public partial class Menu : UserControl
    {
        public MySqlConnection Connected { get; private set; }
        public MySqlConnection SetConnected { set { Connected = value; } }
        private ApplicationLogic AL = new ApplicationLogic();
        private string Login = "";
        public string SetLogin { set { Login = value; } }
        private int AvailableTicketsCount = new int();
        private int CurrentTicketID = new int();
        private string _CurrentTrainNumber = "";
        private int _Current_Railcar_Number = new int();
        private int _Current_Place_Number = new int();
        private string _Current_Arrival_Time = "";
        private string _Current_Arrival_Date = "";
        private string _Current_Arrival_Stop_Name = "";
        private string _Current_Departure_Time = "";
        private string _Current_Departure_Date = "";
        private string _Current_Departure_Stop_Name = "";

        public Menu()
        {
            InitializeComponent();
        }
        public void CheckActivateCabinet()
        {
            string[] args = { Login + "@localhost" };
            AvailableTicketsCount = AL.CatchIntResult(Connected, "call throwCountAvailableTickets", args);
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
        public void BoxesFiling()
        {
            Arrival_BOX.Items.Clear();
            Departure_BOX.Items.Clear();
            string[] param = { "null" };
            var a = AL.CatchStringListResult(Connected, "call ThrowAllStations", param); // AL.TrowAllStations(Connected);
            for (int i = 0; i < a.Count; i++)
            {
                Arrival_BOX.Items.Add(a[i]);
                Departure_BOX.Items.Add(a[i]);
            }
            CheckActivateCabinet();
        }
        private void FindTrain_BUTTON_Click(object sender, RoutedEventArgs e)
        {
            if ((Arrival_BOX.Text != "") && (Departure_BOX.Text != "") && (Departure_BOX.Text != ""))
            {
                try
                {
                    if (Arrival_BOX.Text != Departure_BOX.Text)
                    { 
                        ApplicationLogic AL = new ApplicationLogic();
                        string[] Data = { Arrival_BOX.Text, Departure_BOX.Text, Arrival_Date.Text };
                        List<string> Routs = AL.CatchStringListResult(Connected, "call FindRout", Data);
                        if (Routs.Count > 0)
                        {
                            List<string> TrainsList = new List<string>();
                            for (int i = 0; i < Routs.Count; i++)
                            {
                                string[] args = { Routs[i], Arrival_BOX.Text, Arrival_Date.Text, Departure_BOX.Text };
                                var TMPInputer = AL.CatchStringListResult(Connected, "call newFindTrainList", args);
                                for (int j = 0; j < TMPInputer.Count; j++)
                                    if (TMPInputer[j] != "-1")
                                        TrainsList.Add(TMPInputer[j]);
                            }
                            if (TrainsList.Count > 0)
                            {
                                ReservationControl RCN = new ReservationControl();
                                RCN = new ReservationControl();
                                RCN.SetConnection = Connected;
                                RCN.SetArrival = Arrival_BOX.Text;
                                RCN.SetDeparture = Departure_BOX.Text;
                                RCN.SetDate = Arrival_Date.Text;
                                RCN.SetRouts = Routs;
                                RCN.SetTrainsList = TrainsList;
                                RCN.SetMenu = this;
                                PerfectReflectionGRID.Children.Add(RCN);
                                Reflector.IsEnabled = false;
                            }
                            else
                            {
                                throw new System.Exception();
                            }
                        }
                        else
                        {
                            throw new System.Exception();
                        }
                    }
                    else
                    {
                        throw new System.Exception();
                    }
                }
                catch (System.Exception)
                {
                    AL.MessageShow("К сожалению, поездов по нужному Вам маршруту в данное время нет. Попробуйте выбрать другой день", "=(");
                }
            }
            else
            {
                AL.MessageErrorShow("Вы не заполнили данные", "Ошибка");
            }
        }
        private void ControlMenu_Loaded(object sender, RoutedEventArgs e)
        {

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
        private string throwTicketCode(string[] args)
        {
            return "<table style='height: auto; width: auto;' border='1'>" +
                "<tbody>" +
                    "<tr style = 'height: auto;'> " +
                        "<td style = 'width: auto; height: auto; text-align: center;'> (c)2018.AVB Inc.</td>" +
                            "<td style = 'width: auto; height: auto;'>" +
                            "<p style = 'text-align: center;' >Номер электронного билета:</p>" +
                            "<p style = 'text-align: center;'>&nbsp;<strong>" + args[0] + "</strong></p>" +
                        "</td>" +
                    "</tr>" +
                    "<tr style = 'height: auto;'>" +
                        "<td style = 'width: auto; height: auto; text-align: right;'> Маршрут следования: " + args[1] + "-&gt; &nbsp;</td>" +
                        "<td style = 'width: auto; height: auto;'>" + args[2] + "</td>" +
                    "</tr>" +
                    "<tr style = 'height: auto;'>" +
                        "<td style = 'width: auto; height: auto; text-align: center;' > Отправление: " + args[3] + " " + args[4] + "</td>" +
                        "<td style = 'width: auto; height: auto; text-align: center;' > Прибытие: " + args[5] + " " + args[6] + "</td>" +
                    "</tr>" +
                    "<tr style = 'height: auto;'>" +
                        "<td style = 'width: auto; height: auto; text-align: center;'>" +
                                "<p> Пассажир - " + args[7] + " " + args[8] + " " + args[9] + " " + "&nbsp;</p>" +
                                "<p> Паспортные данные - " + args[10] + " " + args[11] + "</p>" +
                        "</td>" +
                        "<td style = 'width: auto; height: auto; text-align: center;'>" +
                            "<p> Номер поезда: " + args[12] + "&nbsp;</p>" +
                            "<p> Номер вагона: " + args[13] + "&nbsp;</p>" +
                            "<p> Тип вагона: " + args[14] + "</p>" +
                            "<p> Номер места: " + args[15] +
                                    "&nbsp;</p>" +
                            "<p> Цена за билет: " + args[16] + "</p>" +
                        "</td>" +
                    "</tr>" +
                "</tbody>" +
            "</table>";
        }
        private void PrintTicketBUTTON_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTicketID != 0)
            {
                var RailcarInfo = AL.throwRailcarInfo(Connected, _CurrentTrainNumber, _Current_Railcar_Number);
                var PassPrivateInfo = AL.throwPassengerInfo(Connected, CurrentTicketID);
                var t = new HtmlAgilityPack.HtmlDocument();
                string[] data = { CurrentTicketID.ToString(), _Current_Arrival_Stop_Name, _Current_Departure_Stop_Name, _Current_Arrival_Date, _Current_Arrival_Time, _Current_Departure_Date, _Current_Departure_Time,
                PassPrivateInfo[0], PassPrivateInfo[1], PassPrivateInfo[2], PassPrivateInfo[3], PassPrivateInfo[4], _CurrentTrainNumber.ToString(), _Current_Railcar_Number.ToString(), RailcarInfo[0], _Current_Place_Number.ToString(), RailcarInfo[1] };
                Microsoft.Win32.SaveFileDialog SFD = new Microsoft.Win32.SaveFileDialog();
                SFD.Filter = "pdf файл (*.pdf)|*.pdf";
                if (SFD.ShowDialog() == true)
                {
                    try
                    {
                        PdfDocument pdf = GeneratePDF(data);
                        pdf.Save(SFD.FileName);
                        Process.Start(SFD.FileName);
                    }
                    catch (System.Exception)
                    { AL.MessageErrorShow("При сохранении билета произошла ошибка", "Ошибка"); }
                }
            }
            else
                AL.MessageShow("Вы должны выбрать билет, необходимый для сохранения", "Предупреждение");
        }

        private PdfDocument GeneratePDF(string[] data)
        {
            PdfDocument pdf = new PdfDocument();
            PdfGenerateConfig PGC = new PdfGenerateConfig();
            PGC.PageSize = PdfSharp.PageSize.A4;
            PGC.PageOrientation = PdfSharp.PageOrientation.Landscape;
            pdf = PdfGenerator.GeneratePdf(throwTicketCode(data), PGC);
            return pdf;
        }

        private void CancelTripBUTTON_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTicketID != 0)
            {
                try
                {
                    string[] args = { CurrentTicketID.ToString() };
                    var res = AL.CatchStringListResult(Connected, "call CancelTicket", args);
                    AL.MessageShow(res[0], "Отменено");
                    CheckActivateCabinet();
                }
                catch (System.Exception)
                {
                    AL.MessageErrorShow("Во время отмены произошла ошибка", "Ошибка");
                    return;
                }
            }
            else
            {
                AL.MessageShow("Для начала вы должны выбрать билет, который хотите отменить", "Ошибка");
                return;
            }
        }
        private void ShowBuyedTickets_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var t = (DataRowView)ShowBuyedTickets.CurrentItem;
                CurrentTicketID = System.Convert.ToInt32(t[0].ToString());
                _CurrentTrainNumber = t[1].ToString();
                _Current_Railcar_Number = System.Convert.ToInt32(t[2].ToString());
                _Current_Place_Number = System.Convert.ToInt32(t[3].ToString());
                _Current_Arrival_Time = t[4].ToString();
                _Current_Arrival_Date = t[5].ToString();
                _Current_Arrival_Stop_Name = t[6].ToString();
                _Current_Departure_Time = t[7].ToString();
                _Current_Departure_Date = t[8].ToString();
                _Current_Departure_Stop_Name = t[9].ToString();
            }
            catch (System.Exception)
            { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
        private void Departure_BOX_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(Departure_BOX, e);
        }
        private void Arrival_BOX_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AL.DontCtrlVAndSpace(Arrival_BOX, e);
        }
    }
}
