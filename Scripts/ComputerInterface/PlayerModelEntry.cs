using ComputerInterface.Interfaces;
using System;

namespace PlayerModelPro.Scripts.ComputerInterface
{
    public class PlayerModelEntry : IComputerModEntry
    {
        // This is the mod name that is going to show up as a selectable mod
        public string EntryName => "PlayerModelPro";

        // This is the first view that is going to be shown if the user select you mod
        // The Computer Interface mod will instantiate your view 
        public Type EntryViewType => typeof(PlayerModelExplorer);
    }
}
