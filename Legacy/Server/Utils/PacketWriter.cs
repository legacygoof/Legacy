using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PacketWriter
    {
        public byte[] sendString(string msg)
        {
            byte[] data = Encoding.ASCII.GetBytes(msg);
            return data;
        }
    }
}
