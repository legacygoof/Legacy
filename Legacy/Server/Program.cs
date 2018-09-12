using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;




/// <summary>
/// Main Server to handle all connections being made for auth and database usage
/// we dont want our users to get control of our database so best to keep it all server sided
/// </summary>
namespace Server
{
    class Program
    {

        private static Server server = new Server();
        /// <summary>
        /// Main Thread of the server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Thread cmdLoop = new Thread(CommandLoop);
           // cmdLoop.IsBackground = true;
            cmdLoop.Start();
            server.Start();
            Login_Helper.InitializeDB();

            //Console.ReadLine();
        }


        private static void CommandLoop()
        {
            while(true)
            {
                string cmd = Console.ReadLine();
                string[] cmdArgs = cmd.Split(' ');

                if(cmdArgs[0].ToLower() == "ban")
                {
                    Login_Helper.BanUser(cmdArgs[1]);
                }

                else if (cmdArgs[0].ToLower() == "clear")
                {
                    Console.Clear();
                    server.drawLogo();
                }
                else if (cmdArgs[0].ToLower() == "admins")
                {
                    //lists all admins online currently
                }
                else if (cmdArgs[0].ToLower() == "give")
                {
                    //
                }
                else if (cmdArgs[0].ToLower() == "unban")
                {
                    //unbans a user
                }
                else if (cmdArgs[0].ToLower() == "kick")
                {
                    //kick a user out of app
                }
            }
        }

    }
       
}
