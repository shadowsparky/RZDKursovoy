﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RZDKursovoy
{
    class ApplicationLogic
    {
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
                if (userControl == "Add")
                    MessageBox.Show("Запись добавлена", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Запись отредактирована", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Запись удалена", "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
        public List <int> ThrowTrainNumbersList(MySqlConnection connection, string TrainNumber, string RailcarType)
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
        public List<string> TrainInfo(MySqlConnection connected, string TrainNumber, int Arrival_ID_IN, int Departure_ID_IN)
        {
            List<string> TrainInfoList = new List<string>();
            var QueryString = "call TrainInfo(@TrainNumber, @Arrival_ID_IN, @Departure_ID_IN)";
            var BestCommand = new MySqlCommand(QueryString, connected);
            BestCommand.Parameters.AddWithValue("TrainNumber",TrainNumber);
            BestCommand.Parameters.AddWithValue("Arrival_ID_IN", Arrival_ID_IN);
            BestCommand.Parameters.AddWithValue("Departure_ID_IN", Departure_ID_IN);
            var r = BestCommand.ExecuteReader();
            r.Read();
            TrainInfoList.Add(r.GetString(0).ToString());
            TrainInfoList.Add(r.GetString(1).ToString());
            var DateSplit = r.GetString(2).ToString().Split(' ');
            TrainInfoList.Add(DateSplit[0]);
            TrainInfoList.Add(r.GetString(3).ToString());
            r.Close();
            return TrainInfoList;
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
    }
}
