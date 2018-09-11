using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// Main Thread of the server
        /// </summary>
        /// <param name="args"></param>
        /// 
        static Server server = new Server();
        static void Main(string[] args)
        {

            server.Start();
            Console.ReadLine();//reading line so it doesnt close right away
        }
    }
}
