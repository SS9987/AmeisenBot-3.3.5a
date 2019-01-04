using AmeisenBotCore.Structs;
using AmeisenBotLogger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AmeisenBotCore
{
    public class AmeisenEventHook
    {
        public delegate void OnEventFired(long timestamp, List<string> args);
        public bool IsActive { get; private set; }

        private Thread EventReader { get; set; }
        private Dictionary<string, OnEventFired> EventDictionary { get; set; }
        public bool IsNotInWorld { get; set; }

        public AmeisenEventHook()
        {
            EventReader = new Thread(new ThreadStart(ReadEvents));
            EventDictionary = new Dictionary<string, OnEventFired>();
        }

        public void Init()
        {
            StringBuilder luaStuff = new StringBuilder();
            luaStuff.Append("abFrame = CreateFrame(\"FRAME\", \"AbotEventFrame\");");
            luaStuff.Append("abEventTable = {};");
            luaStuff.Append("function abEventHandler(self, event, ...) ");
            luaStuff.Append("table.insert(abEventTable, {time(), event, {...}}) ");
            luaStuff.Append("end;");
            luaStuff.Append("abFrame:SetScript(\"OnEvent\", abEventHandler);");
            AmeisenCore.LuaDoString(luaStuff.ToString());

            IsNotInWorld = false;

            IsActive = true;
            EventReader.Start();
            AmeisenCore.EnableAutoBoPConfirm();
        }

        public void Stop()
        {
            if (IsActive)
            {
                IsActive = false;
                EventReader.Join();
            }
        }

        public void Subscribe(string eventName, OnEventFired onEventFired)
        {
            AmeisenCore.LuaDoString($"abFrame:RegisterEvent(\"{eventName}\");");
            EventDictionary.Add(eventName, onEventFired);
        }

        public void Unsubscribe(string eventName)
        {
            AmeisenCore.LuaDoString($"abFrame:UnregisterEvent(\"{eventName}\");");
            EventDictionary.Remove(eventName);
        }

        private void ReadEvents()
        {
            while (IsActive)
            {
                if (AmeisenCore.IsInLoadingScreen())
                {
                    Thread.Sleep(50);
                    continue;
                }

                // Unminified lua code can be found im my github repo "WowLuaStuff"
                string eventJson = AmeisenCore.GetLocalizedText("abEventJson='['for a,b in pairs(abEventTable)do abEventJson=abEventJson..'{'for c,d in pairs(b)do if type(d)==\"table\"then abEventJson=abEventJson..'\"args\": ['for e,f in pairs(d)do abEventJson=abEventJson..'\"'..f..'\"'if e<=table.getn(d)then abEventJson=abEventJson..','end end;abEventJson=abEventJson..']}'if a<table.getn(abEventTable)then abEventJson=abEventJson..','end else if type(d)==\"string\"then abEventJson=abEventJson..'\"event\": \"'..d..'\",'else abEventJson=abEventJson..'\"time\": \"'..d..'\",'end end end end;abEventJson=abEventJson..']'abEventTable={}", "abEventJson");
                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"LUA Events Json: {eventJson}", this);

                List<RawEvent> rawEvents = new List<RawEvent>();
                try
                {
                    List<RawEvent> finalEvents = new List<RawEvent>();
                    rawEvents = JsonConvert.DeserializeObject<List<RawEvent>>(eventJson);

                    foreach (RawEvent rawEvent in rawEvents)
                    {
                        if (!finalEvents.Contains(rawEvent))
                        {
                            finalEvents.Add(rawEvent);
                        }
                    }

                    AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Parsed {finalEvents.Count} events", this);
                    if (finalEvents.Count > 0)
                    {
                        foreach (RawEvent rawEvent in finalEvents)
                        {
                            if (EventDictionary.ContainsKey(rawEvent.@event))
                            {
                                EventDictionary[rawEvent.@event].Invoke(rawEvent.time, rawEvent.args);
                                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Fired OnEventFired: {rawEvent.@event}", this);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Failed to parse events Json: {e}", this);
                }

                Thread.Sleep(1000);
            }
        }
    }
}
