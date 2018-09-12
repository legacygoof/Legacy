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
            Console.WriteLine("Client Connected!");
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
                    Console.WriteLine("Received msg from " + user.Name);
                }
                dataBuff = new byte[received];
                Array.Copy(g_buffer, dataBuff, received);
                text = Encoding.ASCII.GetString(dataBuff);
                msg = text.Split(' ');
            }
            catch (SocketException)
            {
                Console.WriteLine("client forcefully disconnected");
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
                Console.WriteLine("Error connecting user, possibly malicaious "  + socket.RemoteEndPoint);
                return;
            }
            
             switch (code)//first item is always code
             {
                 case ProcessCodes.Login: {
                        //ProcessLogin(msg,ar);

                        if (Login_Helper.doLogin(msg[1], msg[2]) == ErrorCodes.Success)
                        {
                            userList.Find(i => i.IP == socket.RemoteEndPoint.ToString()).Name = msg[1];
                            Console.WriteLine("User " + msg[1] + " Has logged in ");
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Success));//success error code
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        else
                        {
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.InvalidLogin));//success error code
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            socket.BeginReceive(g_buffer, 0, g_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                        } break;
                 case ProcessCodes.Register: { ProcessRegister(msg); }break;
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

        private void drawLogo()
        {
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
        }

    }
}
