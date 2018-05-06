using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
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
        public void MagicUniversalControlData(string QueryString, string[] DataArgs, string userControl, MySqlConnection Connection)
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
                    var t = ex.Message.ToString();
                    return;
                }
                if (userControl == "RegAdd")
                {
                    MessageShow("Ваш аккаунт успешно зарегистрирован", "ОК");
                }
                else if (userControl == "Reservation")
                {
                    MessageShow("Спасибо за покупку! Билет доступен в личном кабинете", "ОК");
                }
                else if (userControl == "DeleteTicket")
                {
                    MessageShow("Резервирование отменено, деньги скоро вернутся на ваш счет, а отмененный билет больше недействителен", "ОК");
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
                    poselki.BestErrors BE = new poselki.BestErrors();
                    MessageShow(BE.getError(ex.Number.ToString()), "Ошибка");
                    return;
                }
            }
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
    }
}
