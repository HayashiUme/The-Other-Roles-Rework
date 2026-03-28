using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using InnerNet;
using TheOtherRoles.Roles.Core.Events;
using UnityEngine;

namespace TheOtherRoles.Roles.Core;

/// <summary>
///     Central registry for all new-generation <see cref="RoleBase"/> roles.
///     Call <see cref="LoadAll"/> once from the plugin entry point.
/// </summary>
public static class RoleManager
{
    

    private static readonly List<RoleBase> _roles = new();
    private static readonly Dictionary<int, RoleBase>_byId = new();
    private static ManualLogSource? _logger;

    /// <summary>
    ///     Scans all loaded assemblies for <see cref="RegisterRoleAttribute"/>-decorated
    ///     <see cref="RoleBase"/> subclasses, instantiates them, calls <see cref="RoleBase.OnLoad"/>,
    ///     and registers their event handlers with <see cref="EventBus"/>.
    /// </summary>
    public static void LoadAll(ManualLogSource logger)
    {
        _logger = logger;
        EventBus.SetLogger(logger);

        _roles.Clear();
        _byId.Clear();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            IEnumerable<Type> types;
            try { types = assembly.GetTypes(); }
            catch { continue; }

            foreach (var type in types)
            {
                if (type.IsAbstract) continue;
                if (!type.IsSubclassOf(typeof(RoleBase))) continue;
                if (type.GetCustomAttribute<RegisterRoleAttribute>() == null) continue;

                try
                {
                    var role = (RoleBase)Activator.CreateInstance(type)!;
                    _roles.Add(role);
                    _byId[role.RoleId] = role;

                    EventBus.Register(role);
                    role.OnLoad();

                    _logger.LogInfo($"[RoleManager] Loaded role: {role.RoleName} (id={role.RoleId}, team={role.Team})");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[RoleManager] Failed to load {type.FullName}: {ex}");
                }
            }
        }

        _logger.LogInfo($"[RoleManager] Total new-style roles loaded: {_roles.Count}");
    }

    // ── Query ─────────────────────────────────────────────────────────────────

    /// <summary>All registered role instances.</summary>
    public static IReadOnlyList<RoleBase> AllRoles => _roles;

    /// <summary>Returns the role with <paramref name="roleId"/>, or <c>null</c>.</summary>
    public static RoleBase? GetById(int roleId)
        => _byId.TryGetValue(roleId, out var r) ? r : null;

    /// <summary>Returns the first role of type <typeparamref name="T"/>, or <c>null</c>.</summary>
    public static T? Get<T>() where T : RoleBase
        => _roles.OfType<T>().FirstOrDefault();

    // ── Game lifecycle ────────────────────────────────────────────────────────

    /// <summary>
    ///     Resets all registered roles to their default state.
    ///     Call from <c>RPCProcedure.resetVaribles()</c>.
    /// </summary>
    public static void ClearAll()
    {
        foreach (var role in _roles)
        {
            try { role.ClearAndReload(); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] ClearAndReload failed for {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>Called once roles have been assigned.  Fires <see cref="GameStartEvent"/> and routes to each role.</summary>
    public static void OnGameStart()
    {
        EventBus.Dispatch(new GameStartEvent(), EventHandlerType.Postfix);
        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnGameStart(); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnGameStart failed for {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>Called when game ends / returns to lobby.</summary>
    public static void OnGameEnd()
    {
        EventBus.Dispatch(new GameEndEvent(), EventHandlerType.Postfix);
        foreach (var role in _roles)
        {
            try { role.OnGameEnd(); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnGameEnd failed for {role.RoleName}: {ex}"); }
        }
    }

    // ── Player hooks (called from HarmonyPatches) ─────────────────────────────

    /// <summary>
    ///     Called every <c>FixedUpdate</c> for the local player.
    ///     Routes to each active role's <see cref="RoleBase.OnFixedUpdate"/> and
    ///     fires <see cref="PlayerFixedUpdateEvent"/> on the EventBus.
    /// </summary>
    public static void OnFixedUpdate(PlayerControl localPlayer)
    {
        var evt = new PlayerFixedUpdateEvent(localPlayer);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnFixedUpdate(localPlayer); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnFixedUpdate {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>
    ///     Called every <c>FixedUpdate</c> for every player instance.
    ///     Routes to each active role's <see cref="RoleBase.OnFixedUpdateAll"/>.
    /// </summary>
    public static void OnFixedUpdateAll(PlayerControl player)
    {
        var evt = new PlayerFixedUpdateAllEvent(player);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnFixedUpdateAll(player); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnFixedUpdateAll {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>
    ///     Called in <c>MurderPlayer</c> Prefix.
    ///     If any Prefix handler or role returns <c>false</c>, the kill is cancelled.
    /// </summary>
    public static bool OnMurderPlayerPre(PlayerControl killer, PlayerControl target)
    {
        var evt = new PlayerMurderPreEvent(killer, target);
        var allow = EventBus.Dispatch(evt, EventHandlerType.Prefix);

        foreach (var role in _roles)
        {
            try { if (!role.OnMurderPlayerPre(killer, target)) allow = false; }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnMurderPlayerPre {role.RoleName}: {ex}"); }
        }

        return allow;
    }

    /// <summary>Called in <c>MurderPlayer</c> Postfix.</summary>
    public static void OnMurderPlayer(PlayerControl killer, PlayerControl target)
    {
        var evt = new PlayerMurderEvent(killer, target);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnMurderPlayer(killer, target); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnMurderPlayer {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>Called in <c>PlayerControl.Exiled</c> Postfix.</summary>
    public static void OnExiled(PlayerControl player)
    {
        var evt = new PlayerExiledEvent(player);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnExiled(player); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnExiled {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>
    ///     Called in <c>CmdReportDeadBody</c> Prefix.
    ///     Returns <c>false</c> if any handler suppresses the report.
    /// </summary>
    public static bool OnBodyReport(PlayerControl reporter, NetworkedPlayerInfo body)
    {
        var evt = new PlayerBodyReportEvent(reporter, body);
        var allow = EventBus.Dispatch(evt, EventHandlerType.Prefix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive && !role.OnBodyReport(reporter, body)) allow = false; }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnBodyReport {role.RoleName}: {ex}"); }
        }

        return allow;
    }

    /// <summary>Called in <c>HudManager.Update</c> Postfix every frame.</summary>
    public static void OnHudUpdate(HudManager hud)
    {
        var evt = new HudManagerUpdateEvent(hud);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnHudUpdate(hud); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnHudUpdate {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>Called in <c>MeetingHud.Start</c> Postfix.</summary>
    public static void OnMeetingStart(MeetingHud meeting)
    {
        var evt = new MeetingStartEvent(meeting);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnMeetingStart(meeting); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnMeetingStart {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>Called in <c>MeetingHud.VotingComplete</c> Postfix.</summary>
    public static void OnVotingComplete(MeetingHud meeting, MeetingHud.VoterState[]? states, NetworkedPlayerInfo? exiled, bool isTie)
    {
        var evt = new MeetingVotingCompleteEvent(meeting, states, exiled, isTie);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnVotingComplete(meeting, states, exiled, isTie); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnVotingComplete {role.RoleName}: {ex}"); }
        }
    }

    /// <summary>Called in <c>ExileController.BeginForGameplay</c> Prefix.</summary>
    public static void OnExileBegin(ExileController controller, NetworkedPlayerInfo? exiled)
    {
        var evt = new ExileBeginEvent(controller, exiled);
        EventBus.Dispatch(evt, EventHandlerType.Postfix);

        foreach (var role in _roles)
        {
            try { if (role.IsActive) role.OnExileBegin(controller, exiled); }
            catch (Exception ex) { _logger?.LogError($"[RoleManager] OnExileBegin {role.RoleName}: {ex}"); }
        }
    }
}
