using System;
using BepInEx;
using BepInEx.Logging;
using CoopMultiDisplay.Patch;
using HarmonyLib;
using Rewired;
using UnityEngine;

namespace CoopMultiDisplay
{
    [BepInPlugin(Id, Name, Version)]
    public class CoopMultiDisplayMod : BaseUnityPlugin
    {
        private const string Id = "com.laymain.outward.mods.coopmultidisplay";
        private const string Name = "CoopMultiDisplay";
        private const string Version = "0.0.1";
        private const string Author = "Laymain";

        public static bool MultiDisplayEnabled => SplitScreenManager.Instance.CurrentSplitType == SplitScreenManager.SplitType.MultiDisplay;

        public static ManualLogSource PublicLogger;

        internal void Awake()
        {
            PublicLogger = Logger;
            try
            {
                new Harmony(Id).PatchAll();
                PublicLogger.LogInfo($"{Name} by {Author} (version {Version}) loaded.");
            }
            catch (Exception ex)
            {
                PublicLogger.LogError(ex);
            }
        }

        internal void Update()
        {
            if (ReInput.controllers.Keyboard.GetKey(KeyCode.Home) || ReInput.controllers.Keyboard.GetKey(KeyCode.BackQuote))
            {
                if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.M))
                    SetSplitType(SplitScreenManager.SplitType.MultiDisplay);
                else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.V))
                    SetSplitType(SplitScreenManager.SplitType.Vertical);
                else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.H))
                    SetSplitType(SplitScreenManager.SplitType.Horizontal);
            }
        }

        private static void SetSplitType(SplitScreenManager.SplitType splitType)
        {
            if (splitType == SplitScreenManager.SplitType.MultiDisplay && Display.displays.Length < 2)
            {
                PublicLogger.LogWarning("Cannot enable MultiDisplay with only one display");
                return;
            }
            if (SplitScreenManager.Instance.CurrentSplitType != splitType)
            {
                SplitScreenManager.Instance.CurrentSplitType = splitType;
                GameDisplayInUI.Instance.SetMultiDisplayActive(splitType == SplitScreenManager.SplitType.MultiDisplay);
                SplitPlayerPatch.UpdateSplitPlayers();
                SplitScreenManager.Instance.RefreshSplitScreen();
            }
        }
    }
}
