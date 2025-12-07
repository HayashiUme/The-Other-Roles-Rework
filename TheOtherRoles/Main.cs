global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
global using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using AmongUs.Data.Player;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Security.Cryptography;
using Il2CppSystem.Text;
using InnerNet;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using TheOtherRoles.Modules;
using TheOtherRoles.Modules.CustomHats;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = System.Random;
using UnityEngine.UIElements.UIR;
using Rewired;

namespace TheOtherRoles;

[BepInPlugin(Id, "The Other Roles Rework", VersionString)]
[BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInProcess("Among Us.exe")]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public class TheOtherRolesPlugin : BasePlugin
{
    public const string Id = "xtreme.wave.elinmei.torr";
    //public const string Id = "xtremewave.elinmei.theotherrolesreworked";
    public const string VersionString = "2.0.0";
    public static uint betaDays = 0; // amount of days for the build to be usable (0 for infinite!)

    public static Version Version = Version.Parse(VersionString);
    internal static ManualLogSource Logger;
    public static TheOtherRolesPlugin Instance;

    public static int optionsPage = 2;

    public static Sprite ModStamp;

    public static IRegionInfo[] defaultRegions;

    public Harmony Harmony { get; } = new(Id);

    public static ConfigEntry<string> DebugMode { get; private set; }
    public static ConfigEntry<bool> GhostsSeeInformation { get; set; }
    public static ConfigEntry<bool> ShowRoleSummary { get; set; }
    public static ConfigEntry<bool> InsteadDarkMode { get; set; }
    public static ConfigEntry<bool> EnableSoundEffects { get; set; }
    public static ConfigEntry<bool> ShowChatNotifications { get; set; }
    public static ConfigEntry<string> Ip { get; set; }
    public static ConfigEntry<ushort> Port { get; set; }
    public static ConfigEntry<string> ShowPopUpVersion { get; set; }
    public static ConfigEntry<bool> ShowFPS { get; set; }

    // This is part of the Mini.RegionInstaller, Licensed under GPLv3
    // file="RegionInstallPlugin.cs" company="miniduikboot">
    public static void UpdateRegions()
    {
        ServerManager serverManager = FastDestroyableSingleton<ServerManager>.Instance;
        var regions = new IRegionInfo[]
        {
                new StaticHttpRegionInfo("<color=#76BAF6>Niko233 Server [China,Asis]</color>", StringNames.NoTranslation, "https://aucn2.niko233.me", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Niko233(AS_CN)", "https://aucn2.niko233.me", 443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("<color=#D2A2EE>Niko233 Server [USA,NorthAmerica]</color>", StringNames.NoTranslation, "https://au-us2.niko233.me", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Niko233(NA_US2)", "https://au-us2.niko233.me", 443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("<color=#49F0FC>TORR Server</color> <color=#8732FF>[SuQian,China]</color>", StringNames.NoTranslation, "https://newplayer.fangkuai.fun", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("<color=#49F0FC>方块服</color> <color=#8732FF>[宿迁]</color>", "http://sq.fangkuai.fun", 22020, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("<color=#49F0FC>FangKuai Server</color> <color=#00bfff>[HongKong,China]</color>", StringNames.NoTranslation, "https://newauhk.fangkuai.fun", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("方块服 [宿迁]", "https://player.fangkuai.fun", 443, false) })).CastFast<IRegionInfo>(),
        };
        IRegionInfo currentRegion = serverManager.CurrentRegion;
        Logger.LogInfo($"Adding {regions.Length} regions");
        foreach (IRegionInfo region in regions)
        {
            if (region == null)
                Logger.LogError("Could not add region");
            else
            {
                if (currentRegion != null && region.Name.Equals(currentRegion.Name, StringComparison.OrdinalIgnoreCase))
                    currentRegion = region;
                serverManager.AddOrUpdateRegion(region);
            }
        }

        // AU remembers the previous region that was set, so we need to restore it
        if (currentRegion != null)
        {
            Logger.LogDebug("Resetting previous region");
            serverManager.SetRegion(currentRegion);
        }
    }
    public override void Load()
    {
        Logger = Log;
        Instance = this;
        ModTranslation.Load();

        _ = Helpers.checkBeta(); // Exit if running an expired beta
        _ = CredentialsPatch.MOTD.loadMOTDs();

        DebugMode = Config.Bind("Custom", "Enable Debug Mode", "false");
        GhostsSeeInformation = Config.Bind("Custom", "Ghosts See Remaining Tasks", true);
        ShowRoleSummary = Config.Bind("Custom", "Show Role Summary", true);
        InsteadDarkMode = Config.Bind("Custom", "Instead Dark Mod Of Role Color", false);
        EnableSoundEffects = Config.Bind("Custom", "Enable Sound Effects", true);
        ShowPopUpVersion = Config.Bind("Custom", "Show PopUp", "0");
        ShowChatNotifications = Config.Bind("Custom", "Show Chat Notifications", true);
        ShowFPS = Config.Bind("Custom", "Show FPS", true);

        Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
        Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
        defaultRegions = ServerManager.DefaultRegions;
        // Removes vanilla Servers
        ServerManager.DefaultRegions = new Il2CppReferenceArray<IRegionInfo>(new IRegionInfo[0]);
        UpdateRegions();

        // Reactor Credits (future use?)
        // Reactor.Utilities.ReactorCredits.Register("TheOtherRoles", VersionString, betaDays > 0, location => location == Reactor.Utilities.ReactorCredits.Location.PingTracker);

        DebugMode = Config.Bind("Custom", "Enable Debug Mode", "false");
        Harmony.PatchAll();

        CustomOptionHolder.Load();
        CustomColors.Load();
        CustomHatManager.LoadHats();
        if (BepInExUpdater.UpdateRequired)
        {
            AddComponent<BepInExUpdater>();
            return;
        }

        AddComponent<ModUpdater>();

        EventUtility.Load();
        SubmergedCompatibility.Initialize();
        MainMenuPatch.addSceneChangeCallbacks();
        _ = RoleInfo.loadReadme();
        AddToKillDistanceSetting.addKillDistance();
        Logger.LogInfo("Loading TORR completed!");
    }
}

// Deactivate bans, since I always leave my local testing game and ban myself
[HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
public static class IsBannedPatch
{
    public static void Postfix(out bool __result)
    {
        __result = false;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
public static class ChatControllerAwakePatch
{
    private static void Prefix()
    {
        if (!EOSManager.Instance.isKWSMinor)
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
    }
}

// Debugging tools
[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class DebugManager
{
    private static readonly string passwordHash = "d1f51dfdfd8d38027fd2ca9dfeb299399b5bdee58e6c0b3b5e9a45cd4e502848";
    private static readonly Random random = new((int)DateTime.Now.Ticks);
    private static List<PlayerControl> bots = new();

    public static void Postfix(KeyboardJoystick __instance)
    {
        // Check if debug mode is active.
        var builder = new StringBuilder();
        var sha = SHA256Managed.Create();
        byte[] hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(TheOtherRolesPlugin.DebugMode.Value));
        foreach (var b in hashed) builder.Append(b.ToString("x2"));
        var enteredHash = builder.ToString();
        if (enteredHash != passwordHash) return;


        // Spawn dummys
        if (Input.GetKeyDown(KeyCode.F))
        {
            var playerControl = UnityEngine.Object.Instantiate(AmongUsClient.Instance.PlayerPrefab);
            var i = playerControl.PlayerId = (byte)GameData.Instance.GetAvailableId();

            bots.Add(playerControl);
            GameData.Instance.AddDummy(playerControl);
            AmongUsClient.Instance.Spawn(playerControl, -2, InnerNet.SpawnFlags.None);

            playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
            playerControl.GetComponent<DummyBehaviour>().enabled = true;
            playerControl.NetTransform.enabled = false;
            playerControl.SetName(RandomString(10));
            playerControl.SetColor((byte)random.Next(Palette.PlayerColors.Length));
            playerControl.Data.RpcSetTasks(new byte[0]);
        }

        // Terminate round
        if (Input.GetKeyDown(KeyCode.L))
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ForceEnd, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.forceEnd();
        }
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}