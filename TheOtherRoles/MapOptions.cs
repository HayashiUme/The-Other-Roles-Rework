using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.UIR;

namespace TheOtherRoles;

internal static class TORMapOptions
{
    // Set values
    public static int maxNumberOfMeetings = 10;
    public static bool blockSkippingInEmergencyMeetings;
    public static bool noVoteIsSelfVote;
    public static bool hidePlayerNames;
    public static bool ghostsSeeInformation = true;
    public static bool showRoleSummary = true;
    public static bool allowParallelMedBayScans;
    public static bool showLighterDarker = true;
    public static bool enableSoundEffects = true;
    public static bool shieldFirstKill;
    public static bool InsteadDarkMode;
    public static bool ShowChatNotifications = true;
    public static CustomGamemodes gameMode = CustomGamemodes.Classic;
    public static bool showFPS = true;

    // Updating values
    public static int meetingsCount;
    public static List<SurvCamera> camerasToAdd = new();
    public static List<Vent> ventsToSeal = new();
    public static Dictionary<byte, PoolablePlayer> playerIcons = new();
    public static string firstKillName;
    public static PlayerControl firstKillPlayer;

    public static void clearAndReloadMapOptions()
    {
        meetingsCount = 0;
        camerasToAdd = new List<SurvCamera>();
        ventsToSeal = new List<Vent>();
        playerIcons = new Dictionary<byte, PoolablePlayer>();
        ;

        maxNumberOfMeetings = Mathf.RoundToInt(CustomOptionHolder.maxNumberOfMeetings.getSelection());
        blockSkippingInEmergencyMeetings = CustomOptionHolder.blockSkippingInEmergencyMeetings.getBool();
        noVoteIsSelfVote = CustomOptionHolder.noVoteIsSelfVote.getBool();
        hidePlayerNames = CustomOptionHolder.hidePlayerNames.getBool();
        allowParallelMedBayScans = CustomOptionHolder.allowParallelMedBayScans.getBool();
        shieldFirstKill = CustomOptionHolder.shieldFirstKill.getBool();
        firstKillPlayer = null;
    }

    public static void reloadPluginOptions()
    {
        showRoleSummary = TheOtherRolesPlugin.ShowRoleSummary.Value;
        enableSoundEffects = TheOtherRolesPlugin.EnableSoundEffects.Value;
        ShowChatNotifications = TheOtherRolesPlugin.ShowChatNotifications.Value;
        showFPS = TheOtherRolesPlugin.ShowFPS.Value;

        //Patches.ShouldAlwaysHorseAround.isHorseMode = TheOtherRolesPlugin.EnableHorseMode.Value;
    }
}