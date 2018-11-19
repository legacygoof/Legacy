using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI
{
    public class LogEventArgs : EventArgs
    {
        string msg { get; set; }
    }
    public class Log 
    {
        public static string file = "LOG.txt";

        public static void InitiateLog()
        {

        }
        public static string Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
            Logger(msg);
            return "Success: " + msg;
        }
        public static string Write(string msg)
        {

            Console.WriteLine(msg);
            Logger(msg);
            return msg;
        }
        public static string Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
            Logger(msg);
            return "Error: " + msg;
        }
        public static string Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
            Logger(msg);
            return "Warning: " + msg;
        }

        private static void Logger(string msg)
        {
            using (System.IO.StreamWriter sw = System.IO.File.AppendText(AppDomain.CurrentDomain.BaseDirectory + file))
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
                sw.Close();
            }

        }
    }
}
