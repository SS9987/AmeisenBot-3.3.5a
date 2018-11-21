using AmeisenBotData;
using AmeisenBotFSM.Interfaces;
using System.Collections.Generic;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    internal class ActionDoBotStuff : IAction
    {
        public Start StartAction { get { return Start; } }
        public DoThings StartDoThings { get { return DoThings; } }
        public Exit StartExit { get { return Stop; } }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private List<IAction> StuffToDo { get; set; }

        public ActionDoBotStuff(AmeisenDataHolder ameisenDataHolder, List<IAction> stuffToDo)
        {
            AmeisenDataHolder = ameisenDataHolder;
            StuffToDo = stuffToDo;
        }

        public void DoThings()
        {
            DecideWhatToDo();
            DoBotStuff();
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        private void DoBotStuff()
        {
        }

        private void DecideWhatToDo()
        {
        }        
    }
}