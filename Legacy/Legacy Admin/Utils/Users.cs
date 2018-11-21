using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy_Admin.Utils
{
    public class Users
    {
        
        public string Name { get; set; }
        public string IP { get; set; }

        public Users(string Name, string IP)
        {
            this.IP = IP;
            this.Name = Name;
        }
    }
}
