using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
using AmongUs.Data;
using Assets.InnerNet;
using BepInEx.Unity.IL2CPP;
using System.Linq;
using TheOtherRoles;
using TheOtherRoles.Modules;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class MainMenuPatch
    {
        private static void Prefix(MainMenuManager __instance)
        {
            SoundEffectsManager.Load();
            var template = GameObject.Find("ExitGameButton");
            var buttonQQ = UnityEngine.Object.Instantiate(template, template.transform.parent);
            var buttonGH = UnityEngine.Object.Instantiate(template, template.transform.parent);
            buttonQQ.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.586f, 0.43f);
            buttonGH.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.586f, 0.36f);

            var textGH = buttonGH.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new System.Action<float>((p) => {
                textGH.SetText("Github");
            })));
            PassiveButton passiveButtonGH = buttonGH.GetComponent<PassiveButton>();
            passiveButtonGH.activeTextColor = new Color32(0, 191, 255, byte.MaxValue);
            passiveButtonGH.OnClick = new Button.ButtonClickedEvent();
            passiveButtonGH.OnClick.AddListener((System.Action)(() => Application.OpenURL("https://github.com/linmeideli/The-Other-Roles-Rework")));
            passiveButtonGH.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(0f, 0.191f, 255f);
            passiveButtonGH.activeSprites.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 255f);
            Color originalColorpassiveButtonGH = passiveButtonGH.inactiveSprites.GetComponent<SpriteRenderer>().color;
            passiveButtonGH.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorpassiveButtonGH * 0.6f;

            //QQ button
            var textQQ = buttonQQ.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new System.Action<float>((p) => {
                textQQ.SetText("QQ");
            })));
            PassiveButton passiveButtonQQ = buttonQQ.GetComponent<PassiveButton>();
            passiveButtonQQ.activeTextColor = new Color32(0, 191, 255, byte.MaxValue);
            passiveButtonQQ.OnClick = new Button.ButtonClickedEvent();
            passiveButtonQQ.OnClick.AddListener((System.Action)(() => Application.OpenURL("https://qm.qq.com/cgi-bin/qm/qr?authKey=Dn8MKDZAadw0VHyaPg43rRuSNIK9fOpzmI%2BfZA1%2F6%2BCx2QpqZH1vzHlB6QwVKv3Q&k=qDktOeGaUnZHnx0_U6kBoQ9d0ip8_Myp&noverify=0")));
            passiveButtonQQ.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(0f, 0.191f, 255f);
            passiveButtonQQ.activeSprites.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 255f);
            Color originalColorpassiveButtonQQ = passiveButtonQQ.inactiveSprites.GetComponent<SpriteRenderer>().color;
            passiveButtonQQ.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorpassiveButtonQQ * 0.6f;
            buttonQQ.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.586f, 0.43f);
        }
        private static void CheckAndUnpatch()
        {
            // 获取所有已加载的插件
            var loadedPlugins = IL2CPPChainloader.Instance.Plugins.Values;
            // 检查是否有目标插件
            var targetPlugin = loadedPlugins.FirstOrDefault(plugin => plugin.Metadata.Name == "MalumMenu");

            if (targetPlugin != null)
            {
                TheOtherRolesPlugin.Logger.LogMessage("已检测到您使用了MM外挂.\n TORR将不再运行\n删除MalumMenu.dll即可恢复");
                Harmony.UnpatchAll();//当进入MainMenu时检测加载如果有MM 就自动关闭
            }
        }

        public static void addSceneChangeCallbacks()
        {
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) => {
                if (!scene.name.Equals("MatchMaking", StringComparison.Ordinal)) return;
                TORMapOptions.gameMode = CustomGamemodes.Classic;
                // Add buttons For Guesser Mode, Hide N Seek in this scene.
                // find "HostLocalGameButton"
                var template = GameObject.FindObjectOfType<HostLocalGameButton>();
                var gameButton = template.transform.FindChild("CreateGameButton");
                var gameButtonPassiveButton = gameButton.GetComponentInChildren<PassiveButton>();

                var guesserButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                guesserButton.transform.localPosition += new Vector3(0f, -0.5f);
                var guesserButtonText = guesserButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var guesserButtonPassiveButton = guesserButton.GetComponentInChildren<PassiveButton>();

                guesserButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                guesserButtonPassiveButton.OnClick.AddListener((System.Action)(() => {
                    TORMapOptions.gameMode = CustomGamemodes.Guesser;
                    template.OnClick();
                }));

                var HideNSeekButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                HideNSeekButton.transform.localPosition += new Vector3(1.7f, -0.5f);
                var HideNSeekButtonText = HideNSeekButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var HideNSeekButtonPassiveButton = HideNSeekButton.GetComponentInChildren<PassiveButton>();

                HideNSeekButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                HideNSeekButtonPassiveButton.OnClick.AddListener((System.Action)(() => {
                    TORMapOptions.gameMode = CustomGamemodes.HideNSeek;
                    template.OnClick();
                }));

                var PropHuntButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                PropHuntButton.transform.localPosition += new Vector3(3.4f, -0.5f);
                var PropHuntButtonText = PropHuntButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var PropHuntButtonPassiveButton = PropHuntButton.GetComponentInChildren<PassiveButton>();

                PropHuntButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                PropHuntButtonPassiveButton.OnClick.AddListener((System.Action)(() => {
                    TORMapOptions.gameMode = CustomGamemodes.PropHunt;
                    template.OnClick();
                }));

                template.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) => {
                    guesserButtonText.SetText("guesserButtonText".Translate());
                    HideNSeekButtonText.SetText("HideNSeekButtonText".Translate());
                    PropHuntButtonText.SetText("PropHuntButtonText".Translate());
                })));
            }));
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
