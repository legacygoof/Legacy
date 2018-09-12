using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;




/// <summary>
/// Main Server to handle all connections being made for auth and database usage
/// we dont want our users to get control of our database so best to keep it all server sided
/// </summary>
namespace Server
{
    class Program
    {


        private static byte[] _BUFFER = new byte[1024]; 
        private static List<Users> _userList = new List<Users>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Server server = new Server();
        /// <summary>
        /// Main Thread of the server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            server.Start();
            Login_Helper.InitializeDB();
            Console.ReadLine();
        }

        private static void Start_Server()
        {
            Console.WriteLine("Starting Server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1234));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            _userList.Add(new Users(socket.RemoteEndPoint.ToString(), socket));
            Console.WriteLine("Client Connected: " + socket.RemoteEndPoint);
            socket.BeginReceive(_BUFFER, 0, _BUFFER.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;

            int receivedSize = socket.EndReceive(AR);
            byte[] databuff = new byte[receivedSize];
            Array.Copy(_BUFFER, databuff, receivedSize);

            string msg = Encoding.ASCII.GetString(databuff);
            string[] msgArgs = msg.Split(' ');
            Console.WriteLine("Received msg: " + msg);
            ProcessCodes code = (ProcessCodes)UInt16.Parse(msgArgs[0]);

            if (code == ProcessCodes.Login)
            {
                if(Login_Helper.doLogin(msgArgs[1], msgArgs[2]) == ErrorCodes.Success)
                {
                    
                    byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.Success));
                    socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                    socket.BeginReceive(_BUFFER, 0, _BUFFER.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
                else
                {
                    byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(ErrorCodes.InvalidLogin));
                    socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                    socket.BeginReceive(_BUFFER, 0, _BUFFER.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
            }

            if (code == ProcessCodes.Register)
            {

            }

            if (code == ProcessCodes.Fail)
            {

            }
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
