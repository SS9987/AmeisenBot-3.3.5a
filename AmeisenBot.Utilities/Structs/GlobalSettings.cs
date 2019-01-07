using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenBotUtilities.Structs
{
    public class GlobalSettings
    {
        public string wowExePath = "none";
        public string wowRealmlistPath = "none";
        public int wowSelectedRealmlist = 0;
        public List<string> wowRealmlists = new List<string>() { "127.0.0.1" };
    }
}
