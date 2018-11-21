using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Server
{
    class Server
    {
        #region server variables
        public string clientVersion = "1.0";
        public string adminVersion = "admin_1.0";
        public bool connectionsOpen = true;
        private byte[] g_buffer = new byte[1024];
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Users> userList = new List<Users>();
        List<Admins> adminList = new List<Admins>();
        //List<Socket> clientSockets = new List<Socket>();
        #endregion
        public void Start()
        {
            drawLogo();
            Log.Write("Starting Server...");
            server.Bind(new IPEndPoint(IPAddress.Any, 1234));
            server.Listen(5);
            Log.Write("Started!");
            Thread.Sleep(2000);
            Console.Clear();
            drawLogo();
            Log.Write("Listening on port " + 1234 + "\nWaiting for clients");

            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }


        #region admin commands
        public void ListUsers()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Users Online: " + userList.Count);//////////////////////////
            Console.WriteLine("-----------------------------|-------------------------------");
            foreach(Users u in userList)
            {
                Console.WriteLine(u.Name + "                            " + u.IP);
            }
            Console.ResetColor();
        }
        // user ip, user ip
        //
        public void sendUpdatedUsers()
        {
            string msg = ProcessCodes.UpdateUsers.ToString()+" ,";
            foreach(Users u in userList)
            {
                if (u.Name != "" || u.Name != null)
                    msg += u.Name + " " + u.IP + " ";
            }
            foreach (Admins a in adminList)
            {
                Socket socket = a.clientSocket;
                byte[] data = Encoding.ASCII.GetBytes(msg);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            }
        }

        public void sendAdminsLog(string message)
        {
            string msg = ProcessCodes.Logger.ToString() + " " + message;
            foreach(Admins a in adminList)
            {
                //if (userList.Find(i => i.IP == a.IP).IP != "" || userList.Find(i => i.IP == a.IP).IP != null)
               // {
                    Socket socket = a.clientSocket;
                    byte[] data = Encoding.ASCII.GetBytes(msg);
                    socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
              //  }
            }
        }

        public void KickUser(string username, string message)
        {
            string msg = ProcessCodes.Kick.ToString() + " " + message;
            Users user = userList.Find(i => i.Name == username);
            Socket socket = user.clientSocket;
            byte[] data = Encoding.ASCII.GetBytes(msg);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        }

        public void SendUserMsg(string username, string message)
        {
            string msg = ProcessCodes.Message.ToString() + " " + message;
            Users user = userList.Find(i => i.Name == username);
            Admins admin = adminList.Find(i => i.Name == username);
            if (user != null)
            {
                Socket socket = user.clientSocket;
                byte[] data = Encoding.ASCII.GetBytes(msg);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

            }
            else if(admin!=null)
            {
                Socket socket = admin.clientSocket;
                byte[] data = Encoding.ASCII.GetBytes(msg);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            }
        }

        public void SendUserMsgAll(string message)
        {
            string msg = ProcessCodes.Message.ToString() + " " + message;
            foreach (Users user in userList)
            {
                Socket socket = user.clientSocket;
                byte[] data = Encoding.ASCII.GetBytes(msg);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                
            }

        }

        public void BanUser(string username, string message)
        {
            string msg = ProcessCodes.Message.ToString() + " " + message;
            Users user = userList.Find(i => i.Name == username);
            Socket socket = user.clientSocket;
            byte[] data = Encoding.ASCII.GetBytes(msg);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        }

        public void RebootServer()
        {
            
            string msg = ProcessCodes.Reboot.ToString() + " Server is rebooting or an update is required exiting application now!";
            foreach (Users u in userList)
            {
                Socket socket = u.clientSocket;
                byte[] data = Encoding.ASCII.GetBytes(msg);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            }
        }
        #endregion

        #region server client processing
        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socket;
            try
            {
                socket = server.EndAccept(ar);

            }
            catch (ObjectDisposedException)
            {
                return;
            }
            userList.Add(new Users(socket.RemoteEndPoint.ToString(), socket));
            Log.Success("Client Connected!");
            sendAdminsLog("GREEN " + "Client Connected!");
            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            //better to initailize these outside the try catch to make them accessable throughout method
            Socket socket = (Socket)ar.AsyncState;
            ProcessCodes code;
            string[] msg;
            int received;
            string text;
            byte[] dataBuff;

            //Everythings done in a try catch in case the client unexpectedly disconnects so it doesnt crash the server
            try
            {
                
                received = socket.EndReceive(ar);
                Users user = userList.Find(i => i.IP == socket.RemoteEndPoint.ToString());
                Admins admin = adminList.Find(i => i.IP == socket.RemoteEndPoint.ToString());
                dataBuff = new byte[received];
                Array.Copy(g_buffer, dataBuff, received);
                text = Encoding.ASCII.GetString(dataBuff);
                msg = text.Split(' ');
                if ((user != null) && (user.Name != null && user.Name != ""))
                {
                    Log.Success("Received msg from " + user.Name);
                    sendAdminsLog("GREEN " + "Received msg from " + user.Name);
                }
                else if ((admin != null) && (admin.Name != null && admin.Name != "") && (msg[1] == "KICK" || msg[1] == "BAN" || msg[1] == "SENDMSG"))
                {
                    Log.AdminConnect(admin.Name + " Issued a "+msg[1]);
                    sendAdminsLog("GREEN " + "Received msg from " + admin.Name);
                }
                
            }
            catch (SocketException)
            {
                int msgtype = 0;
                string rep = "";
                Users user = userList.Find(i => i.IP == socket.RemoteEndPoint.ToString());
                Admins admin = adminList.Find(i => i.IP == socket.RemoteEndPoint.ToString());
                if (user != null && user.Name != null && user.Name != "")
                {

                    Log.Warning(user.Name + " Has Forcefully Disconnected!");
                    msgtype = 1;
                    Login_Helper.UpdateUser(user.Name);
                }
                else if (admin != null && admin.Name != null && admin.Name != "")
                {

                    Log.Warning("ADMIN: "+admin.Name + " Has Forcefully Disconnected!");
                    msgtype = 3;
                    Login_Helper.UpdateUser(admin.Name);
                }
                else
                {
                    Log.Error("Random Client Has Forcefully Disconnected! "+ socket.RemoteEndPoint.ToString());
                    rep = socket.RemoteEndPoint.ToString();
                    msgtype = 2;
                }

                var ItemToRemove = userList.SingleOrDefault(i => i.IP == socket.RemoteEndPoint.ToString());
                var AdminToRemove = adminList.SingleOrDefault(i => i.IP == socket.RemoteEndPoint.ToString());
                socket.Close();
                //clientSockets.Remove(socket);
                if(ItemToRemove != null)
                    userList.Remove(ItemToRemove);
                if (AdminToRemove != null)
                    adminList.Remove(AdminToRemove);
                if (msgtype == 2)
                    sendAdminsLog("RED " + "Random Client Has Forcefully Disconnected!  " + rep);
                else if(msgtype == 1)
                    sendAdminsLog("YELLOW " + user.Name + " Has Forcefully Disconnected!");
                else if(msgtype == 3)
                    sendAdminsLog("YELLOW " + admin.Name + " Has Forcefully Disconnected!");
                return;
            }
            //incase the code given is not an actual code so we dont run into formatting problems and the server crashes
            try
            {
                
                code = (ProcessCodes)UInt16.Parse(msg[0]);
            }
            catch(System.FormatException e)
            {
                var ItemToRemove = userList.SingleOrDefault(i => i.IP == socket.RemoteEndPoint.ToString());
                var AdminToRemove = adminList.SingleOrDefault(i => i.IP == socket.RemoteEndPoint.ToString());
                //socket.Close();
                //clientSockets.Remove(socket);
                if (ItemToRemove != null)
                    userList.Remove(ItemToRemove);
                if (AdminToRemove != null)
                    adminList.Remove(AdminToRemove);
                Log.Error("Error connecting user, possibly malicaious "  + socket.RemoteEndPoint);
                sendAdminsLog("RED " + "Error connecting user, possibly malicaious " + socket.RemoteEndPoint);
                return;
            }
            
             switch (code)//first item is always code
             {
                 case ProcessCodes.Login: {
                        ErrorCodes tempCode = Login_Helper.doLogin(msg[1], msg[2]);
                        byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(tempCode));
                        if(tempCode == ErrorCodes.Success)
                        {
                            Log.Success(msg[1] + " Has Logged In!");
                            sendAdminsLog("GREEN "+ msg[1] + " Has Logged In!");
                            if(Login_Helper.checkAdmin(msg[1]))
                            {
                                adminList.Add(new Admins(userList.Find(i => i.IP == socket.RemoteEndPoint.ToString()).IP, userList.Find(i => i.IP == socket.RemoteEndPoint.ToString()).clientSocket));
                                adminList.Find(i => i.IP == socket.RemoteEndPoint.ToString()).Name = msg[1];
                                var toRemove = userList.Single(i => i.IP == socket.RemoteEndPoint.ToString());
                                userList.Remove(toRemove);
                            }
                            else
                                userList.Find(i => i.IP == socket.RemoteEndPoint.ToString()).Name = msg[1];
                            
                            IPEndPoint ipAdd = socket.RemoteEndPoint as IPEndPoint;
                            Login_Helper.UpdateUser(ipAdd.ToString(),msg[1],1);
                        }
                        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                        socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                    } break;
                 case ProcessCodes.Version: {
                        if(clientVersion.Equals(msg[1]))
                        {
                            Log.Success("Client versions matched, cleared for login");
                            sendAdminsLog("GREEN " + "Client versions matched, cleared for login");
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Version_Success.ToString()));
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else
                        {
                            Log.Warning("Client versions do match closing client");
                            sendAdminsLog("YELLOW " + "Client versions do match closing client");
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Error.ToString()) + " "+clientVersion);
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }


                    } break;
                case ProcessCodes.Command:
                    {
                        string message = "";
                        for(int i = 3; i < msg.Length; i++)
                        {
                            message += msg[i] +" ";
                        }
                        if(msg[1] == "KICK")
                        {
                            KickUser(msg[2], message);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else if (msg[1] == "BAN")
                        {
                            BanUser(msg[2], message);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else if (msg[1] == "SENDMSG")
                        {
                            SendUserMsg(msg[2], message);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else if (msg[1] == "SENDMSGALL")
                        {
                            SendUserMsgAll(message);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else if (msg[1] == "UPDATEUSERS")
                        {
                            sendUpdatedUsers();
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }

                    }
                    break;
                  default: { ProcessError(ar); }break;
             }
        }



        private void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        private void ProcessError(IAsyncResult AR)
        {

        }
        #endregion
        public void drawLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("$$\\                                                        ");
            Console.WriteLine("$$ |                                                       ");
            Console.WriteLine("$$ |      $$$$$$\\   $$$$$$\\   $$$$$$\\   $$$$$$$\\ $$\\   $$\\ ");
            Console.WriteLine("$$ |     $$  __$$\\ $$  __$$\\  \\____$$\\ $$  _____|$$ |  $$ |");
            Console.WriteLine("$$ |     $$$$$$$$ |$$ /  $$ | $$$$$$$ |$$ /      $$ |  $$ |");
            Console.WriteLine("$$ |     $$   ____|$$ |  $$ |$$  __$$ |$$ |      $$ |  $$ |");
            Console.WriteLine("$$$$$$$$\\$$$$$$$\\ \\$$$$$$$ |\\$$$$$$$ |\\$$$$$$$\\ \\$$$$$$$ |");
            Console.WriteLine("\\________|\\_______| \\____$$ | \\_______| \\_______| \\____$$ |");
            Console.WriteLine("                   $$\\   $$ |                    $$\\   $$ |");
            Console.WriteLine("                   \\$$$$$$  |                    \\$$$$$$  |");
            Console.WriteLine("                    \\______/                      \\______/ ");
            Console.ResetColor();
        }

    }
}
