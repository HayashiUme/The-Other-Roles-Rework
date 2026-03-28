using HarmonyLib;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch()]
public static class DarkModePatches
{
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetCosmetics))]
    private static class ChatBubblePatch
    {
        private static void Postfix(ChatBubble __instance)
        {
            var roleinfo = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
            Color color = new Color();
            foreach (RoleInfo id in roleinfo)
            {
                color = RoleInfo.roleInfoById[id.roleId].color;
            }
            __instance.Background.color = new UnityEngine.Color(0.2f, 0.2f, 0.2f);
            __instance.MaskArea.color = new UnityEngine.Color(0.1f, 0.1f, 0.1f);
            __instance.TextArea.color = new UnityEngine.Color(1, 1, 1);
            __instance.TextArea.outlineWidth = __instance.NameText.outlineWidth * 0.75f;
            __instance.Background.color = new Color(color.r, color.g, color.b, 0.5f);
            __instance.MaskArea.color = color;
            __instance.TextArea.color = Helpers.IsDarkColor(color) ? Color.black : Color.white;
            __instance.TextArea.outlineWidth = __instance.NameText.outlineWidth * 0.75f;
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    private static class ChatControllerPatch
    {
        private static void Postfix(ChatController __instance)
        {
            __instance.backgroundImage.color = new UnityEngine.Color(0.2f, 0.2f, 0.2f);

            __instance.chatButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.chatButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.chatButton.selectedSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.chatButton.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.quickChatButton.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            //__instance.banButton.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            //__instance.openKeyboardButton.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            __instance.quickChatButton.OnDestroy();
        }
    }

    [HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.Awake))]
    private static class FreeChatInputField_Awake
    {
        private static void Postfix(FreeChatInputField __instance)
        {
            __instance.background.color = new UnityEngine.Color(0.2f, 0.2f, 0.2f);

            var comp = __instance.background.GetComponent<ButtonRolloverHandler>();
            comp.OutColor = new UnityEngine.Color(0.15f, 0.15f, 0.15f);
            comp.OverColor = new UnityEngine.Color(0.25f, 0.25f, 0.25f);
            comp.UnselectedColor = new UnityEngine.Color(0.15f, 0.15f, 0.15f);
            __instance.textArea.gameObject.GetComponent<TextMeshPro>().color = UnityEngine.Color.white;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    private static class MeetingHud_Start
    {
        private static void Postfix(MeetingHud __instance)
        {
            __instance.meetingContents.transform.FindChild("PhoneUI").FindChild("baseColor").GetComponent<SpriteRenderer>().color = new Color(0.01f, 0.01f, 0.01f);
            __instance.Glass.color = new Color(0.7f, 0.7f, 0.7f, 0.3f);
            __instance.SkipVoteButton.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    private static class HudManager_Update
    {
        private static void Postfix(HudManager __instance)
        {
            __instance.MapButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.MapButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.MapButton.selectedSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.MapButton.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

            __instance.SettingsButton.GetComponent<PassiveButton>().inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.SettingsButton.GetComponent<PassiveButton>().activeSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.SettingsButton.GetComponent<PassiveButton>().selectedSprites.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            __instance.SettingsButton.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    [HarmonyPatch(typeof(ProgressTracker), nameof(ProgressTracker.FixedUpdate))]
    private static class ProgressTracker_FixedUpdate
    {
        private static void Postfix(ProgressTracker __instance)
        {

            __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
        }
    }

    [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.Awake))]
    private static class ChatNotification_Awake
    {
        private static void Postfix(ChatNotification __instance)
        {
            __instance.background.color = new Color(0.2f, 0.2f, 0.2f);
            __instance.chatText.color = Color.white;
        }
    }
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    private static class ChatController_Update
    {
        private static void Postfix(ChatController __instance)
        {
            var BanButton = __instance.transform.GetChild(7);
            BanButton.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);

            var QuickChatButton = __instance.transform.GetChild(5);
            QuickChatButton.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);

            var OpenKeyBoardButton = __instance.transform.GetChild(6);
            OpenKeyBoardButton.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
        }
    }
}