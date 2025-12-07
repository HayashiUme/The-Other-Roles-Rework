using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Modules;

using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch
    {
        public static string ModName = $"<size=130%><color=#FF0000>The Other Roles <color=#8470FF>Rework</color></color></size> v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}";
        public static string FangKuai = "<color=#00FFFF>FangKuai</color>";
        public static string TOR = "<color=#FCCE03FF>TheOtherRolesAU</color>";
        public static string ELinmei = "<color=#00FFFF>ELinmei</color>";

        private static float deltaTime;
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            static void Postfix(PingTracker __instance)
            {
                var ping = AmongUsClient.Instance.Ping;
                string PingColor = "#ff4500";
                if (ping < 50) PingColor = "#44dfcc";
                else if (ping < 100) PingColor = "#7bc690";
                else if (ping < 200) PingColor = "#f3920e";
                else if (ping < 400) PingColor = "#ff146e";

                deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
                float fps = Mathf.Ceil(1.0f / deltaTime);

                __instance.text.alignment = AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ? TextAlignmentOptions.Top : TextAlignmentOptions.TopLeft;
                var position = __instance.GetComponent<AspectPosition>();
                position.Alignment = AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ? AspectPosition.EdgeAlignments.Top : AspectPosition.EdgeAlignments.LeftTop;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    string gameModeText = $"";
                    if (HideNSeek.isHideNSeekGM) gameModeText = ModTranslation.GetString("isHideNSeekGM");
                    else if (HandleGuesser.isGuesserGm) gameModeText = ModTranslation.GetString("isGuesserGm");
                    else if (PropHunt.isPropHuntGM) gameModeText = ModTranslation.GetString("isPropHuntGM");
                    if (gameModeText != "") gameModeText = Helpers.cs(Color.yellow, gameModeText) + "\n";
                    __instance.text.text = $"<size=130%><color=#FF0000> The Other Roles <color=#8470FF>Rework</color></color></size> v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "") + "\n" + $"{gameModeText}" + $"<color={PingColor}>PING: <b>{AmongUsClient.Instance.Ping}</b> MS</color>" + $"  {(TORMapOptions.showFPS ? $"  <color=#00a4ff>FPS: {fps}</color>" : "")}{(Helpers.isSpecialDay(4,1) ? (Helpers.IsChinese() ? "\n<color=#7CFC00>愚人节快乐!!</color>" : "\n<color=#7CFC00>Happy April Fool's Day!!</color>") : "")}"}";

                    position.DistanceFromEdge = new Vector3(1.5f, 0.11f, 0);
                }
                else
                {
                    string gameModeText = $"";
                    if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek) gameModeText = ModTranslation.GetString("isHideNSeekGM");
                    else if (TORMapOptions.gameMode == CustomGamemodes.Guesser) gameModeText = ModTranslation.GetString("isGuesserGm");
                    else if (TORMapOptions.gameMode == CustomGamemodes.PropHunt) gameModeText = ModTranslation.GetString("isPropHuntGM");
                    if (gameModeText != "") gameModeText = Helpers.cs(Color.yellow, gameModeText);


                    __instance.text.text = $"{ModName}<br><size=70%>{ModTranslation.GetString("moder")} {ELinmei}<br>{ModTranslation.GetString("baseOnModer")} {TOR}<br>{ModTranslation.GetString("modTranslators")}{ELinmei} & {FangKuai}<br></size>" + $"<color={PingColor}>PING: <b>{AmongUsClient.Instance.Ping}</b> MS</color>" + $"  {(TORMapOptions.showFPS ? $"  <color=#00a4ff>FPS: {fps}</color>" : "")}";
                    position.DistanceFromEdge = new Vector3(0.5f, 0.11f);

                    try
                    {
                        var GameModeText = GameObject.Find("GameModeText")?.GetComponent<TextMeshPro>();
                        GameModeText.text = gameModeText == "" ? (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek ? "Hide&Seek" : "Classic") : gameModeText;
                        var ModeLabel = GameObject.Find("ModeLabel")?.GetComponentInChildren<TextMeshPro>();
                    }
                    catch { }
                }
                position.AdjustPosition();
            }
        }
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class LogoPatch
        {
            public static SpriteRenderer renderer;
            public static Sprite bannerSprite;
            private static PingTracker instance;
            public static GameObject motdObject;
            public static TextMeshPro motdText;
            static void Postfix(PingTracker __instance)
            {
                var torLogo = new GameObject("bannerLogo_TOR");
                motdObject = new GameObject("torMOTD");
                motdText = motdObject.AddComponent<TextMeshPro>();
                motdText.alignment = TMPro.TextAlignmentOptions.Center;
                motdText.fontSize *= 0.045f;

                motdText.transform.SetParent(torLogo.transform);
                motdText.enableWordWrapping = true;
                var rect = motdText.gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(5.2f, 0.25f);

                motdText.transform.localPosition = Vector3.down * 2.7f;
                motdText.color = new Color(1, 53f / 255, 31f / 255);
                Material mat = motdText.fontSharedMaterial;
                mat.shaderKeywords = new string[] { "OUTLINE_ON" };
                motdText.SetOutlineColor(Color.white);
                motdText.SetOutlineThickness(0.095f);
            }

            public static void updateSprite()
            {
                //      loadSprites();
                if (renderer != null)
                {
                    float fadeDuration = 1f;
                    instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                    {
                        renderer.color = new Color(1, 1, 1, 1 - p);
                        if (p == 1)
                        {
                            renderer.sprite = bannerSprite;
                            instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                            {
                                renderer.color = new Color(1, 1, 1, p);
                            })));
                        }
                    })));
                }
            }
        }
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
        public static class MOTD
        {
            public static List<string> motds = new List<string>();
            private static float timer = 0f;
            private static float maxTimer = 5f;
            private static int currentIndex = 0;

            public static void Postfix()
            {
                if (motds.Count == 0)
                {
                    timer = maxTimer;
                    return;
                }
                if (motds.Count > currentIndex && LogoPatch.motdText != null)
                    LogoPatch.motdText.SetText(motds[currentIndex]);
                else return;

                // fade in and out:
                float alpha = Mathf.Clamp01(Mathf.Min(new float[] { timer, maxTimer - timer }));
                if (motds.Count == 1) alpha = 1;
                LogoPatch.motdText.color = LogoPatch.motdText.color.SetAlpha(alpha);
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = maxTimer;
                    currentIndex = (currentIndex + 1) % motds.Count;
                }
            }

            public static async Task loadMOTDs()
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(!Helpers.IsChinese() ?"http://api.fangkuai.fun:22022/TheOtherRolesAU/MOTD/main/motd.txt" : "https://raw.githubusercontent.com/linmeideli/TheOtherRolesReworked-MOTD/main/motd.txt");
                response.EnsureSuccessStatusCode();
                string motds = await response.Content.ReadAsStringAsync();
                foreach (string line in motds.Split("\n", StringSplitOptions.RemoveEmptyEntries))
                {
                    MOTD.motds.Add(line);
                }
            }

        }
    }
}
