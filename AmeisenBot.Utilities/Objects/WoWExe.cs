using System.Diagnostics;

namespace AmeisenBotUtilities
{
    public class WowExe
    {
        public string characterName;
        public Process process;
        public bool alreadyHooked;

        public override string ToString() => $"{process.Id.ToString()} - {characterName} {(alreadyHooked ? "- In Use" : "")}";
    }
}