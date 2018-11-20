using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Legacy_Admin
{
    class DB_Helper
    {
        //database config
        private static string SERVER = "127.0.0.1";
        private static string DATABASE = "legacy";
        private static string UID = "root";
        private static string PASSWORD = "";
        public static DataTable userDT = new DataTable();
        public static DataTable tokenDT = new DataTable();
        private static MySqlConnection dbConn;
        static MySqlDataAdapter sdaUsers;
        static MySqlDataAdapter sdaTokens;

        public static DataTable getDbClientInfo()
        {
            DataTable dt = new DataTable();

            /* string query = "SELECT * FROM users";
             MySqlCommand cmd = new MySqlCommand(query, dbConn);
             dbConn.Open();
             MySqlDataReader reader = cmd.ExecuteReader();
             dt.Load(reader);
             dbConn.Close();*/
            sdaUsers = new MySqlDataAdapter("SELECT * FROM users", dbConn);
            userDT.Clear();
            sdaUsers.Fill(userDT);

            return userDT;
        }


        public static DataTable getTokenInfo()
        {
            DataTable dt = new DataTable();

            /*string query = "SELECT * FROM tokens";
            MySqlCommand cmd = new MySqlCommand(query, dbConn);
            dbConn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            dt.Load(reader);
            dbConn.Close();*/
            sdaTokens = new MySqlDataAdapter("SELECT * FROM tokens", dbConn);
            tokenDT.Clear();
            sdaTokens.Fill(tokenDT);

            return tokenDT;
        }

        public static void Update_Users()
        {
            dbConn.Open();
            sdaUsers = new MySqlDataAdapter("SELECT * FROM users", dbConn);
            MySqlCommandBuilder cmd = new MySqlCommandBuilder(sdaUsers);
            sdaUsers.UpdateCommand = cmd.GetUpdateCommand();
            sdaUsers.Update(userDT);
        
            
            dbConn.Close();
            MessageBox.Show("DONE!");
           
            
            
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
            }
            catch (MySqlException)
            {
                MessageBox.Show("DB Failed to initialize");
            }
        }

    }
}
