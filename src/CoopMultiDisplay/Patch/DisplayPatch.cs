using HarmonyLib;
using UnityEngine;

namespace CoopMultiDisplay.Patch
{
    public class DisplayPatch
    {
        [HarmonyPatch(typeof(Display), nameof(Display.RelativeMouseAt), typeof(Vector3))]
        public class RelativeMouseAt
        {
            private static bool Prefix(Vector3 inputMouseCoordinates, ref Vector3 __result)
            {
                // Outward emulates cursors to use EventSystem, but GraphicsRaycaster needs the cursor to be on the correct display (see GraphicRaycaster.Raycast).
                // An easy fix is to return Vector3.zero when it calls Display.RelativeMouseAt, since it will just bypass the screen check.
                __result = Vector3.zero;
                return false;
            }
        }
    }
}
