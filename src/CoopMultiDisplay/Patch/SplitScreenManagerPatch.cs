using HarmonyLib;

namespace CoopMultiDisplay.Patch
{
    public class SplitScreenManagerPatch
    {
        [HarmonyPatch(typeof(SplitScreenManager), nameof(SplitScreenManager.RenderInImage), MethodType.Getter)]
        public class RenderInImage
        {
            public static bool Prefix(SplitScreenManager __instance, ref bool __result)
            {
                if (__instance.CurrentSplitType != SplitScreenManager.SplitType.Horizontal)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
