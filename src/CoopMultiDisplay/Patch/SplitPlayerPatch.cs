using HarmonyLib;

namespace CoopMultiDisplay.Patch
{
    public class SplitPlayerPatch
    {
        [HarmonyPatch(typeof(SplitPlayer), nameof(SplitPlayer.InitCamera))]
        public class InitCamera
        {
            private static void Postfix(SplitPlayer __instance)
            {
                if (__instance.RewiredID == 1)
                    FixSplitPlayerCameraAndUI(__instance);
            }
        }

        public static void UpdateSplitPlayers()
        {
            if (SplitScreenManager.Instance.LocalPlayers.Count == 2)
                FixSplitPlayerCameraAndUI(SplitScreenManager.Instance.LocalPlayers[1]);
        }

        private static void FixSplitPlayerCameraAndUI(SplitPlayer splitPlayer)
        {
            var multiDisplay = SplitScreenManager.Instance.CurrentSplitType == SplitScreenManager.SplitType.MultiDisplay;

            // Fix camera's target display
            splitPlayer.m_camera.targetDisplay = multiDisplay ? 1 : 0;

            // CharacterUI is not in the right UI holder if SplitType is set to MultiDisplay before player 2 joined
            CharacterUI charUi = SplitScreenManager.Instance.GetCachedUI(1);
            charUi.gameObject.transform.SetParent(multiDisplay ? MenuManager.Instance.m_characterUIHolderP2 : MenuManager.Instance.m_characterUIHolder, worldPositionStays: false);
            charUi.gameObject.transform.ResetLocal();

            // 'GameplayP2' game object in scene has its 'GameDisplays (1)' active, but it should not
            MenuManager.Instance.gameObject.transform.Find("GameplayP2/GameDisplays (1)")?.gameObject.SetActive(false);

        }
    }
}
