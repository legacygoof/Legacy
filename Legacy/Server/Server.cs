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
        public string clientVersion = "1.0";
        public bool connectionsOpen = true;
        private byte[] g_buffer = new byte[1024];
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Users> userList = new List<Users>();
        //List<Socket> clientSockets = new List<Socket>();

        public void Start()
        {
            drawLogo();
            Console.WriteLine("Starting Server...");
            server.Bind(new IPEndPoint(IPAddress.Any, 1234));
            server.Listen(5);
            Console.WriteLine("Started!");
            Thread.Sleep(2000);
            Console.Clear();
            drawLogo();
            Console.WriteLine("Listening on port " + 1234 + "\nWaiting for clients");

            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        

        public void ListUsers()
        {
            Console.WriteLine("Users Online: " + userList.Count);//////////////////////////
            Console.WriteLine("-----------------------------|-------------------------------");
            foreach(Users u in userList)
            {
                Console.WriteLine(u.Name + "                            " + u.IP);
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
            Socket socket = user.clientSocket;
            byte[] data = Encoding.ASCII.GetBytes(msg);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Client Connected!");
            Console.ResetColor();
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

            //Everythings done in a try catch in case the client unexpectedly disconnects it doesnt crash the server
            try
            {
                
                received = socket.EndReceive(ar);
                Users user = userList.Find(i => i.IP == socket.RemoteEndPoint.ToString());
                if (user.Name != null && user.Name != "")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Received msg from " + user.Name);
                    Console.ResetColor();
                }
                dataBuff = new byte[received];
                Array.Copy(g_buffer, dataBuff, received);
                text = Encoding.ASCII.GetString(dataBuff);
                msg = text.Split(' ');
            }
            catch (SocketException)
            {
                Users user = userList.Find(i => i.IP == socket.RemoteEndPoint.ToString());
                if (user.Name != null && user.Name != "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(user.Name + " Has Forcefully Disconnected!");
                    Console.ResetColor();
                    Login_Helper.UpdateUser(user.Name);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Random Client Has Forcefully Disconnected! "+ socket.RemoteEndPoint.ToString());
                    Console.ResetColor();
                }

                    var ItemToRemove = userList.Single(i => i.IP == socket.RemoteEndPoint.ToString());
                socket.Close();
                //clientSockets.Remove(socket);
                userList.Remove(ItemToRemove);
                return;
            }
            //incase the code given is not an actual code so we dont run into formatting problems and the server crashes
            try
            {
                
                code = (ProcessCodes)UInt16.Parse(msg[0]);
            }
            catch(System.FormatException e)
            {
                var toRemove = userList.Single(i => i.IP == socket.RemoteEndPoint.ToString());
                userList.Remove(toRemove);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error connecting user, possibly malicaious "  + socket.RemoteEndPoint);
                Console.ResetColor();
                return;
            }
            
             switch (code)//first item is always code
             {
                 case ProcessCodes.Login: {
                        //ProcessLogin(msg,ar);
                        ErrorCodes tempCode = Login_Helper.doLogin(msg[1], msg[2]);
                        byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(tempCode));
                        if(tempCode == ErrorCodes.Success)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(msg[1] + " Has Logged In!");
                            Console.ResetColor();
                            userList.Find(i => i.IP == socket.RemoteEndPoint.ToString()).Name = msg[1];
                            IPEndPoint ipAdd = socket.RemoteEndPoint as IPEndPoint;
                            Login_Helper.UpdateUser(ipAdd.ToString(),msg[1],1);
                        }
                        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                        socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        //Console.WriteLine(Convert.ToString(tempCode));
                        /*if (Login_Helper.doLogin(msg[1], msg[2]) == ErrorCodes.Success)
                        {
                            userList.Find(i => i.IP == socket.RemoteEndPoint.ToString()).Name = msg[1];
                            Console.WriteLine("User " + msg[1] + " Has logged in ");
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Success));//success error code
                            IPEndPoint ipAdd = socket.RemoteEndPoint as IPEndPoint;
                            Login_Helper.UpdateUser(ipAdd.ToString());
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else
                        {
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.InvalidLogin));//success error code
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }*/
                    } break;
                 case ProcessCodes.Version: {
                        Console.WriteLine(text);
                        if(clientVersion.Equals(msg[1]))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Client versions matched, cleared for login");
                            Console.ResetColor();
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Version_Success.ToString()));
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Client versions do match closing client");
                            Console.ResetColor();
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Error.ToString()) + " "+clientVersion);
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }


                    } break;
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
