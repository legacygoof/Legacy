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
        private byte[] buffer = new byte[1024];
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientSockets = new List<Socket>();

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
            clientSockets.Add(socket);
            Console.WriteLine("Client Connected!");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            ProcessCodes code;
            //PacketReader pr = new PacketReader();
            //PacketWriter pw = new PacketWriter();
            Socket socket = (Socket)ar.AsyncState;
            int received = socket.EndReceive(ar);
            byte[] dataBuff = new byte[received];
            Array.Copy(buffer, dataBuff, received);
            string text = Encoding.ASCII.GetString(dataBuff);
            Console.WriteLine(text);
            string[] msg;
            msg = text.Split(' ');
            try
            {
                // receieved = socket.EndReceive(ar);
                //msg = pr.ReceiveMsg(socket.EndReceive(ar), buffer);//decrypts message and splits into string
                Console.WriteLine(msg[0]);
                Console.WriteLine(msg.Length);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client Forefully disconnected");
                socket.Close();
                clientSockets.Remove(socket);
                return;
            }
            try
            {
                code = (ProcessCodes)UInt16.Parse(msg[0]);
              //  Console.WriteLine("CODE: " + code);
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
                        if (msg[1].Equals("goof") && msg[2].Equals(""))
                        {
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Success));//success error code
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            //socket.Send(data);
                        }
                        else
                        {
                            byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.InvalidLogin));//success error code
                            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                            //socket.Send(data);
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

        private void ProcessLogin(string[] info, IAsyncResult ar)
        {

            Console.WriteLine("Processing Login");
            PacketWriter pw = new PacketWriter();
            Socket socket = (Socket)ar.AsyncState;
            //do db stuff to check login
            if (info[1].Equals("goof") && info[2].Equals(""))
            {
                byte[] data = pw.sendString(Convert.ToString(ErrorCodes.Success));//success error code
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                //socket.Send(data);
            }
            else
            {
                byte[] data = pw.sendString(Convert.ToString(ErrorCodes.InvalidLogin));//success error code
                socket.BeginSend(data, 0, data.Length, SocketFlags.None,new AsyncCallback(SendCallback), socket);
                //socket.Send(data);
            }

        }

        private void ProcessError(IAsyncResult ar)//if theres an error send it
        {
            Console.WriteLine("Client Encountered an error");
            PacketWriter pw = new PacketWriter();
            Socket socket = (Socket)ar.AsyncState;
            byte[] data = pw.sendString(Convert.ToString(ErrorCodes.Error));//success error code
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        }

        private void ProcessRegister(string[] info)
        {
            Console.WriteLine("Processing Resgister");
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
