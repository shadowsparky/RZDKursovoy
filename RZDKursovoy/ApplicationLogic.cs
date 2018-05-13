using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RZDKursovoy
{
    class ApplicationLogic
    {
        /*Переменные, предназначенные для защиты ввода*/
        private Regex OnlyLowCaseWordsChecker = new Regex("[а-я]");
        private Regex OnlyUpCaseWordsChecker = new Regex("[А-Я]");
        private Regex EN_OnlyLowCaseWordsChecker = new Regex("[a-z]");
        private Regex EN_OnlyUpCaseWordsChecker = new Regex("[A-Z]");
        private Regex OnlyNumbersChecker = new Regex("[1-9 0]");
        /*Универсальные процедуры*/
        public string [] MagicUniversalControlData(string QueryString, string[] DataArgs, string userControl, MySqlConnection Connection)
        {
            if (userControl != "Delete")
            {
                QueryString += "(";
                string[] ParameterArg = new string[DataArgs.Length];
                for (int i = 0; i < DataArgs.Length; i++)
                {
                    if (i != DataArgs.Length - 1)
                        QueryString += "@ARG" + i + ", ";
                    else
                        QueryString += "@ARG" + i;
                    ParameterArg[i] = "@ARG" + i;
                }
                QueryString += ")";
                var BestCommand = new MySqlCommand(QueryString, Connection);
                for (int i = 0; i < DataArgs.Length; i++)
                {
                    BestCommand.Parameters.AddWithValue(ParameterArg[i], DataArgs[i]);
                }
                try
                {
                    BestCommand.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    string[] Result = { ex.Message, ex.Number.ToString() };
                    return Result;
                }
                switch (userControl)
                {
                    case "RegAdd":
                        MessageShow("Ваш аккаунт успешно зарегистрирован", "ОК");
                        break;
                    case "Reservation":
                        MessageShow("Спасибо за покупку! Билет доступен в личном кабинете", "ОК");
                        break;
                    case "DeleteTicket":
                        MessageShow("Резервирование отменено, деньги скоро вернутся на ваш счет, а отмененный билет больше недействителен", "ОК");
                        break;
                    case "UpdateTrain":
                        MessageShow("Информация о поезде была отредактирована", "ОК");
                        break;
                    case "UpdateRailcar":
                        MessageShow("Информация о вагоне была отредактирована", "ОК");
                        break;
                    case "UpdateStop":
                        MessageShow("Информация об остановке была отредактирована", "ОК");
                        break;
                    case "UpdateRout":
                        MessageShow("Информация о маршруте была отредактирована", "ОК");
                        break;
                    case "DeleteTrain":
                        MessageShow("Информация о поезде была успешно удалена", "ОК");
                        break;
                    case "DeleteRailcar":
                        MessageShow("Информация о вагоне была успешно удалена", "ОК");
                        break;
                    case "DeleteStop":
                        MessageShow("Информация об остановке была успешно удалена", "ОК");
                        break;
                    case "DeleteRout":
                        MessageShow("Информация о маршруте была успешно удалена", "ОК");
                        break;
                    case "DeleteArrival":
                        MessageShow("Информация о прибытии поезда была успешно удалена", "ОК");
                        break;
                    case "UpdateArrival":
                        MessageShow("Информация о прибытии поезда была успешно изменена", "ОК");
                        break;
                    case "DeleteDeparture":
                        MessageShow("Информация об отправлении поезда была успешно удалена", "ОК");
                        break;
                    case "UpdateDeparture":
                        MessageShow("Информация об отправлении поезда была успешно изменена", "ОК");
                        break;
                }
            }
            else if (userControl == "Delete")
            {
                var DelCommand = new MySqlCommand(QueryString, Connection);
                DelCommand.Parameters.AddWithValue("Num", DataArgs[0]);
                try
                {
                    DelCommand.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    string[] Result = { ex.Message, ex.Number.ToString() };
                    return Result;
                }
            }
            string[] Result3 = { "OK", "1" };
            return Result3;
        }
        public void MagicUserControl(MySqlConnection connected, DataRowView t, string Proc, string Control)
        {
            string[] args = new string[t.Row.ItemArray.Length];
            for (int i = 0; i < t.Row.ItemArray.Length; i++)
                args[i] = t.Row.ItemArray[i].ToString();
            var res = MagicUniversalControlData(Proc, args, Control, connected);
            poselki.BestErrors BE = new poselki.BestErrors();
            BE.CatchError(res);
        }
        public List<string> CatchStringListResult(MySqlConnection con, string Query, string[] Args)
        {
            List<string> Result = new List<string>();
            MySqlDataReader ResultReader = null;
            Query += "(";
            string[] ParameterArg = new string[Args.Length];
            if (Args[0] != "null")
            {
                for (int i = 0; i < Args.Length; i++)
                {
                    if (i != Args.Length - 1)
                        Query += "@ARG" + i + ", ";
                    else
                        Query += "@ARG" + i;
                    ParameterArg[i] = "@ARG" + i;
                }
            }
            Query += ")";
            var BestCommand = new MySqlCommand(Query, con);
            if (Args[0] != "null")
            {
                for (int i = 0; i < Args.Length; i++)
                {
                    BestCommand.Parameters.AddWithValue(ParameterArg[i], Args[i]);
                }
            }
            try
            {
                ResultReader = BestCommand.ExecuteReader();
                while (ResultReader.Read())
                {
                    Result.Add(ResultReader.GetString(0));
                }
            }
            catch(Exception)
            {
                Result.Clear();
                Result.Add("-1");
                return Result;
            }
            finally
            {
                ResultReader.Close();
            }
            return Result;
        }
        public int CatchIntResult(MySqlConnection con, string Query, string [] Args)
        {
            int Result = new int();
            MySqlDataReader ResultReader = null;
            Query += "(";
            string[] ParameterArg = new string[Args.Length];
            if (Args[0] != "null")
            {
                for (int i = 0; i < Args.Length; i++)
                {
                    if (i != Args.Length - 1)
                        Query += "@ARG" + i + ", ";
                    else
                        Query += "@ARG" + i;
                    ParameterArg[i] = "@ARG" + i;
                }
            }
            Query += ")";
            var BestCommand = new MySqlCommand(Query, con);
            if (Args[0] != "null")
            {
                for (int i = 0; i < Args.Length; i++)
                {
                    BestCommand.Parameters.AddWithValue(ParameterArg[i], Args[i]);
                }
            }
            try
            {
                ResultReader = BestCommand.ExecuteReader();
                ResultReader.Read();
                Result = ResultReader.GetInt32(0);
            }
            catch (Exception)
            {
                Result = -1;
                return Result;
            }
            finally
            {
                ResultReader.Close();
            }
            return Result;
        }
        public bool ConvertCheck(object sender, DataGridCellEditEndingEventArgs e, int[] itemarr)
        {
            if (itemarr[0] != -1)
            {
                for (int i = 0; i < itemarr.Length; i++)
                {
                    if (e.Column.DisplayIndex == itemarr[i])
                    {
                        var t = e.EditingElement.ToString().Split(':');
                        try
                        {
                            Convert.ToInt32(t[1]);
                        }
                        catch (Exception)
                        {
                            MessageErrorShow("Вы ввели - " + t[1] + ". Это значение не является целым числом", "Ошибка");
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public DataRowView BlockUpdate(object sender, DataGridCellEditEndingEventArgs e, int[] itemarr)
        {
            DataRowView TMPGridRow = null;
            if (itemarr[0] != -1)
            {
                for (int i = 0; i < itemarr.Length; i++)
                {
                    if (e.Column.DisplayIndex != i)
                    {
                        try
                        {
                            TMPGridRow = (DataRowView)(sender as DataGrid).CurrentItem;
                        }
                        catch (Exception)
                        { }
                    }
                    else
                    {
                        TMPGridRow = null;
                        return TMPGridRow;
                    }
                }
            }
            else
            {
                try
                {
                    TMPGridRow = (DataRowView)(sender as DataGrid).CurrentItem;
                }
                catch (Exception)
                { }
            }
            return TMPGridRow;
        }
        /*Уникальные процедуры*/
        public List<string> Available_Railcar_Types(MySqlConnection connection, string TrainNum)
        {
            List<string> SavedTypes = new List<string>();
            string QueryString = "call Available_Railcar_Types(@Train_Num)";
            var BestCommand = new MySqlCommand(QueryString, connection);
            BestCommand.Parameters.AddWithValue("Train_Num", TrainNum);
            var RailcarsTypesReader = BestCommand.ExecuteReader();
            while (RailcarsTypesReader.Read())
            {
                SavedTypes.Add(RailcarsTypesReader.GetString(0) + " цена за билет - " + RailcarsTypesReader.GetString(1) + " руб.");
            }
            RailcarsTypesReader.Close();
            return SavedTypes;
        }
        public List<string> TrainInfo(MySqlConnection connected, string TrainNumber, int Arrival_ID_IN, int Departure_ID_IN)
        {
            List<string> TrainInfoList = new List<string>();
            var QueryString = "call TrainInfoTwo(@TrainNumber, @Arrival_ID_IN, @Departure_ID_IN)";
            var BestCommand = new MySqlCommand(QueryString, connected);
            BestCommand.Parameters.AddWithValue("TrainNumber", TrainNumber);
            BestCommand.Parameters.AddWithValue("Arrival_ID_IN", Arrival_ID_IN);
            BestCommand.Parameters.AddWithValue("Departure_ID_IN", Departure_ID_IN);
            var r = BestCommand.ExecuteReader();
            r.Read();
            TrainInfoList.Add(r.GetString(0).ToString());
            TrainInfoList.Add(r.GetString(1).ToString());
            TrainInfoList.Add(r.GetString(2).ToString());
            TrainInfoList.Add(r.GetString(3).ToString());
            TrainInfoList.Add(r.GetString(4).ToString());
            TrainInfoList.Add(r.GetString(5).ToString());
            r.Close();
            return TrainInfoList;
        }
        public List<string> FindPassengerWithPersonalData(MySqlConnection connection, int Passport_Series_IN, int Passport_Number_IN)
        {
            List<string> Result = new List<string>();
            var QueryString = "call FindPassengerWithPersonalData(@Passport_Series_IN, @Passport_Number_IN, @KeySi)";
            var BestCommand = new MySqlCommand(QueryString, connection);
            BestCommand.Parameters.AddWithValue("Passport_Series_IN", Passport_Series_IN);
            BestCommand.Parameters.AddWithValue("Passport_Number_IN", Passport_Number_IN);
            BestCommand.Parameters.AddWithValue("KeySi", Properties.PersonalData.Default.KeySi);
            var r = BestCommand.ExecuteReader();
            r.Read();
            for (int i = 0; i < 4; i++)
            { Result.Add(r.GetString(i)); }
            r.Close();
            return Result;
        }
        public List<string> throwPassengerInfo(MySqlConnection connected, int Reservation_ID_IN)
        {
            List<string> result = new List<string>();
            var QueryString = "call throwPassengerInfo(@Reservation_ID_IN, @KeySi)";
            var BestCommand = new MySqlCommand(QueryString, connected);
            BestCommand.Parameters.AddWithValue("Reservation_ID_IN", Reservation_ID_IN);
            BestCommand.Parameters.AddWithValue("KeySi", Properties.PersonalData.Default.KeySi);
            var PassengerInfoRead = BestCommand.ExecuteReader();
            while (PassengerInfoRead.Read())
            {
                for (int i = 0; i < 5; i++)
                    result.Add(PassengerInfoRead.GetString(i));
            }
            PassengerInfoRead.Close();
            return result;
        }
        public List<string> throwRailcarInfo(MySqlConnection connected, string Train_Number_IN, int Railcar_Number_IN)
        {
            List<string> result = new List<string>();
            var QueryString = "call throwRailcarInfo('" + Train_Number_IN + "', '"+Railcar_Number_IN+"')";
            var BestCommand = new MySqlCommand(QueryString, connected);
            var RailcarInfoRead = BestCommand.ExecuteReader();
            while (RailcarInfoRead.Read())
            {
                for (int i = 0; i < 2; i++)
                    result.Add(RailcarInfoRead.GetString(i));
            }
            RailcarInfoRead.Close();
            return result;
        }
        public string FindTrain(MySqlConnection connected, string RoutID, int Arrival_Stop_IN, string Arrival_Date_IN)
        {
            string QueryString = "SELECT FindTrain(@RoutID, @Arrival_Stop_IN, @Arrival_Date_IN)";
            var BestCommand = new MySqlCommand(QueryString, connected);
            BestCommand.Parameters.AddWithValue("RoutID", RoutID);
            BestCommand.Parameters.AddWithValue("Arrival_Stop_IN", Arrival_Stop_IN);
            BestCommand.Parameters.AddWithValue("Arrival_Date_IN", Arrival_Date_IN);
            MySqlDataReader r = BestCommand.ExecuteReader();
            string TrainID = "";
            r.Read();
            TrainID = r.GetString(0).ToString();
            r.Close();
            return TrainID;
        }
        /*Процедуры для проверки ввода*/
        public void InputProtector(System.Windows.Controls.TextChangedEventArgs e, System.Windows.Controls.TextBox TB)
        {
            TB.Text = TB.Text.Replace(" ", string.Empty);
            TB.Text = TB.Text.Replace("'", string.Empty);
            TB.Text = TB.Text.Replace('"', ' ');
            TB.Text = TB.Text.Replace("*", string.Empty);
            TB.Text = TB.Text.Replace("/", string.Empty);
            TB.Text = TB.Text.Replace(";", string.Empty);
            TB.Text = TB.Text.Replace("@", string.Empty);
            TB.Text = TB.Text.Replace("!", string.Empty);
            TB.Text = TB.Text.Replace("#", string.Empty);
            TB.Text = TB.Text.Replace("$", string.Empty);
            TB.Text = TB.Text.Replace("№", string.Empty);
            TB.Text = TB.Text.Replace("%", string.Empty);
            TB.Text = TB.Text.Replace("^", string.Empty);
            TB.Text = TB.Text.Replace(":", string.Empty);
            TB.Text = TB.Text.Replace("?", string.Empty);
            TB.Text = TB.Text.Replace("*", string.Empty);
            TB.Text = TB.Text.Replace("(", string.Empty);
            TB.Text = TB.Text.Replace(")", string.Empty);
            TB.Text = TB.Text.Replace(",", string.Empty);
            TB.Text = TB.Text.Replace(".", string.Empty);
            TB.Text = TB.Text.Replace("<", string.Empty);
            TB.Text = TB.Text.Replace(">", string.Empty);
            TB.Text = TB.Text.Replace("[", string.Empty);
            TB.Text = TB.Text.Replace("]", string.Empty);
            TB.Text = TB.Text.Replace("{", string.Empty);
            TB.Text = TB.Text.Replace("}", string.Empty);
            TB.Text = TB.Text.Replace("-", string.Empty);
            TB.Text = TB.Text.Replace("|", string.Empty);
            TB.Text = TB.Text.Replace("\\", string.Empty);
            TB.SelectionStart = TB.Text.Length;
        }
        public void InputPersonalDataProtector(System.Windows.Controls.TextBox TB, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (TB.Text.Length != 0)
            {
                Match matchLowCase = OnlyLowCaseWordsChecker.Match(e.Text);
                if (!matchLowCase.Success)
                    e.Handled = true;
            }
            else
            {
                Match matchUpCase = OnlyUpCaseWordsChecker.Match(e.Text);
                if (!matchUpCase.Success)
                    e.Handled = true;
            }
        }
        public void InputNumbersDataProtector(System.Windows.Controls.TextBox TB, System.Windows.Input.TextCompositionEventArgs e)
        {
            Match match = OnlyNumbersChecker.Match(e.Text);
            if (!match.Success)
                e.Handled = true;
        }
        public void InputWordsProtector(System.Windows.Input.TextCompositionEventArgs e)
        {
            Match matchLowCase = OnlyLowCaseWordsChecker.Match(e.Text);
            Match matchUpCase = OnlyUpCaseWordsChecker.Match(e.Text);
            if ((!matchLowCase.Success) && (!matchUpCase.Success))
            {
                e.Handled = true;
            }
        }
        public void EN_InputWordsProtector(System.Windows.Input.TextCompositionEventArgs e)
        {
            Match matchLowCase = EN_OnlyLowCaseWordsChecker.Match(e.Text);
            Match matchUpCase = EN_OnlyUpCaseWordsChecker.Match(e.Text);
            if ((!matchLowCase.Success) && (!matchUpCase.Success))
            {
                e.Handled = true;
            }
        }
        public void EN_InputLoginWordsProtector(System.Windows.Controls.TextBox TB, System.Windows.Input.TextCompositionEventArgs e)
        {
            Match matchLowCase = EN_OnlyLowCaseWordsChecker.Match(e.Text);
            Match matchUpCase = EN_OnlyUpCaseWordsChecker.Match(e.Text);
            Match matchNumbersCase = OnlyNumbersChecker.Match(e.Text);
            if ((!matchLowCase.Success) && (!matchUpCase.Success) && (!matchNumbersCase.Success))
            {
                e.Handled = true;
            }
        }
        /*Вывод сообщения*/
        public void MessageErrorShow(string Message, string Title)
        {
            var msg = new BespokeFusion.CustomMaterialMessageBox
            {
                TxtMessage = { Text = Message, Foreground = Brushes.Black },
                TxtTitle = { Text = Title, Foreground = Brushes.White },
                BtnOk = { Content = "Ок", Background = Brushes.DarkRed, BorderBrush = Brushes.DarkRed},
                BtnCancel = {Visibility = Visibility.Collapsed},
                BtnCopyMessage = { Visibility = Visibility.Collapsed },
                MainContentControl = { Background = Brushes.White},
                TitleBackgroundPanel = { Background = Brushes.DarkRed },
                BorderBrush = Brushes.DarkRed
            };
            msg.Show();
        }
        public void MessageShow(string Message, string Title)
        {
            var msg = new BespokeFusion.CustomMaterialMessageBox
            {
                TxtMessage = { Text = Message, Foreground = Brushes.Black },
                TxtTitle = { Text = Title, Foreground = Brushes.White },
                BtnOk = { Content = "Ок", Background = Brushes.LightSeaGreen, BorderBrush = Brushes.LightSeaGreen },
                BtnCancel = { Visibility = Visibility.Collapsed },
                BtnCopyMessage = { Visibility = Visibility.Collapsed},
                MainContentControl = { Background = Brushes.White},
                TitleBackgroundPanel = { Background = Brushes.LightSeaGreen },
                BorderBrush = Brushes.LightSeaGreen
            };
            msg.Show();
        }
        /*Dispatcher Procedures*/
        public bool KeyUpInside(MySqlConnection Connected, object sender, System.Windows.Input.KeyEventArgs e, DataGrid grid, DataRowView TMPRow, bool ConvertCheck, string DeleteCommand, 
            string UpdateCommand, string DeleteControl, string UpdateControl, string Error, string [] args)
        {
            try
            {
                var r = e.Key.ToString();
                if (r == "Delete")
                {
                    var res = MagicUniversalControlData(DeleteCommand, args, DeleteControl, Connected);
                    poselki.BestErrors BE = new poselki.BestErrors();
                    BE.CatchError(res);
                    return true;
                }
                else if (r == "Return")
                {
                    var t1 = TMPRow;
                    if (t1 != null)
                    {
                        if (ConvertCheck)
                        {
                            MagicUserControl(Connected, t1, UpdateCommand, UpdateControl);
                        }
                    }
                    else
                    {
                        MessageErrorShow(Error, "Ошибка");
                    }
                    return true;
                }
                else if (r == "Escape")
                {
                    return true;
                }
            }
            catch (Exception)
            { }
            return false;
        }
        public bool FillTable(MySqlConnection connection, DataGrid DG, string query, string [] args)
        {
            try
            {
                MySqlDataAdapter ad = new MySqlDataAdapter();
                ad.SelectCommand = new MySqlCommand(query, connection);
                DataTable table = new DataTable();
                ad.Fill(table);
                for (int i = 0; i < args.Length; i++)
                {
                    table.Columns[i].ColumnName = args[i];
                }
                DG.ItemsSource = table.DefaultView;
            }
            catch (Exception ex)
            {
                var e = ex.Message;
                MessageErrorShow("При загрузке данных произошла ошибка", "Ошибка");
                return false;
            }
            return true;
        }
    }
}
