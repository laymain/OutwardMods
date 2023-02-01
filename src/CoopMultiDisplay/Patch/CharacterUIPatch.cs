using HarmonyLib;
using UnityEngine;

namespace CoopMultiDisplay.Patch
{
    public class CharacterUIPatch
    {
        [HarmonyPatch(typeof(CharacterUI), nameof(CharacterUI.DelayedRefreshSize))]
        public class DelayedRefreshSize
        {
            private static bool Prefix(CharacterUI __instance)
            {
                if (CoopMultiDisplayMod.MultiDisplayEnabled)
                {
                    Vector2 screenSize = __instance.RewiredID == 0 ? MenuManager.Instance.m_characterUIHolder.sizeDelta : MenuManager.Instance.m_characterUIHolderP2.sizeDelta;
                    __instance.m_rectTransform.sizeDelta = Vector2.Scale(__instance.m_targetSizeDelta, screenSize);
                    __instance.m_rectTransform.anchoredPosition = Vector2.Scale(__instance.m_targetAnchorPos, __instance.m_rectTransform.sizeDelta);
                    __instance.m_canvasGroup.alpha = 1f;
                    return false;
                }
                return true;
            }
        }
    }
}
