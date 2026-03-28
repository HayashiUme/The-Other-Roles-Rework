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
    public static bool ghostsSeeRoles = true;
    public static bool ghostsSeeModifier = true;
    public static bool ghostsSeeInformation = true;
    public static bool ghostsSeeVotes = true;
    public static bool showRoleSummary = true;
    public static bool allowParallelMedBayScans;
    public static bool showLighterDarker = true;
    public static bool enableSoundEffects = true;
    public static bool enableHorseMode;
    public static bool shieldFirstKill;
    public static bool ShowVentsOnMap = true;
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
        ghostsSeeRoles = TheOtherRolesPlugin.GhostsSeeRoles.Value;
        ghostsSeeModifier = TheOtherRolesPlugin.GhostsSeeModifier.Value;
        ghostsSeeInformation = TheOtherRolesPlugin.GhostsSeeInformation.Value;
        ghostsSeeVotes = TheOtherRolesPlugin.GhostsSeeVotes.Value;
        showRoleSummary = TheOtherRolesPlugin.ShowRoleSummary.Value;
        showLighterDarker = TheOtherRolesPlugin.ShowLighterDarker.Value;
        enableSoundEffects = TheOtherRolesPlugin.EnableSoundEffects.Value;
        enableHorseMode = TheOtherRolesPlugin.EnableHorseMode.Value;
        ShowVentsOnMap = TheOtherRolesPlugin.ShowVentsOnMap.Value;
        ShowChatNotifications = TheOtherRolesPlugin.ShowChatNotifications.Value;
        showFPS = TheOtherRolesPlugin.ShowFPS.Value;

        //Patches.ShouldAlwaysHorseAround.isHorseMode = TheOtherRolesPlugin.EnableHorseMode.Value;
    }
}