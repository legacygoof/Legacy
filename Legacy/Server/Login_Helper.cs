using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Server
{
    class Login_Helper
    {
        //database config
        private static string SERVER = "127.0.0.1";
        private static string DATABASE = "legacy";
        private static string UID = "root";
        private static string PASSWORD = "";
        private static MySqlConnection dbConn;
        /// <summary>
        /// Check database and if username/password combo exists then return true else return false
        /// </summary>
        /// <param name="username">username received from client</param>
        /// <param name="password">password received from client</param>
        /// <returns></returns>
        public static ErrorCodes doLogin(string username, string password)
        {
            ErrorCodes code;
                string query = "SELECT * FROM users WHERE Username ='" + username + "' AND Password ='" + password + "'";
                MySqlCommand cmd = new MySqlCommand(query, dbConn);
                dbConn.Open();

                MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                if (reader.GetBoolean("LoggedIn"))
                {
                    dbConn.Close();
                    Console.WriteLine(username + " Attempted To Login But Was Already Logged In!");
                    return ErrorCodes.AlreadyLogged;
                }
                if (reader.GetBoolean("Banned"))
                {
                    dbConn.Close();
                    Console.WriteLine(username + " Attempted To Login But Is Banned!");
                    return ErrorCodes.Banned;
                }

                else
                {
                    dbConn.Close();
                    return ErrorCodes.Success;
                }
            }


            else
            {
                dbConn.Close();
                return ErrorCodes.InvalidLogin;
            }
            
        }

        public static void UpdateUser(string ip)
        {

            
        }

        public static ErrorCodes doRegister(string email, string username, string password)
        {

            return ErrorCodes.Exists;
        }

        public static void InitializeDB()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = SERVER;
            builder.UserID = UID;
            builder.Password = PASSWORD;
            builder.Database = DATABASE;
            builder.SslMode = MySqlSslMode.None;

            string connString = builder.ToString();

            dbConn = new MySqlConnection(connString);
        }
    }
}
