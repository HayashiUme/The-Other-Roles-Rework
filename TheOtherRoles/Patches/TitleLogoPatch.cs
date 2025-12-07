using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using TMPro;
using Object = UnityEngine.Object;
using Assets.InnerNet;
using AmongUs.GameOptions;
using static TheOtherRoles.Helpers;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPriority(Priority.First)]
internal class TitleLogoPatch
{
    public static GameObject Background;
    public static GameObject AULogo;
    public static GameObject BottomButtonBounds;
    public static GameObject Ambience;
    public static GameObject Starfield;
    public static GameObject RightPanel;
    public static GameObject Tint;
    public static GameObject Sizer;
    public static Vector3 RightPanelOp;

    private static void Postfix(MainMenuManager __instance)
    {
        Background = new GameObject("TOR Background");
        Background.transform.position = new Vector3(0, 0, 520f);
        var bgRenderer = Background.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = loadSpriteFromResources("MainMenuBackground.jpg", 150f);

        if (!(Ambience = GameObject.Find("Ambience"))) return;
        if (!(Starfield = Ambience.transform.FindChild("starfield").gameObject)) return;
        var starGen = Starfield.GetComponent<StarGen>();
        starGen.SetDirection(new Vector2(0, -2));
        Starfield.transform.SetParent(Background.transform);
        Object.Destroy(Ambience);

        if (!(AULogo = GameObject.Find("LOGO-AU"))) return;
        var logoRenderer = AULogo.GetComponent<SpriteRenderer>();
        logoRenderer.sprite = loadSpriteFromResources("Banner.png", 60f);
        AULogo.transform.localPosition += new Vector3(-0.4f, 0.15f, 0);
        AULogo.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        if (!(BottomButtonBounds = GameObject.Find("BottomButtonBounds"))) return;
        BottomButtonBounds.transform.localPosition += new Vector3(-0.3f, 0.6f, 0);
        __instance.playButton.transform.localPosition += new Vector3(-0.3f, 0.6f, 0);
        __instance.inventoryButton.transform.localPosition += new Vector3(-0.3f, 0.6f, 0);
        __instance.shopButton.transform.localPosition += new Vector3(-0.3f, 0.6f, 0);
        __instance.myAccountButton.transform.localPosition += new Vector3(-0.3f, 0.6f, 0);
        __instance.newsButton.transform.localPosition += new Vector3(-0.3f, 0.6f, 0);
        __instance.settingsButton.transform.localPosition += new Vector3(-0.3f, 0.6f, 0);

        GameObject.Find("Divider")?.SetActive(false);

        if (!(RightPanel = GameObject.Find("RightPanel"))) return;
        var rpap = RightPanel.GetComponent<AspectPosition>();
        if (rpap) UnityEngine.Object.Destroy(rpap);
        RightPanelOp = RightPanel.transform.localPosition;
        RightPanel.transform.localPosition = RightPanelOp + new Vector3(10f, 0f, 0f);
        RightPanel.GetComponent<SpriteRenderer>().color = new(0.38f, 0.04f, 1.01f, 1f);


        Tint = __instance.screenTint.gameObject;
        var ttap = Tint.GetComponent<AspectPosition>();
        if (ttap) UnityEngine.Object.Destroy(ttap);
        Tint.transform.SetParent(RightPanel.transform);
        Tint.transform.localPosition = new Vector3(-0.0824f, 0.0513f, Tint.transform.localPosition.z);
        Tint.transform.localScale = new Vector3(1f, 1f, 1f);
        __instance.howToPlayButton.gameObject.SetActive(true);
        __instance.howToPlayButton.transform.parent.Find("FreePlayButton").gameObject.SetActive(true);

        var creditsScreen = __instance.creditsScreen;
        if (creditsScreen)
        {
            var csto = creditsScreen.GetComponent<TransitionOpen>();
            if (csto) UnityEngine.Object.Destroy(csto);
            var closeButton = creditsScreen.transform.FindChild("CloseButton");
            closeButton?.gameObject.SetActive(false);
        }

        if (!(Sizer = GameObject.Find("Sizer"))) return;

        if (!(BottomButtonBounds = GameObject.Find("BottomButtonBounds"))) return;
        BottomButtonBounds.transform.localPosition -= new Vector3(0f, 0.1f, 0f);
    }

    public static Dictionary<string, Sprite> CachedSprites = new();
    

    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static void Postfix(VersionShower __instance)
    {
        __instance.text.alignment = TextAlignmentOptions.BottomLeft;
        __instance.text.fontSize = 1.85f;
        __instance.text.text = $"v{Application.version}- "+ $"<color=#FF0000>The Other Roles <color=#8470FF>Rework</color></color></size> v{TheOtherRolesPlugin.Version.ToString()} + {(TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}";
    }
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameStartManagePatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            var AspectSize = GameObject.Find("AspectSize");
            AspectSize.transform.FindChild("Divider").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            GameObject mapImage = AspectSize.FindChild<Transform>("MapImage").gameObject;
            mapImage.transform.localPosition = new Vector3(-16.8918f, -8.483f, -2f);
            GameObject sb = AspectSize.FindChild<Transform>("ModeLabel").gameObject;
            sb.transform.localPosition = new Vector3(1111f, -8.483f, -2f);
            GameObject sp = AspectSize.FindChild<Transform>("PrivacyLabel").gameObject;
            sp.transform.localPosition = new Vector3(1111f, -8.483f, -2f);
            GameObject sc = AspectSize.FindChild<Transform>("CapacityLabel").gameObject;
            sc.transform.localPosition = new Vector3(1111f, -8.483f, -2f);
            GameObject sd = AspectSize.FindChild<Transform>("Background").gameObject;
            sd.transform.localPosition = new Vector3(-1.0986f, -4.7321f, 0f);
            sd.transform.localScale = new Vector3(0.7009f, 0.6009f, 0f);
            sd.GetComponent<SpriteRenderer>().color = new(0f, 0f, 1f);

            if (__instance == null) return;

            TextMeshPro temp = __instance.PlayerCounter;


            if (AmongUsClient.Instance.AmHost)
            {
                __instance.EditButton.transform.localPosition = new Vector3(-0.4815f, -0.11f, -1f);
                __instance.EditButton.transform.localScale = new Vector3(1.24f, 0.8f, 0f);

                __instance.HostViewButton.transform.localPosition = new Vector3(-0.4815f, 0.52f, -1f);
                __instance.HostViewButton.transform.localScale = new Vector3(1.24f, 0.8f, 0f);
            }
            else
            {
                __instance.ClientViewButton.transform.localPosition = new Vector3(0.7823f, 0.5357f, 0f);
                __instance.ClientViewButton.transform.localScale = new Vector3(0.592f, 0.6f, 0f);
            }
        }
    }
}
