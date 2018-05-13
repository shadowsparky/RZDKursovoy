using System;
using System.IO;
using System.Windows;

namespace poselki
{
    class BestErrors
    {
        public void ExceptionProtector()
        {
            MessageBox.Show("Ой, ой. А что это? Это ошибка, но, к сожалению, я о ней уже знал", "Митинг подавлен", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public string getError(string NumError)
        {
            switch (NumError)
            {
                case "1406":
                    NumError = "Вы заполнили слишком длинное значение для одного или нескольких полей";
                    break;
                case "1452":
                    NumError = "He удается добавить или обновить дочернюю строку: ограничение внешнего ключа дает сбой";
                    break;
                case "1062":
                    NumError = "Произошла ошибка. Дублирование информации запрещено";
                    break;
                case "1366":
                    NumError = "Вы заполнили одно или несколько полей некорректно, извинитесь перед разработчиком и пообещайте ему что вы больше так не будете";
                    break;
                case "1370":
                    NumError = "У вас нет необходимых прав, чтобы выполнить данное действие";
                    break;
                case "1394":
                    NumError = "При работе с пользователями произошла ошибка";
                    break;
                case "1396":
                    NumError = "При работе с пользователями произошла ошибка";
                    break;
                case "1133":
                    NumError = "Такого пользователя не существует";
                    break;
                case "1141":
                    NumError = "Такого пользователя не существует";
                    break;
                case "1411":
                    NumError = "Произошла ошибка. Вы неправильно заполнили дату";
                    break;
                case "1292":
                    NumError = "Произошла ошибка. Вы неправильно заполнили время или дату";
                    break;
                default:
                    string ErrorToFile = "Произошла неизвестная ошибка. Номер ошибки - " + NumError;
                    if (LogMessageToFile(ErrorToFile))
                        NumError = "Произошла неизвестная ошибка. Номер ошибки - " + NumError + ". Отправьте разработчику лог - " + GetTempPath() + "AVBIncLogFile.txt";
                    else
                        NumError = "Произошла неизвестная ошибка. Номер ошибки - " + NumError;
                    break;
            }
            return NumError;
        }

        public void CatchError(string [] Error)
        {
            if (Error[1] != "1")
            {
                RZDKursovoy.ApplicationLogic AL = new RZDKursovoy.ApplicationLogic();
                if (Error[1] != "1644")
                {
                    AL.MessageErrorShow(getError(Error[1]), "Ошибка");
                }
                else
                {
                    AL.MessageErrorShow(Error[0], "Ошибка");
                }
            }
        }

        private string GetTempPath()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }
        private bool LogMessageToFile(string msg)
        {
            StreamWriter sw = File.AppendText(
            GetTempPath() + "AVBIncLogFile.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sw.Close();
            }
            return true;
        }
    }
}
