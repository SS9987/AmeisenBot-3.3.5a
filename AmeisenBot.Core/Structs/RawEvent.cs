using System.Collections.Generic;

namespace AmeisenBotCore.Structs
{
    public struct RawEvent
    {
        public long timestamp;
        public string eventname;
        public List<string> args;
    }
}
