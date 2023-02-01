using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoopMultiDisplay.Patch
{
    public class EventSystemPatch
    {
        [HarmonyPatch(typeof(EventSystem), nameof(EventSystem.RaycastAll), typeof(PointerEventData), typeof(List<RaycastResult>))]
        public class RaycastAll
        {
            private static bool Prefix(EventSystem __instance, PointerEventData eventData, List<RaycastResult> raycastResults)
            {
                if (!CoopMultiDisplayMod.MultiDisplayEnabled || SplitScreenManager._instance.LocalPlayers.Count != 2)
                    return true;
                // optimize by limiting to raycasters that are on player's display
                foreach (BaseRaycaster raycaster in RaycasterManager.GetRaycasters())
                {
                    if (raycaster.GetComponentInParent<Canvas>()?.rootCanvas?.targetDisplay == eventData.playerID)
                        raycaster.Raycast(eventData, raycastResults);
                }
                raycastResults.Sort(EventSystem.s_RaycastComparer);
                return false;
            }
        }
    }
}
