using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using PlayerModelPro.Scripts;
using UnityEngine;

namespace PlayerModelPro.Scripts
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
            __instance.PreviewModel(0);
        }
    }
}
