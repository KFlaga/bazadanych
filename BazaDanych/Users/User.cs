using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class User
    {
        public int Id { get; private set; }
        public String Login { get; private set; }
        private String __passwd;
        public int Type { get; private set; }

        public User(int id, String login, string passwd, int type)
        {
            Id = id;
            Login = login;
            __passwd = passwd;
            Type = type;
        }

        public Boolean IsPasswdValid(String passwd)
        {
            if (__passwd.Equals(passwd))
                return true;

            return false;
        }

        public override string ToString()
        {
            return String.Format("({0}): {1}", Id.ToString(), Login);
        }
    }
}
