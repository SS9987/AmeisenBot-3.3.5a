using AmeisenBot.Character;
using AmeisenBotData;
using AmeisenBotDB;
using AmeisenBotFSM.BotStuff;
using AmeisenBotFSM.Interfaces;
using System.Collections.Generic;
using System.Threading;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    internal class ActionDoBotStuff : IAction
    {
        public Start StartAction { get { return Start; } }
        public DoThings StartDoThings { get { return DoThings; } }
        public Exit StartExit { get { return Stop; } }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenCharacterManager AmeisenCharacterManager { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }
        private List<IAction> StuffToDo { get; set; }
        private IAction ThingTodo { get; set; }

        public ActionDoBotStuff(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenCharacterManager ameisenCharacterManager,
            List<IAction> stuffToDo)
        {
            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            AmeisenCharacterManager = ameisenCharacterManager;
            StuffToDo = stuffToDo;
        }

        public void DoThings()
        {
            if (ThingTodo != null)
            {
                DoBotStuff(ThingTodo);
            }
            else
            {
                // got nothing to do
                Thread.Sleep(1000);
            }
        }

        public void Start()
        {
            ThingTodo = DecideWhatToDo();
        }

        public void Stop()
        {
        }

        private void DoBotStuff(IAction whatToDo)
        {
            whatToDo.StartAction?.Invoke();
        }

        private IAction DecideWhatToDo()
        {
            return new BotStuffRepairEquip(AmeisenDataHolder, AmeisenDBManager, AmeisenCharacterManager);
        }
    }
}