// ============================================================
//  ExampleRole — demonstrates the new RoleBase system
//
//  HOW TO WRITE A NEW ROLE (self-contained in one file)
//  ─────────────────────────────────────────────────────
//
//  1. Inherit RoleBase, decorate with [RegisterRole].
//  2. Declare options, RPCs, and buttons in the constructor + OnLoad().
//  3. Override the virtual hooks you need.
//  4. Use [EventHandler] for event-driven logic — no editing other files!
//  5. Use NewRpc / NewRpc<T> for networked actions.
//
//  NOTHING else needs to change in any other .cs file.
// ============================================================

using HarmonyLib;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Roles.Core.Events;
using TheOtherRoles.Roles.Core.Rpc;
using UnityEngine;

namespace TheOtherRoles.Roles.Example;

/// <summary>
///     一個案例職業，如果你有興趣，可以這麽把職業重寫。
/// </summary>
[RegisterRole]
public sealed class ExampleRole : RoleBase
{
    private readonly RoleRpc<PlayerControl> _doActionRpc;

    private bool _actionUsed;

    public ExampleRole()
        : base("Example", RoleTeam.Crewmate, new Color32(200, 180, 60, 255))
    {
        _doActionRpc = NewRpc<PlayerControl>(0, target =>
        {
            TheOtherRolesPlugin.Logger.LogInfo($"[ExampleRole] DoAction received, target={target?.name}");
            _actionUsed = true;
        });
    }

    public override void OnLoad()
    {
    }

    public override void ClearAndReload()
    {
        base.ClearAndReload();
        _actionUsed = false;
    }

    public override void OnFixedUpdate(PlayerControl localPlayer)
    {
        CurrentTarget = SetTarget();
        SetPlayerOutline(CurrentTarget, RoleColor);
    }

    public override void OnHudUpdate(HudManager hud)
    {
    }

    [EventHandler(EventHandlerType.Postfix)]
    public void OnMurder(PlayerMurderEvent e)
    {
        if (!IsActive) return;
        if (e.Killer == Player)
        {
            TheOtherRolesPlugin.Logger.LogInfo("[ExampleRole] The ExampleRole player killed someone!");
        }
    }

    [EventHandler(EventHandlerType.Postfix)]
    public void OnMeetingBegin(MeetingStartEvent e)
    {
        if (!IsActive) return;
    }
    /// <summary>
    ///     CustomClick的時候調用
    /// </summary>
    public void DoAction(PlayerControl target)
    {
        _doActionRpc.PerformAndSend(target);
    }
}
