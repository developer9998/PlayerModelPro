using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using PlayerModelPlus.Scripts;

namespace PlayerModelPlus.Scripts
{
    /// <summary>
	/// This is an example patch, made to demonstrate how to use Harmony. You should remove it if it is not used.
	/// </summary>

	[HarmonyPatch(typeof(Controller))]
    [HarmonyPatch("Start", MethodType.Normal)]
    internal class ControllerStartPatch
    {
        private static void Prefix(Controller __instance)
        {
            if (Controller.Instance == null)
                __instance.PreviewModel(0);
            else
                Controller.Instance.PreviewModel(0);
        }
    }
}
