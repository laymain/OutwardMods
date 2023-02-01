using System;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;

namespace CoopMultiDisplay.Patch
{
    public class GameDisplayInUIPatch
    {
        [HarmonyPatch(typeof(GameDisplayInUI), nameof(GameDisplayInUI.SetMultiDisplayActive), typeof(bool))]
        public class SetMultiDisplayActive
        {
            private static int _secondaryWindowHandle;

            private static bool Prefix(GameDisplayInUI __instance, bool _active)
            {
                if (_active)
                {
                    __instance.Screens[1].rectTransform.ResetRectTrans();
                    __instance.Screens[1].transform.SetParent(__instance.SecondaryDisplayCam.transform, worldPositionStays: true);
                    if (!__instance.SecondaryScreenActivated && Display.displays.Length > 1)
                    {
                        if (_secondaryWindowHandle == 0)
                        {
                            Display.displays[1].Activate();
                            _secondaryWindowHandle = FindWindow(null, "Unity Secondary Display").ToInt32();
                            CoopMultiDisplayMod.PublicLogger.LogInfo($"Found window handle: {_secondaryWindowHandle}");
                        }
                        else
                        {
                            ShowWindowAsync(_secondaryWindowHandle, SW_SHOW);
                        }
                        __instance.SecondaryScreenActivated = true;
                    }
                }
                else
                {
                    __instance.Screens[1].rectTransform.ResetRectTrans();
                    __instance.Screens[1].transform.SetParent(__instance.transform, worldPositionStays: true);
                    if (__instance.SecondaryScreenActivated)
                    {
                        ShowWindowAsync(_secondaryWindowHandle, SW_HIDE);
                        __instance.SecondaryScreenActivated = false;
                    }
                }
                __instance.SecondaryDisplayCanvas.gameObject.SetActive(_active);
                __instance.SecondaryDisplayCam.enabled = _active;
                MenuManager.Instance.SetMultiDisplayActive(_active);
                return false;
            }

            #region Win32 API

            private const int SW_HIDE = 0;
            private const int SW_SHOW = 5;

            [DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern int ShowWindowAsync(int hwnd, int nCmdShow);

            #endregion
        }
    }
}
