using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI
{
    /// <summary>
    /// Class used for storing a socket with a name so we can do things accordingly while they are connected
    /// </summary>
    public class Users
    {
        public Socket clientSocket { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }

        public Users(string IP, Socket clientSocket)
        {
            this.IP = IP;
            this.clientSocket = clientSocket;
        }
    }
}
