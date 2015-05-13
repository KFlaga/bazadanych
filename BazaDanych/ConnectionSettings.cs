using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class ConnectionSettings
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string TNSName { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string SID { get; set; }
        public bool AsAdmin { get; set; }
        public bool UseTNS { get; set; }
    
        public void SetDefault()
        {
            TNSName = "TESTKAMIL";
            Host = "localhost";
            Port = "1522";
            SID = "bazatest";
            AsAdmin = false;
            UseTNS = false;
        }
    }
}
