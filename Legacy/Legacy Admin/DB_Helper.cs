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
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
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
           
            
            
        }

        public static void Update_Tokens()
        {
            dbConn.Open();
            sdaTokens = new MySqlDataAdapter("SELECT * FROM tokens", dbConn);
            MySqlCommandBuilder cmd = new MySqlCommandBuilder(sdaTokens);
            sdaTokens.UpdateCommand = cmd.GetUpdateCommand();
            sdaTokens.Update(tokenDT);
            dbConn.Close();



        }

        public static void Generate_Token(int days)
        {
            string tokenid = RandomString(25);

            string query = "INSERT INTO tokens (tokenid,days,used,usedby,buyer,seller) VALUES(@token,@day,@used,@usedby,@buyer,@seller)";
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@token", tokenid);
            cmd.Parameters.AddWithValue("@day", days);
            cmd.Parameters.AddWithValue("@used", Convert.ToBoolean(false));
            cmd.Parameters.AddWithValue("@usedby", "");
            cmd.Parameters.AddWithValue("@buyer", "");
            cmd.Parameters.AddWithValue("@seller", "");
            cmd.Connection = dbConn;
            dbConn.Open();
            cmd.ExecuteReader();
            dbConn.Close();
        }

        public static void Generate_Token(int days, string tokenid)
        {
                string query = "INSERT INTO tokens (tokenid,days,used,usedby,buyer,seller) VALUES(@token,@day,@used,@usedby,@buyer,@seller)";
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@token", tokenid);
                cmd.Parameters.AddWithValue("@day", days);
                cmd.Parameters.AddWithValue("@used", Convert.ToBoolean(false));
                cmd.Parameters.AddWithValue("@usedby", "");
                cmd.Parameters.AddWithValue("@buyer", "");
                cmd.Parameters.AddWithValue("@seller", "");
                cmd.Connection = dbConn;
                dbConn.Open();
                cmd.ExecuteReader();
                dbConn.Close();
        }
        public static void Generate_Token(int days, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                string tokenid = RandomString(25);
                string query = "INSERT INTO tokens (tokenid,days,used,usedby,buyer,seller) VALUES(@token,@day,@used,@usedby,@buyer,@seller)";
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@token", tokenid);
                cmd.Parameters.AddWithValue("@day", days);
                cmd.Parameters.AddWithValue("@used", Convert.ToBoolean(false));
                cmd.Parameters.AddWithValue("@usedby", "");
                cmd.Parameters.AddWithValue("@buyer", "");
                cmd.Parameters.AddWithValue("@seller", "");
                cmd.Connection = dbConn;
                dbConn.Open();
                cmd.ExecuteReader();
                dbConn.Close();
            }
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
