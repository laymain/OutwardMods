using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardMod_CoopUIScaler
{
    [BepInPlugin(Id, Name, Version)]
    public class CoopUIScalerMod : BaseUnityPlugin
    {
        private const string Id = "com.laymain.outward.mods.coopuiscaler";
        private const string Name = "CoopUIScaler";
        private const string Version = "0.0.5";
        private const string Author = "SirMuffin+Laymain";

        private static ManualLogSource _logger;
        private static Configuration _configuration;

        private class Configuration
        {
            public ConfigEntry<SplitScreenManager.SplitType> SplitType { get; }
            public ConfigEntry<bool> MoveGlobalUiToPlayer1 { get; }
            public ConfigEntry<float> ScaleFactor { get; }

            public Configuration(ConfigFile config)
            {
                SplitType = config.Bind("General", "SplitType", SplitScreenManager.SplitType.Vertical, "Split screen type");
                MoveGlobalUiToPlayer1 = config.Bind("General", "MoveGlobalUIToPlayer1", true, "Move global UI to player 1's screen");
                ScaleFactor = config.Bind("General", "ScaleFactor", 1.0f, new ConfigDescription("UI scale factor", new AcceptableValueRange<float>(0.5f, 2.0f)));
            }
        }

        internal void Awake()
        {
            _logger = Logger;
            _configuration = new Configuration(Config);
            new Harmony(Id).PatchAll();
            _logger.LogInfo($"{Name} by {Author} (version {Version}) loaded.");
        }

        [HarmonyPatch(typeof(MapDisplay), nameof(MapDisplay.Show), typeof(CharacterUI))]
        public class MapDisplay_Show_CharacterUI
        {
            private static bool _initialized = false;
            private static Vector2 _mapOrigAnchoredPosition;
            private static Vector2 _mapOrigSizeDelta;

            private static void Postfix(MapDisplay __instance, CharacterUI _owner)
            {
                if (_initialized)
                {
                    _mapOrigAnchoredPosition = __instance.RectTransform.anchoredPosition;
                    _mapOrigSizeDelta =__instance.RectTransform.sizeDelta;
                    _initialized = true;
                }

                if (_configuration.MoveGlobalUiToPlayer1.Value)
                {
                    __instance.RectTransform.anchoredPosition = _owner.m_rectTransform.anchoredPosition;
                    __instance.RectTransform.sizeDelta = _owner.m_rectTransform.sizeDelta;
                }
                else
                {
                    __instance.RectTransform.anchoredPosition = _mapOrigAnchoredPosition;
                    __instance.RectTransform.sizeDelta = _mapOrigSizeDelta;
                }
            }
        }

        [HarmonyPatch(typeof(OptionsPanel), nameof(OptionsPanel.StartInit))]
        public class OptionsPanel_StartInit
        {
            private static void Postfix(OptionsPanel __instance)
            {
                __instance.m_sldFoVSplit.maxValue = 90f;
            }
        }

        [HarmonyPatch(typeof(SplitScreenManager), nameof(SplitScreenManager.Update))]
        public class SplitScreenManager_Update
        {
            private static int _lastScreenHeight;
            private static int _lastScreenWidth;

            private static void Prefix(SplitScreenManager __instance)
            {
                if (__instance.CurrentSplitType != _configuration.SplitType.Value)
                {
                    __instance.CurrentSplitType = _configuration.SplitType.Value;
                    __instance.ForceRefreshRatio = true;
                }
                if (Screen.height != _lastScreenHeight)
                {
                    _lastScreenHeight = Screen.height;
                    __instance.ForceRefreshRatio = true;
                }
                if (Screen.width != _lastScreenWidth)
                {
                    _lastScreenWidth = Screen.width;
                    __instance.ForceRefreshRatio = true;
                }
            }
        }

        [HarmonyPatch(typeof(SplitScreenManager), nameof(SplitScreenManager.DelayedRefreshSplitScreen))]
        public class SplitScreenManager_DelayedRefreshSplitScreen
        {
            private static void Postfix(SplitScreenManager __instance)
            {
                switch (__instance.CurrentSplitType)
                {
                    case SplitScreenManager.SplitType.Horizontal:
                        Horizontal(__instance);
                        break;
                    case SplitScreenManager.SplitType.Vertical:
                        Vertical(__instance);
                        break;
                }
                foreach (CanvasScaler scaler in MenuManager.Instance.GetComponentsInChildren<CanvasScaler>())
                {
                    scaler.matchWidthOrHeight = Screen.height > Screen.width ? 0f : 1f;
                }
                foreach (Canvas canvas in MenuManager.Instance.GetComponentsInChildren<Canvas>())
                {
                    canvas.scaleFactor = _configuration.ScaleFactor.Value;
                }
            }

            private static void Horizontal(SplitScreenManager __instance)
            {
                if (_configuration.MoveGlobalUiToPlayer1.Value)
                {
                    Vector2 zero = Vector2.zero;
                    Vector2 zero2 = Vector2.zero;
                    if (__instance.LocalPlayers.Count == 2)
                    {
                        zero2.y = -0.5f;
                        zero.y = -0.5f;
                    }

                    Vector2 vector = Vector2.Scale(zero2, MenuManager.Instance.ScreenSize);
                    Vector2 anchoredPosition = Vector2.Scale(zero, vector);
                    if (MenuManager.Instance.m_masterLoading != null)
                    {
                        var transform = MenuManager.Instance.m_masterLoading.GetComponentInChildren<RectTransform>();
                        if (transform != null)
                        {
                            transform.sizeDelta = vector;
                            transform.anchoredPosition = anchoredPosition;
                        }
                    }

                    if (MenuManager.Instance.m_prologueScreen != null)
                    {
                        RectTransform rectTransform = MenuManager.Instance.m_prologueScreen.RectTransform;
                        rectTransform.sizeDelta = vector;
                        rectTransform.anchoredPosition = anchoredPosition;
                    }
                }
            }

            private static void Vertical(SplitScreenManager __instance)
            {
                if (GameDisplayInUI.Instance.gameObject.activeSelf != __instance.RenderInImage)
                {
                    GameDisplayInUI.Instance.gameObject.SetActive(__instance.RenderInImage);
                }

                for (var i = 0; i < __instance.m_localCharacterUIs.Count; i++)
                {
                    SplitPlayer splitPlayer = __instance.m_localCharacterUIs.Values[i];
                    Vector3 default_OFFSET = CharacterCamera.DEFAULT_OFFSET;
                    Vector2 zero = Vector2.zero;
                    Vector2 zero2 = Vector2.zero;
                    var splitRect = new Rect(0f, 0f, 0f, 0f);
                    RawImage rawImage = !__instance.RenderInImage ? null : GameDisplayInUI.Instance.Screens[i];
                    float foV;
                    if (__instance.m_localCharacterUIs.Count == 1)
                    {
                        splitRect.position = Vector2.zero;
                        splitRect.size = Vector2.one;
                        foV = OptionManager.Instance.GetFoVSolo(i);
                        if (__instance.RenderInImage)
                        {
                            rawImage.rectTransform.localScale = Vector3.one;
                            GameDisplayInUI.Instance.Screens[1].gameObject.SetActive(false);
                        }

                        GameDisplayInUI.Instance.SetMultiDisplayActive(false);
                    }
                    else
                    {
                        if (__instance.m_localCharacterUIs.Count != 2)
                            throw new NotImplementedException("Support for more than 2 players is not implemented.");

                        var num = i + 1;
                        if (__instance.RenderInImage)
                        {
                            splitRect.position = i != 0 ? new Vector2(0.5f, 0f) : Vector2.zero;
                            splitRect.size = new Vector2(0.5f, 1f);
                        }
                        else
                        {
                            splitRect.position = new Vector2(0.5f * (i != 0 ? 1 : -1), 0f);
                            splitRect.size = Vector2.one;
                        }

                        foV = OptionManager.Instance.GetFoVSplit(i);
                        default_OFFSET.z = -2.5f;
                        zero2.x = -0.5f;
                        zero.x = (num % 2 != 1 ? -1 : 1) * 0.5f;
                        if (__instance.RenderInImage) GameDisplayInUI.Instance.Screens[1].gameObject.SetActive(true);
                    }

                    CameraSettings cameraSettings;
                    cameraSettings.FoV = foV;
                    cameraSettings.SplitRect = splitRect;
                    cameraSettings.Offset = default_OFFSET;
                    cameraSettings.CameraDepth = 2 * i;
                    cameraSettings.Image = rawImage;
                    splitPlayer.RefreshSplitScreen(zero, zero2, cameraSettings);
                }

                if (_configuration.MoveGlobalUiToPlayer1.Value)
                {
                    Vector2 zero3 = Vector2.zero;
                    Vector2 zero4 = Vector2.zero;
                    if (__instance.LocalPlayers.Count == 2)
                    {
                        zero4.x = -0.5f;
                        zero3.x = 0.5f;
                    }
                    Vector2 vector = Vector2.Scale(zero4, MenuManager.Instance.ScreenSize);
                    Vector2 anchoredPosition = Vector2.Scale(zero3, vector);
                    if (MenuManager.Instance.m_masterLoading != null)
                    {
                        var componentInChildren = MenuManager.Instance.m_masterLoading.GetComponentInChildren<RectTransform>();
                        if (componentInChildren != null)
                        {
                            componentInChildren.sizeDelta = vector;
                            componentInChildren.anchoredPosition = anchoredPosition;
                        }
                    }
                    if (MenuManager.Instance.m_prologueScreen != null)
                    {
                        RectTransform rectTransform = MenuManager.Instance.m_prologueScreen.RectTransform;
                        rectTransform.sizeDelta = vector;
                        rectTransform.anchoredPosition = anchoredPosition;
                    }
                }
            }
        }
    }
}
