using System.Collections.Generic;

namespace AmeisenBotCore.Structs
{
    public struct RawEvent
    {
        public long time;
        public string eventname;
        public List<string> args;
    }
}
