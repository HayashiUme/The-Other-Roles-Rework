using System;
using System.Collections.Generic;
using AmongUs.Data;
using Assets.InnerNet;
using HarmonyLib;
using TheOtherRoles.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TheOtherRoles;
using System.IO;
using static UnityEngine.UI.Button;
using TheOtherRoles.Objects;
using static TheOtherRoles.DebugManager;



namespace TheOtherRoles.Modules
{

    [HarmonyPatch(typeof(MainMenuManager), "Start")]
    public class MainMenuPatch
    {
        private static bool horseButtonState = TORMapOptions.enableHorseMode;
        private static GameObject QQButton;
        public static MainMenuManager Instance { get; private set; }

        public static void addSceneChangeCallbacks()
        {
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
            {
                if (!scene.name.Equals("MatchMaking", StringComparison.Ordinal)) return;
                TORMapOptions.gameMode = CustomGamemodes.Classic;
                // Add buttons For Guesser Mode, Hide N Seek in this scene.
                // find "HostLocalGameButton"
                var template = GameObject.FindObjectOfType<HostLocalGameButton>();
                var gameButton = template.transform.FindChild("CreateGameButton");
                var gameButtonPassiveButton = gameButton.GetComponentInChildren<PassiveButton>();

                var guesserButton = GameObject.Instantiate(gameButton, gameButton.parent);
                guesserButton.transform.localPosition += new Vector3(0f, -0.5f);
                var guesserButtonText = guesserButton.GetComponentInChildren<TextMeshPro>();
                var guesserButtonPassiveButton = guesserButton.GetComponentInChildren<PassiveButton>();

                guesserButtonPassiveButton.OnClick = new ButtonClickedEvent();
                guesserButtonPassiveButton.OnClick.AddListener((Action)(() =>
                {
                    TORMapOptions.gameMode = CustomGamemodes.Guesser;
                    template.OnClick();
                }));

                var HideNSeekButton = GameObject.Instantiate(gameButton, gameButton.parent);
                HideNSeekButton.transform.localPosition += new Vector3(1.7f, -0.5f);
                var HideNSeekButtonText = HideNSeekButton.GetComponentInChildren<TextMeshPro>();
                var HideNSeekButtonPassiveButton = HideNSeekButton.GetComponentInChildren<PassiveButton>();

                HideNSeekButtonPassiveButton.OnClick = new ButtonClickedEvent();
                HideNSeekButtonPassiveButton.OnClick.AddListener((Action)(() =>
                {
                    TORMapOptions.gameMode = CustomGamemodes.HideNSeek;
                    template.OnClick();
                }));

                var PropHuntButton = GameObject.Instantiate(gameButton, gameButton.parent);
                PropHuntButton.transform.localPosition += new Vector3(3.4f, -0.5f);
                var PropHuntButtonText = PropHuntButton.GetComponentInChildren<TextMeshPro>();
                var PropHuntButtonPassiveButton = PropHuntButton.GetComponentInChildren<PassiveButton>();

                PropHuntButtonPassiveButton.OnClick = new ButtonClickedEvent();
                PropHuntButtonPassiveButton.OnClick.AddListener((Action)(() =>
                {
                    TORMapOptions.gameMode = CustomGamemodes.PropHunt;
                    template.OnClick();
                }));

                template.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(p =>
                {
                    guesserButtonText.SetText("guesserButtonText".Translate());
                    HideNSeekButtonText.SetText("HideNSeekButtonText".Translate());
                    PropHuntButtonText.SetText("PropHuntButtonText".Translate());
                })));
            }));
        }
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPostfix]
        public static void Start_Postfix(MainMenuManager __instance)
        {
            Instance = __instance;

            SimpleButton.SetBase(__instance.quitButton);


            int row = 1; int col = 0;
            GameObject CreatButton(string text, Action action)
            {
                col++; if (col > 2) { col = 1; row++; }
                var template = col == 1 ? __instance.creditsButton.gameObject : __instance.quitButton.gameObject;
                var button = Object.Instantiate(template, template.transform.parent);
                button.transform.transform.FindChild("FontPlacer").GetChild(0).gameObject.DestroyTranslator();
                var buttonText = button.transform.FindChild("FontPlacer").GetChild(0).GetComponent<TextMeshPro>();
                buttonText.text = text;
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new();
                passiveButton.OnClick.AddListener(action);
                AspectPosition aspectPosition = button.GetComponent<AspectPosition>();
                aspectPosition.anchorPoint = new Vector2(col == 1 ? 0.415f : 0.583f, 0.5f - 0.08f * row);
                SimpleButton.SetBase(passiveButton);
                return button;
            }

            if (QQButton == null) QQButton = CreatButton("QQ", () => Application.OpenURL("https://qm.qq.com/cgi-bin/qm/qr?authKey=Dn8MKDZAadw0VHyaPg43rRuSNIK9fOpzmI%2BfZA1%2F6%2BCx2QpqZH1vzHlB6QwVKv3Q&k=qDktOeGaUnZHnx0_U6kBoQ9d0ip8_Myp&noverify=0"));
            QQButton.gameObject.SetActive(true);
            QQButton.name = "QQ";
            PassiveButton passiveQQButton = QQButton.GetComponent<PassiveButton>();
            SpriteRenderer SpriteQQButton = QQButton.transform.FindChild("Inactive").GetComponent<SpriteRenderer>();
            Color QQColor = new Color(0.317f, 0, 1, 0.8f);
            SpriteQQButton.color = QQColor;
            passiveQQButton.OnMouseOut.AddListener((System.Action)delegate
            {
                SpriteQQButton.color = QQColor;
            });

        }
    }
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    internal class VersionShowerStartPatch
    {
        private static GameObject VersionShower1;
        public static GameObject OVersionShower;
        private static TextMeshPro VisitText;
        private static void Postfix(VersionShower __instance)
        {
            string credentialsText = "<color=#CCFFFF>XtremeWave</color> © 2025 - <color=#00ffff>ELinmei</color>";
            credentialsText += "\t\t\t";
            string versionText = $"<color=#8470FF>TORR</color></color> - {TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}";
            credentialsText += versionText;
            var friendCode = GameObject.Find("FriendCode");
            if (friendCode != null && VersionShower1 == null)
            {
                VersionShower1 = Object.Instantiate(friendCode, friendCode.transform.parent);
                VersionShower1.name = "TORR Version Shower";
                VersionShower1.transform.localPosition = friendCode.transform.localPosition + new Vector3(3.2f, 0f, 0f);
                VersionShower1.transform.localScale *= 1.7f;
                var TMP = VersionShower1.GetComponent<TextMeshPro>();
                TMP.alignment = TextAlignmentOptions.Right;
                TMP.fontSize = 30f;
                TMP.SetText(credentialsText);
            }
            if ((OVersionShower = GameObject.Find("VersionShower")) != null && VisitText == null)
            {
                VisitText = UnityEngine.Object.Instantiate(__instance.text);
                VisitText.name = "TORR User Counter";
                VisitText.alignment = TextAlignmentOptions.Left;
                VisitText.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                VisitText.transform.localPosition = new Vector3(-3.92f, -2.9f, 0f);
                VisitText.enabled = GameObject.Find("TOR Background") != null;

                __instance.text.text = $"<color=#C1FFC1>Among Us<color=#FF0000> The Other Roles <color=#8470FF>Rework</color></color></color> v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}{(Helpers.isSpecialDay(4, 1) ? (Helpers.IsChinese() ? "<color=#7CFC00>愚人节快乐!!</color>" : "<color=#7CFC00>Happy April Fool's Day!!</color>") : "")}";
                __instance.text.alignment = TextAlignmentOptions.Left;
                OVersionShower.transform.localPosition = new Vector3(-4.92f, -3.3f, 0f);

                var ap1 = OVersionShower.GetComponent<AspectPosition>();
                if (ap1 != null) UnityEngine.Object.Destroy(ap1);
                var ap2 = VisitText.GetComponent<AspectPosition>();
                if (ap2 != null) UnityEngine.Object.Destroy(ap2);
            }
            ;
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPriority(Priority.First)]
    internal class TitleLogoPatch
    {

        public static GameObject TOR_Background;
        public static GameObject Ambience;
        public static GameObject Starfield;
        public static GameObject LeftPanel;
        public static GameObject RightPanel;
        public static GameObject CloseRightButton;
        public static GameObject Tint;
        public static GameObject Sizer;
        public static GameObject AULogo;
        public static GameObject BottomButtonBounds;

        //public static Texture2D image;

        public static Vector3 RightPanelOp;

        private static void Postfix(MainMenuManager __instance)
        {

            GameObject.Find("BackgroundTexture")?.SetActive(false);

            TOR_Background = new GameObject("TOR Background");
            TOR_Background.transform.position = new Vector3(0, 0, 520f);
            var bgRenderer = TOR_Background.AddComponent<SpriteRenderer>();
            //System.Random rnd = new System.Random();
            //int rndnum = rnd.Next(1,MainMenuImage.imgnum);
            //bgRenderer.sprite = Helpers.LoadSpriteFromDisk(@$"{MainMenuImage.FolderPath()}/image_{rndnum}.jpg",160f);
            bgRenderer.sprite = Helpers.loadSpriteFromResources("paintingbyarcaea.jpg", 320f);
            // bgRenderer.sprite = Helpers.LoadSpriteFromDisk("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Among Us\\Among Us TORR\\ACG\\image_8.jpg", 320f);

            if (!(Ambience = GameObject.Find("Ambience"))) return;
            if (!(Starfield = Ambience.transform.FindChild("starfield").gameObject)) return;
            StarGen starGen = Starfield.GetComponent<StarGen>();
            starGen.SetDirection(new Vector2(0, -2));
            Starfield.transform.SetParent(TOR_Background.transform);
            UnityEngine.Object.Destroy(Ambience);

            if (!(LeftPanel = GameObject.Find("LeftPanel"))) return;
            LeftPanel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            static void ResetParent(GameObject obj) => obj.transform.SetParent(LeftPanel.transform.parent);
            LeftPanel.ForEachChild((Il2CppSystem.Action<GameObject>)ResetParent);
            LeftPanel.SetActive(false);

            Color shade = new(0f, 0f, 0f, 0f);
            var standardActiveSprite = __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().sprite;
            var minorActiveSprite = __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().sprite;
            Dictionary<List<PassiveButton>, (Sprite, Color, Color, Color, Color)> mainButtons = new()
            {
                {new List<PassiveButton>() {__instance.playButton, __instance.inventoryButton, __instance.shopButton},
                    (standardActiveSprite, new(0.65f, 0.03f, 0.79f, 0.8f), shade, Color.white, Color.white) },
                {new List<PassiveButton>() {__instance.newsButton, __instance.myAccountButton, __instance.settingsButton},
                    (minorActiveSprite, new(1.12f, 0.02f, 1.08f, 0.8f), shade, Color.white, Color.white) },
                {new List<PassiveButton>() {__instance.creditsButton, __instance.quitButton},
                    (minorActiveSprite, new(1.12f, 0.02f, 0.77f, 0.8f), shade, Color.white, Color.white) },
            };
            try
            {
                mainButtons.Keys.Flatten().DoIf(x => x != null, x => x.buttonText.color = Color.white);
            }
            catch { }

            void FormatButtonColor(PassiveButton button, Sprite borderType, Color inActiveColor, Color activeColor, Color inActiveTextColor, Color activeTextColor)
            {
                button.activeSprites.transform.FindChild("Shine")?.gameObject?.SetActive(false);
                button.inactiveSprites.transform.FindChild("Shine")?.gameObject?.SetActive(false);
                var activeRenderer = button.activeSprites.GetComponent<SpriteRenderer>();
                var inActiveRenderer = button.inactiveSprites.GetComponent<SpriteRenderer>();
                activeRenderer.sprite = minorActiveSprite;
                inActiveRenderer.sprite = minorActiveSprite;
                activeRenderer.color = activeColor.a == 0f ? new Color(inActiveColor.r, inActiveColor.g, inActiveColor.b, 1f) : activeColor;
                inActiveRenderer.color = inActiveColor;
                button.activeTextColor = activeTextColor;
                button.inactiveTextColor = inActiveTextColor;
            }

            foreach (var kvp in mainButtons)
                kvp.Key.Do(button => FormatButtonColor(button, kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3, kvp.Value.Item4, kvp.Value.Item5));

            GameObject.Find("Divider")?.SetActive(false);

            if (!(RightPanel = GameObject.Find("RightPanel"))) return;
            var rpap = RightPanel.GetComponent<AspectPosition>();
            if (rpap) UnityEngine.Object.Destroy(rpap);
            RightPanelOp = RightPanel.transform.localPosition;
            RightPanel.transform.localPosition = RightPanelOp + new Vector3(10f, 0f, 0f);
            RightPanel.GetComponent<SpriteRenderer>().color = new(0.38f, 0.04f, 1.01f, 1f);

            CloseRightButton = new GameObject("CloseRightPanelButton");
            CloseRightButton.transform.SetParent(RightPanel.transform);
            CloseRightButton.transform.localPosition = new Vector3(-4.78f, 1.3f, 1f);
            CloseRightButton.transform.localScale = new(1f, 1f, 1f);
            CloseRightButton.AddComponent<BoxCollider2D>().size = new(0.6f, 1.5f);
            var closeRightSpriteRenderer = CloseRightButton.AddComponent<SpriteRenderer>();
            closeRightSpriteRenderer.sprite = null;//我没这个的图片
            closeRightSpriteRenderer.color = new(0.38f, 0.04f, 1.01f, 1f);
            var closeRightPassiveButton = CloseRightButton.AddComponent<PassiveButton>();
            closeRightPassiveButton.OnClick = new();
            closeRightPassiveButton.OnClick.AddListener((System.Action)MainMenuManagerPatch.HideRightPanel);
            closeRightPassiveButton.OnMouseOut = new();
            closeRightPassiveButton.OnMouseOut.AddListener((System.Action)(() => closeRightSpriteRenderer.color = new(0.38f, 0.04f, 1.01f, 1f)));
            closeRightPassiveButton.OnMouseOver = new();
            closeRightPassiveButton.OnMouseOver.AddListener((System.Action)(() => closeRightSpriteRenderer.color = new(0.38f, 0.04f, 1.01f, 1f)));

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
            if (!(AULogo = GameObject.Find("LOGO-AU"))) return;
            Sizer.transform.localPosition += new Vector3(0f, 0.12f, 0f);
            AULogo.transform.localScale = new Vector3(0.66f, 0.67f, 1f);
            AULogo.transform.position += new Vector3(0f, 0.1f, 0f);
            var logoRenderer = AULogo.GetComponent<SpriteRenderer>();
            logoRenderer.sprite = Helpers.loadSpriteFromResources("Banner.png", 80f);

            if (!(BottomButtonBounds = GameObject.Find("BottomButtonBounds"))) return;
            BottomButtonBounds.transform.localPosition -= new Vector3(0f, 0.1f, 0f);
        }
    }
    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    internal class ModManagerLateUpdatePatch
    {
        public static void Prefix(ModManager __instance)
        {
            __instance.ShowModStamp();
        }
    }
    [HarmonyPatch(typeof(CreditsScreenPopUp))]
    internal class CreditsScreenPopUpPatch
    {
        [HarmonyPatch(nameof(CreditsScreenPopUp.OnEnable))]
        public static void Postfix(CreditsScreenPopUp __instance)
        {
            __instance.BackButton.transform.parent.FindChild("Background").gameObject.SetActive(false);
        }
    }
}