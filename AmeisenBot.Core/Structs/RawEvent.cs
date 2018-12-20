using System.Collections.Generic;

namespace AmeisenBotCore.Structs
{
    public struct RawEvent
    {
        public long time;
        public string @event;
        public List<string> args;
    }
}
