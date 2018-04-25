using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace RZDKursovoy
{
    class ApplicationLogic
    {
        private Regex OnlyLowCaseWordsChecker = new Regex("[а-я]");
        private Regex OnlyUpCaseWordsChecker = new Regex("[А-Я]");
        private Regex EN_OnlyLowCaseWordsChecker = new Regex("[a-z]");
        private Regex EN_OnlyUpCaseWordsChecker = new Regex("[A-Z]");
        private Regex OnlyNumbersChecker = new Regex("[1-9 0]");

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
                    MessageBox.Show(ex.Number.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (userControl == "RegAdd")
                {
                    MessageBox.Show("Ваш аккаунт успешно зарегистрирован", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (userControl == "Reservation")
                {
                    MessageBox.Show("Место успешно зарезервировано", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                //if (userControl == "Add")
                //    MessageBox.Show("Запись добавлена", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
                //else MessageBox.Show("Запись отредактирована", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show(ex.Number.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //MessageBox.Show("Запись удалена", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public List<string> TrowAllStations(MySqlConnection connection)
        {
            List<string> SavedTypes = new List<string>();
            string QueryString = "call ThrowAllStations";
            var BestCommand = new MySqlCommand(QueryString, connection);
            var RailcarsTypesReader = BestCommand.ExecuteReader();
            while (RailcarsTypesReader.Read())
            {
                SavedTypes.Add(RailcarsTypesReader.GetString(0));
            }
            RailcarsTypesReader.Close();
            return SavedTypes;
        }
        public List<string> Available_Railcar_Types(MySqlConnection connection, string TrainNum)
        {
            List<string> SavedTypes = new List<string>();
            string QueryString = "call Available_Railcar_Types(" + TrainNum + ")";
            var BestCommand = new MySqlCommand(QueryString, connection);
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
        public List<string> FindRout(MySqlConnection connected, string[] data)
        {
            string QueryString = "call FindRout(@ARG1, @ARG2, @ARG3)";
            var BestCommand = new MySqlCommand(QueryString, connected);
            BestCommand.Parameters.AddWithValue("ARG1", data[0]);
            BestCommand.Parameters.AddWithValue("ARG2", data[1]);
            BestCommand.Parameters.AddWithValue("ARG3", data[2]);
            MySqlDataReader r = BestCommand.ExecuteReader();
            List<string> RoutNumbers = new List<string>();
            int i = 0;
            while (r.Read())
            {
                RoutNumbers.Add(r.GetString(0).ToString());
                i++;
            }
            r.Close();
            return RoutNumbers;
        }
        public List<int> ThrowTrainNumbersList(MySqlConnection connection, string TrainNumber, string RailcarType)
        {
            List<int> SavedTypes = new List<int>();
            string QueryString = "call ThrowTrainNumbersList(@TrainNumber, @RailcarType)";
            var BestCommand = new MySqlCommand(QueryString, connection);
            BestCommand.Parameters.AddWithValue("TrainNumber", TrainNumber);
            BestCommand.Parameters.AddWithValue("RailcarType", RailcarType);
            var TrainNumbersRead = BestCommand.ExecuteReader();
            while (TrainNumbersRead.Read())
            {
                SavedTypes.Add(TrainNumbersRead.GetInt32(0));
            }
            TrainNumbersRead.Close();
            return SavedTypes;
        }
        public List<int> Available_For_Planting_Seats(MySqlConnection connection, string TrainNumber, int RailcarNumber, int CA, int CD)
        {
            List<int> SavedTypes = new List<int>();
            string QueryString = "call Available_For_Planting_Seats(@Train_Number, @Railcar_Number, @CurrentArrivalID, @CurrentDepartureID)";
            MySqlCommand BestCommand = new MySqlCommand(QueryString, connection);
            BestCommand.Parameters.AddWithValue("Train_Number", TrainNumber);
            BestCommand.Parameters.AddWithValue("Railcar_Number", RailcarNumber);
            BestCommand.Parameters.AddWithValue("CurrentArrivalID", CA);
            BestCommand.Parameters.AddWithValue("CurrentDepartureID", CD);
            var SeatsRead = BestCommand.ExecuteReader();
            while (SeatsRead.Read())
            {
                SavedTypes.Add(SeatsRead.GetInt32(0));
            }
            SeatsRead.Close();
            return SavedTypes;
        }
        public List<string> FindPassengerWithPersonalData(MySqlConnection connection, int Passport_Series_IN, int Passport_Number_IN)
        {
            List<string> Result = new List<string>();
            var QueryString = "call FindPassengerWithPersonalData(@Passport_Series_IN, @Passport_Number_IN)";
            var BestCommand = new MySqlCommand(QueryString, connection);
            BestCommand.Parameters.AddWithValue("Passport_Series_IN", Passport_Series_IN);
            BestCommand.Parameters.AddWithValue("Passport_Number_IN", Passport_Number_IN);
            var r = BestCommand.ExecuteReader();
            r.Read();
            for (int i = 0; i < 4; i++)
            { Result.Add(r.GetString(i)); }
            r.Close();
            return Result;
        }
        public List<string> FindTrainList(MySqlConnection connection, string RoutID, int Arrival_Stop_IN, string Arrival_Date_IN)
        {
            List<string> SavedTypes = new List<string>();
            string QueryString = "call FindTrainList(@RoutID, @Arrival_Stop_IN, @Arrival_Date_IN)";
            var BestCommand = new MySqlCommand(QueryString, connection);
            BestCommand.Parameters.AddWithValue("RoutID", RoutID);
            BestCommand.Parameters.AddWithValue("Arrival_Stop_IN", Arrival_Stop_IN);
            BestCommand.Parameters.AddWithValue("Arrival_Date_IN", Arrival_Date_IN);
            var TrainListRead = BestCommand.ExecuteReader();
            while (TrainListRead.Read())
            {
                SavedTypes.Add(TrainListRead.GetString(0));
            }
            TrainListRead.Close();
            return SavedTypes;
        }
        public int GetDepartureID(MySqlConnection connection, string DepartureStation, int RoutID, string TrainNum)
        {
            MySqlCommand GetDI = new MySqlCommand("select GetDepartureID(@DepartureStation, @RoutID, @TrainNumber)", connection);
            GetDI.Parameters.AddWithValue("DepartureStation", DepartureStation);
            GetDI.Parameters.AddWithValue("RoutID", RoutID);
            GetDI.Parameters.AddWithValue("TrainNumber", TrainNum);
            var DIRead = GetDI.ExecuteReader();
            DIRead.Read();
            var Departure_ID = DIRead.GetInt32(0);
            DIRead.Close();
            return Departure_ID;
        }
        public int GetArrivalID(MySqlConnection connection, string ArrivalStation, int RoutID, string TrainNum)
        {
            MySqlCommand GetAI = new MySqlCommand("select GetArrivalID(@ArrivalStation, @RoutID, @TrainNumber)", connection);
            GetAI.Parameters.AddWithValue("ArrivalStation", ArrivalStation);
            GetAI.Parameters.AddWithValue("RoutID", RoutID);
            GetAI.Parameters.AddWithValue("TrainNumber", TrainNum);
            var AIRead = GetAI.ExecuteReader();
            AIRead.Read();
            var Arrival_ID = AIRead.GetInt32(0);
            AIRead.Close();
            return Arrival_ID;
        }
        public int FindPassenger(MySqlConnection connected, int Passport_Series_IN, int Passport_Number_IN)
        {
            var Result = new int();
            var QueryString = "SELECT FindPassenger(@Passport_Series_IN, @Passport_Number_IN)";
            var BestCommand = new MySqlCommand(QueryString, connected);
            BestCommand.Parameters.AddWithValue("Passport_Series_IN", Passport_Series_IN);
            BestCommand.Parameters.AddWithValue("Passport_Number_IN", Passport_Number_IN);
            var r = BestCommand.ExecuteReader();
            r.Read();
            Result = r.GetInt32(0);
            r.Close();
            return Result;
        }
        public int PassengerAddToDB(MySqlConnection connection, string Last_Name_IN, string First_Name_IN, string Pathronymic, int Passport_Series_IN, int Passport_Number_IN, string Passenger_Phone_Number_IN)
        {
            var QueryString = "SELECT PassengerAddToDB(@Last_Name_IN, @First_Name_IN, @Pathronymic, @Passport_Series_IN, @Passport_Number_IN, @Passenger_Phone_Number)";
            var BestCommand = new MySqlCommand(QueryString, connection);
            BestCommand.Parameters.AddWithValue("Last_Name_IN", Last_Name_IN);
            BestCommand.Parameters.AddWithValue("First_Name_IN", First_Name_IN);
            BestCommand.Parameters.AddWithValue("Pathronymic", Pathronymic);
            BestCommand.Parameters.AddWithValue("Passport_Series_IN", Passport_Series_IN);
            BestCommand.Parameters.AddWithValue("Passport_Number_IN", Passport_Number_IN);
            BestCommand.Parameters.AddWithValue("Passenger_Phone_Number", Passenger_Phone_Number_IN);
            var r = BestCommand.ExecuteReader();
            r.Read();
            var result = r.GetInt32(0);
            r.Close();
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
    }
}
