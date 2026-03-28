using System;
using System.Collections.Generic;
using Hazel;
using TheOtherRoles.Patches;
using TheOtherRoles.Roles.Core.Rpc;
using UnityEngine;

namespace TheOtherRoles.Roles.Core;

// ── RoleTeam enum ─────────────────────────────────────────────────────────────

/// <summary>Which faction this role belongs to.</summary>
public enum RoleTeam
{
    Crewmate,
    Impostor,
    Neutral,
    /// <summary>A sub-role / modifier (no team affiliation).</summary>
    Modifier
}

// ── [RegisterRole] attribute ──────────────────────────────────────────────────

/// <summary>
///     Decorate a <see cref="RoleBase"/> subclass with this attribute to have
///     <see cref="RoleManager"/> discover and instantiate it automatically on load.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterRoleAttribute : Attribute { }

// ── RoleBase ──────────────────────────────────────────────────────────────────

/// <summary>
///     Abstract base class for every new-generation TOR role.
///
/// <para/>
///     Inherit this class and override the virtual hooks to implement role behaviour.
///     All player-related patches route through this class automatically — no need
///     to edit any other .cs files.
///
/// <para/>
///     Decorate event-handler methods with <see cref="Events.EventHandlerAttribute"/>
///     and they will be registered with the <see cref="Events.EventBus"/> automatically.
/// </summary>
public abstract class RoleBase
{
    // ── Identity ──────────────────────────────────────────────────────────────

    private static int _nextId = 2000; // IDs below 2000 reserved for legacy TOR roles

    /// <summary>Unique integer ID.  Auto-assigned at construction.</summary>
    public int RoleId { get; }

    /// <summary>Display name of the role.</summary>
    public string RoleName { get; }

    /// <summary>Which faction this role belongs to.</summary>
    public RoleTeam Team { get; }

    /// <summary>Role colour used for name tints, outlines, etc.</summary>
    public Color RoleColor { get; }

    /// <summary>Set in the constructor; indicates this role is a sub-role / modifier.</summary>
    public bool IsModifier => Team == RoleTeam.Modifier;

    /// <summary>Whether this role allows venting.  Override in constructor if needed.</summary>
    public bool CanVent { get; protected set; }

    /// <summary>Whether this role can perform kills.  Override in constructor if needed.</summary>
    public bool CanKill { get; protected set; }

    /// <summary>Whether this role can trigger sabotages.  Override in constructor if needed.</summary>
    public bool CanSabotage { get; protected set; }

    // ── Player reference (valid during a game) ────────────────────────────────

    /// <summary>The player currently assigned to this role.  Set by the role-assignment patch.</summary>
    public PlayerControl? Player { get; internal set; }

    /// <summary>Whether any player has been assigned to this role.</summary>
    public bool IsActive => Player != null;

    /// <summary>The current targeting target (set by derived class in <see cref="OnFixedUpdate"/>).</summary>
    public PlayerControl? CurrentTarget { get; protected set; }

    // ── RPC registry ──────────────────────────────────────────────────────────

    private readonly Dictionary<uint, IRoleRpc> _roleRpcHandlers = new();

    // ── Constructor ───────────────────────────────────────────────────────────

    protected RoleBase(string roleName, RoleTeam team, Color roleColor)
    {
        RoleId    = _nextId++;
        RoleName  = roleName;
        Team      = team;
        RoleColor = roleColor;

        CanVent      = team == RoleTeam.Impostor;
        CanKill      = team == RoleTeam.Impostor;
        CanSabotage  = team == RoleTeam.Impostor;
    }

    // ── Lifecycle hooks (override in derived classes) ─────────────────────────

    /// <summary>
    ///     Called every <c>FixedUpdate</c> frame for the LOCAL player only
    ///     when they are assigned to this role.
    ///     Good place for: target-finding, button cooldown updates, timer decrements.
    /// </summary>
    public virtual void OnFixedUpdate(PlayerControl localPlayer) { }

    /// <summary>
    ///     Called every <c>FixedUpdate</c> frame for EVERY player instance.
    ///     Good place for: per-player visual effects, animations.
    /// </summary>
    public virtual void OnFixedUpdateAll(PlayerControl player) { }

    /// <summary>
    ///     Called in <c>PlayerControl.MurderPlayer</c> Prefix.
    ///     Return <c>false</c> to prevent the vanilla kill (the player won't die).
    /// </summary>
    public virtual bool OnMurderPlayerPre(PlayerControl killer, PlayerControl target) => true;

    /// <summary>
    ///     Called in <c>PlayerControl.MurderPlayer</c> Postfix after the kill.
    ///     Both killer and target might belong to any role.
    /// </summary>
    public virtual void OnMurderPlayer(PlayerControl killer, PlayerControl target) { }

    /// <summary>
    ///     Called in <c>PlayerControl.Exiled</c> Postfix.
    ///     Useful for lover-suicide, lawyer-suicide, sidekick-promotion, etc.
    /// </summary>
    public virtual void OnExiled(PlayerControl player) { }

    /// <summary>
    ///     Called in <c>HudManager.Update</c> Postfix every frame.
    ///     Good place for HUD element updates (name colours, button visibility, etc.).
    /// </summary>
    public virtual void OnHudUpdate(HudManager hud) { }

    /// <summary>Called when a meeting starts (<c>MeetingHud.Start</c>).</summary>
    public virtual void OnMeetingStart(MeetingHud meeting) { }

    /// <summary>
    ///     Called when all votes have been cast (<c>MeetingHud.VotingComplete</c>).
    /// </summary>
    public virtual void OnVotingComplete(MeetingHud meeting, MeetingHud.VoterState[]? states, NetworkedPlayerInfo? exiled, bool isTie) { }

    /// <summary>
    ///     Called when the exile animation begins (<c>ExileController</c>).
    ///     Good place to show/hide role-summary data or trigger post-exile RPCs.
    /// </summary>
    public virtual void OnExileBegin(ExileController controller, NetworkedPlayerInfo? exiled) { }

    /// <summary>
    ///     Called when any player reports a body.
    ///     Return <c>false</c> to suppress the report (rare; most roles should not do this).
    /// </summary>
    public virtual bool OnBodyReport(PlayerControl reporter, NetworkedPlayerInfo reportedBody) => true;

    /// <summary>Called once the game has fully started (roles have been assigned).</summary>
    public virtual void OnGameStart() { }

    /// <summary>Called when the game ends / returns to lobby.  Clear role state here.</summary>
    public virtual void OnGameEnd() { }

    /// <summary>
    ///     Called by <see cref="RoleManager"/> when the role is first loaded.
    ///     Override to create custom buttons, register options, etc.
    /// </summary>
    public virtual void OnLoad() { }

    /// <summary>
    ///     Resets all mutable state of this role back to defaults.
    ///     Called at the start of each game and after exile.
    /// </summary>
    public virtual void ClearAndReload()
    {
        Player        = null;
        CurrentTarget = null;
    }

    // ── Targeting helpers (mirrors PlayerControlFixedUpdatePatch helpers) ──────

    /// <summary>
    ///     Returns the nearest valid target relative to <see cref="Player"/>,
    ///     or <c>null</c> if none is in range.
    /// </summary>
    protected PlayerControl? SetTarget(
        bool                onlyCrewmates       = false,
        bool                targetPlayersInVents = false,
        List<PlayerControl>? untargetablePlayers = null)
    {
        return PlayerControlFixedUpdatePatch.setTarget(
            onlyCrewmates, targetPlayersInVents, untargetablePlayers, Player);
    }

    /// <summary>Sets the material outline on <paramref name="target"/> with <paramref name="color"/>.</summary>
    protected static void SetPlayerOutline(PlayerControl? target, Color color)
    {
        PlayerControlFixedUpdatePatch.setPlayerOutline(target, color);
    }

    // ── RPC factory methods ───────────────────────────────────────────────────

    /// <summary>
    ///     Creates and registers an untyped fluent-builder RoleRpc for this role.
    ///     Use in constructor.
    /// </summary>
    /// <param name="localId">An int or enum-cast-to-int unique per role.</param>
    protected RoleRpc NewRpc(int localId)
    {
        var rpc = new RoleRpc(this);
        RoleRpcManager.Register(this, localId, rpc);
        _roleRpcHandlers[rpc.AllocatedId] = rpc;
        return rpc;
    }

    /// <summary>Overload accepting an <see cref="Enum"/> for readability.</summary>
    protected RoleRpc NewRpc(Enum localId) => NewRpc(Convert.ToInt32(localId));

    /// <summary>Creates and registers a strongly-typed single-argument RoleRpc.</summary>
    protected RoleRpc<T> NewRpc<T>(int localId, Action<T> onReceive) where T : notnull
    {
        var rpc = new RoleRpc<T>(this, onReceive);
        RoleRpcManager.Register(this, localId, rpc);
        _roleRpcHandlers[rpc.AllocatedId] = rpc;
        return rpc;
    }

    /// <summary>Overload accepting an <see cref="Enum"/> for readability.</summary>
    protected RoleRpc<T> NewRpc<T>(Enum localId, Action<T> onReceive) where T : notnull
        => NewRpc(Convert.ToInt32(localId), onReceive);

    /// <summary>Creates and registers a strongly-typed two-argument RoleRpc.</summary>
    protected RoleRpc<T1, T2> NewRpc<T1, T2>(int localId, Action<T1, T2> onReceive)
        where T1 : notnull where T2 : notnull
    {
        var rpc = new RoleRpc<T1, T2>(this, onReceive);
        RoleRpcManager.Register(this, localId, rpc);
        _roleRpcHandlers[rpc.AllocatedId] = rpc;
        return rpc;
    }

    /// <summary>Overload accepting an <see cref="Enum"/> for readability.</summary>
    protected RoleRpc<T1, T2> NewRpc<T1, T2>(Enum localId, Action<T1, T2> onReceive)
        where T1 : notnull where T2 : notnull
        => NewRpc(Convert.ToInt32(localId), onReceive);

    /// <summary>Creates and registers a strongly-typed three-argument RoleRpc.</summary>
    protected RoleRpc<T1, T2, T3> NewRpc<T1, T2, T3>(int localId, Action<T1, T2, T3> onReceive)
        where T1 : notnull where T2 : notnull where T3 : notnull
    {
        var rpc = new RoleRpc<T1, T2, T3>(this, onReceive);
        RoleRpcManager.Register(this, localId, rpc);
        _roleRpcHandlers[rpc.AllocatedId] = rpc;
        return rpc;
    }

    /// <summary>Dispatches an incoming RoleRpc packet to this role's handler table.</summary>
    internal void DispatchRoleRpc(IRoleRpc handler, MessageReader reader)
    {
        handler.InvokeReceive(reader);
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    /// <summary>Returns a hex-coloured version of <see cref="RoleName"/>.</summary>
    public string GetColoredName() => Helpers.cs(RoleColor, RoleName);

    /// <summary>Whether the local player is assigned to this role.</summary>
    public bool IsLocalPlayerRole() => Player != null && Player == PlayerControl.LocalPlayer;

    /// <summary>Whether <paramref name="player"/> is assigned to this role.</summary>
    public bool IsPlayerRole(PlayerControl player) => Player != null && Player == player;
}
