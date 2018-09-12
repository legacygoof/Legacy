using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Login_Helper
    {
        /// <summary>
        /// Check database and if username/password combo exists then return true else return false
        /// </summary>
        /// <param name="username">username received from client</param>
        /// <param name="password">password received from client</param>
        /// <returns></returns>
        public static ErrorCodes doLogin(string username, string password)
        {
            ErrorCodes code;
            if (username == "goof" && password == "goof")
                code = ErrorCodes.Success;
            else
                code = ErrorCodes.InvalidLogin;

            return code;
        }

        public static ErrorCodes doRegister(string email, string username, string password)
        {

            return ErrorCodes.Exists;
        }
    }
}
