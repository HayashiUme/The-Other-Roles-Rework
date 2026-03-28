using System.Text.RegularExpressions;
using AmongUs.Data;
using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;
using static TheOtherRoles.Modules.ModTranslation;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
public sealed class LobbyJoinBind
{
    private static int GameId;
    public static Color CompeletedColor = new(185, 255, 181, 255);
    public const string CodeColor = "#8470FF";
    public static GameObject LobbyText;

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
    [HarmonyPostfix]
    public static void Postfix(InnerNetClient __instance)
    {
        GameId = __instance.GameId;
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    [HarmonyPostfix]
    public static void Postfix()
    {
        var code2 = GUIUtility.systemCopyBuffer;

        if (code2.Length != 6 || !Regex.IsMatch(code2, @"^[a-zA-Z]+$"))
            code2 = "";

        if (!LobbyText)
        {
            LobbyText = new GameObject("lobbycode");
            LobbyText.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            var comp = LobbyText.AddComponent<TextMeshPro>();
            comp.fontSize = 2.5f;
            comp.outlineWidth = -2f;
            float lastY = code2 == "" ? 0.25f : 0.1f;
            LobbyText.transform.localPosition = new Vector3(8f, lastY, 0);
            LobbyText.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    [HarmonyPostfix]
    public static void Postfix(MainMenuManager __instance)
    {
        var code2 = GUIUtility.systemCopyBuffer;

        if (code2.Length != 6 || !Regex.IsMatch(code2, @"^[a-zA-Z]+$"))
            code2 = "";
        var code2Disp = DataManager.Settings.Gameplay.StreamerMode ? new string('*', code2.Length) : code2.ToUpper();
        if (GameId != 0 && Input.GetKeyDown(KeyCode.LeftShift))
        {
            __instance.StartCoroutine(AmongUsClient.Instance.CoJoinOnlineGameFromCode(GameId));
            LobbyText.GetComponent<TextMeshPro>().color = CompeletedColor;
            LobbyText.GetComponent<TextMeshPro>().color = CompeletedColor.AlphaMultiplied(0.75f);
        }

        else if (Input.GetKeyDown(KeyCode.RightShift) && code2 != "")
        {
            __instance.StartCoroutine(AmongUsClient.Instance.CoJoinOnlineGameFromCode(GameCode.GameNameToInt(code2)));
            LobbyText.GetComponent<TextMeshPro>().color = CompeletedColor;
            LobbyText.GetComponent<TextMeshPro>().color = CompeletedColor.AlphaMultiplied(0.75f);
        }

        if (LobbyText)
        {
            LobbyText.GetComponent<TextMeshPro>().text = "";
            if (GameId != 0 && GameId != 32)
            {
                var code = GameCode.IntToGameName(GameId);
                if (code != "")
                {
                    code = DataManager.Settings.Gameplay.StreamerMode ? new string('*', code.Length) : code;
                    LobbyText.GetComponent<TextMeshPro>().text =
                        string.Format($"{GetString("lobbyTextPrevText")} ",$"<color={CodeColor}>{code}</color>");
                }
            }

            if (code2 != "")
                LobbyText.GetComponent<TextMeshPro>().text +=
                    string.Format($"\n{GetString("lobbyTextClipboardText")}",$"<color={CodeColor}>{code2Disp}</color>");
        }
    }
}