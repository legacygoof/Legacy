using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PacketReader
    {
        public string ReadString(int size, byte[] data)
        {
            string txt = "";

            return txt;
        }

        public string[] ReceiveMsg(int size, byte[] data)
        {
            byte[] temp = new byte[size];
            Array.Copy(data, temp, size);//copies data

            string txt = Encoding.ASCII.GetString(temp);

            string[] msg = txt.Split(' ');

            return msg;
        }
    }
}
