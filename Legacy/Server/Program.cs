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
            Log.InitiateLog();

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
                    string msg = "";
                    for (int i = 2; i < cmdArgs.Length; i++)
                        msg += cmdArgs[i] + " ";
                    server.BanUser(cmdArgs[1], msg);
                    Log.Warning("Server Issued Ban To " + cmdArgs[1]);
                }

                else if (cmdArgs[0].ToLower() == "clear")
                {
                    Log.Warning("Clear cmd Called");
                    Console.Clear();
                    server.drawLogo();
                }
                else if (cmdArgs[0].ToLower() == "list")
                {
                    server.ListUsers();
                    Log.Warning("List cmd Called");
                }
                else if (cmdArgs[0].ToLower() == "admins")
                {
                    //lists all admins online currently
                }
                else if (cmdArgs[0].ToLower() == "give")
                {
                    //
                }
                else if(cmdArgs[0].ToLower() == "reboot")
                {
                    server.RebootServer();
                    Log.Warning("SERVER CMD REBOOT CALLED");
                }
                else if (cmdArgs[0].ToLower() == "unban")
                {
                    Login_Helper.UnBanUser(cmdArgs[1]);
                    Log.Warning("Server Unbanned User " + cmdArgs[1]);

                }
                else if (cmdArgs[0].ToLower() == "kick")
                {
                    Log.Warning("Server Kicked User " + cmdArgs[1]);
                    string msg = "";
                    for (int i = 2; i < cmdArgs.Length; i++)
                        msg += cmdArgs[i] + " ";
                    server.KickUser(cmdArgs[1], msg);
                }
                else if (cmdArgs[0].ToLower() == "send")
                {
                    Log.Warning("Server Sent Message To User " + cmdArgs[1]);
                    string msg = "";
                    for (int i = 2; i < cmdArgs.Length; i++)
                        msg += cmdArgs[i] +" ";
                    server.SendUserMsg(cmdArgs[1], msg);
                }
            }
        }

    }
       
}
