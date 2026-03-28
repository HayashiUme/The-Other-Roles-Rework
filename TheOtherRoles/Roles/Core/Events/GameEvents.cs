using InnerNet;
using UnityEngine;

namespace TheOtherRoles.Roles.Core.Events;

/// <summary>Fired inside <c>PlayerControl.FixedUpdate</c> for the local player only.</summary>
public sealed class PlayerFixedUpdateEvent : RoleEvent
{
    public PlayerFixedUpdateEvent(PlayerControl player) => Player = player;
    public PlayerControl Player { get; }
}

/// <summary>Fired inside <c>PlayerControl.FixedUpdate</c> for every player instance.</summary>
public sealed class PlayerFixedUpdateAllEvent : RoleEvent
{
    public PlayerFixedUpdateAllEvent(PlayerControl player) => Player = player;
    public PlayerControl Player { get; }
}

/// <summary>
///     Fired in <c>PlayerControl.MurderPlayer</c> Prefix.
///     Cancel the event to prevent vanilla murder logic.
/// </summary>
public sealed class PlayerMurderPreEvent : RoleEvent
{
    public PlayerMurderPreEvent(PlayerControl killer, PlayerControl target)
    {
        Killer = killer;
        Target = target;
    }
    public PlayerControl Killer { get; }
    public PlayerControl Target { get; }
}

/// <summary>Fired in <c>PlayerControl.MurderPlayer</c> Postfix after the kill has happened.</summary>
public sealed class PlayerMurderEvent : RoleEvent
{
    public PlayerMurderEvent(PlayerControl killer, PlayerControl target)
    {
        Killer = killer;
        Target = target;
    }
    public PlayerControl Killer { get; }
    public PlayerControl Target { get; }
}

/// <summary>Fired in <c>PlayerControl.Exiled</c> Postfix.</summary>
public sealed class PlayerExiledEvent : RoleEvent
{
    public PlayerExiledEvent(PlayerControl player) => Player = player;
    public PlayerControl Player { get; }
}

/// <summary>
///     Fired in <c>PlayerControl.CmdReportDeadBody</c> Postfix
///     when a player reports a body.
/// </summary>
public sealed class PlayerBodyReportEvent : RoleEvent
{
    public PlayerBodyReportEvent(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        Reporter = reporter;
        Target   = target;
    }
    public PlayerControl      Reporter { get; }
    public NetworkedPlayerInfo Target   { get; }
}

/// <summary>
///     Fired when a player's role/position requires re-checking the vent availability.
///     Cancel to disallow venting.
/// </summary>
public sealed class PlayerVentCheckEvent : RoleEvent
{
    public PlayerVentCheckEvent(PlayerControl player, Vent vent)
    {
        Player = player;
        Vent   = vent;
    }
    public PlayerControl Player   { get; }
    public Vent          Vent     { get; }
    public bool          CanUse   { get; set; } = true;
    public bool          CouldUse { get; set; } = true;
}

/// <summary>Fired in <c>HudManager.Update</c> Postfix every frame.</summary>
public sealed class HudManagerUpdateEvent : RoleEvent
{
    public HudManagerUpdateEvent(HudManager manager) => Manager = manager;
    public HudManager Manager { get; }
}

/// <summary>Fired when the game starts (<c>GameManager.StartGame</c>).</summary>
public sealed class GameStartEvent : RoleEvent { }

/// <summary>Fired when the game ends / returns to lobby.</summary>
public sealed class GameEndEvent : RoleEvent { }

/// <summary>Fired when roles are being assigned so roles can react.</summary>
public sealed class RoleAssignmentEvent : RoleEvent { }

/// <summary>Fired in <c>MeetingHud.Start</c> Postfix.</summary>
public sealed class MeetingStartEvent : RoleEvent
{
    public MeetingStartEvent(MeetingHud meeting) => Meeting = meeting;
    public MeetingHud Meeting { get; }
}

/// <summary>Fired in <c>MeetingHud.VotingComplete</c> Postfix.</summary>
public sealed class MeetingVotingCompleteEvent : RoleEvent
{
    public MeetingVotingCompleteEvent(MeetingHud meeting, MeetingHud.VoterState[]? states, NetworkedPlayerInfo? exiled, bool isTie)
    {
        Meeting = meeting;
        States  = states;
        Exiled  = exiled;
        IsTie   = isTie;
    }
    public MeetingHud                 Meeting { get; }
    public MeetingHud.VoterState[]?   States  { get; }
    public NetworkedPlayerInfo?       Exiled  { get; }
    public bool                       IsTie   { get; }
}

/// <summary>Fired in <c>ExileController</c> Postfix after exile animation starts.</summary>
public sealed class ExileBeginEvent : RoleEvent
{
    public ExileBeginEvent(ExileController controller, NetworkedPlayerInfo? exiled)
    {
        Controller = controller;
        Exiled     = exiled;
    }
    public ExileController     Controller { get; }
    public NetworkedPlayerInfo? Exiled    { get; }
}
