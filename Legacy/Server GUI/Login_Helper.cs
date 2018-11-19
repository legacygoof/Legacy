using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace Server_GUI
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
            try
            {
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
                        Log.Warning(username + " Attempted To Login But Was Already Logged In!");
                        return ErrorCodes.AlreadyLogged;
                    }
                    if (reader.GetBoolean("Banned"))
                    {
                        dbConn.Close();
                        Log.Error(username + " Attempted To Login But Is Banned!");
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
                    Log.Warning("Someone typed in the wrong info for user: " + username);
                    return ErrorCodes.InvalidLogin;
                }
            }
            catch (MySqlException ex)
            {

                return ErrorCodes.Error;
            }

        }
        //set user to logged in and update ip
        public static void UpdateUser(string ip, string username, int type)
        {
            try
            {
                string query = "UPDATE Users SET IP=@ip,LoggedIn=@logged Where Username=@uname";
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@ip", ip);
                cmd.Parameters.AddWithValue("@logged", Convert.ToBoolean(true));
                cmd.Parameters.AddWithValue("@uname", username);
                cmd.Connection = dbConn;
                dbConn.Open();
                cmd.ExecuteReader();
                dbConn.Close();
            }
            catch (MySqlException ex)
            {

            }
        }
        //when user logs out, overloaded method cause lazy :p
        public static void UpdateUser(string username)
        {
            string query = "UPDATE Users SET LoggedIn=@logged Where Username=@uname";
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@logged", Convert.ToBoolean(false));
            cmd.Parameters.AddWithValue("@uname", username);
            cmd.Connection = dbConn;
            dbConn.Open();
            cmd.ExecuteReader();
            dbConn.Close();
        }

        public static void BanUser(string username)
        {
            string query = "UPDATE Users SET Banned=@banned Where Username=@uname";
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@banned", Convert.ToBoolean(true));
            cmd.Parameters.AddWithValue("@uname", username);
            cmd.Connection = dbConn;
            dbConn.Open();
            cmd.ExecuteReader();
            dbConn.Close();
            Log.Error(username + " Has Been Banned By Server!");
        }

        public static void UnBanUser(string username)
        {
            string query = "UPDATE Users SET Banned=@banned Where Username=@uname";
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@banned", Convert.ToBoolean(false));
            cmd.Parameters.AddWithValue("@uname", username);
            cmd.Connection = dbConn;
            dbConn.Open();
            cmd.ExecuteReader();
            dbConn.Close();
            Log.Success(username + " Has Been Unbanned By Server!");
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

            try
            {
                dbConn.Open();
                dbConn.Close();
                Log.Warning("DB Intialized");
            }
            catch (MySqlException)
            {
                Log.Error("DB Failed to initialize");
            }
        }
    }
}
