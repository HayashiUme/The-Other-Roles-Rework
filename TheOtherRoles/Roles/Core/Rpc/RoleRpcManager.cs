using Hazel;
using System.Collections.Generic;

namespace TheOtherRoles.Roles.Core.Rpc;

/// <summary>
///     Central registry and dispatcher for all role-scoped RPCs created by
///     <see cref="RoleBase.NewRpc"/> / <see cref="RoleBase.NewRpc{T}"/> etc.
///
///     Wire format  (shared callId <c>CustomRPC.RoleRpc</c>):
///     <code>
///     [ byte   callId   = 96        ]  written by AmongUsClient.StartRpcImmediately
///     [ int    roleId               ]  owning RoleBase.RoleId
///     [ uint   allocId  ≥ 1         ]  globally-unique, assigned at registration time
///     [ ...    payload              ]  role-defined via Write&lt;T&gt; calls
///     </code>
/// </summary>
public static class RoleRpcManager
{
    private static uint _nextId = 1;
    private static readonly Dictionary<uint, IRoleRpc>                          DispatchTable = new();
    private static readonly Dictionary<int, List<(int LocalId, uint AllocId)>> RoleRegistry  = new();

    // ── Registration ──────────────────────────────────────────────────────────

    /// <summary>
    ///     Assigns a globally-unique <see cref="uint"/> to <paramref name="handler"/>,
    ///     writes it back via <see cref="IRoleRpc.AllocatedId"/>, and adds it to the
    ///     dispatch table.  Called automatically by <see cref="RoleBase.NewRpc(int)"/>.
    /// </summary>
    internal static uint Register(RoleBase role, int localId, IRoleRpc handler)
    {
        var allocId = _nextId++;
        ((IRoleRpc)handler).AllocatedId = allocId;
        DispatchTable[allocId] = handler;

        if (!RoleRegistry.TryGetValue(role.RoleId, out var list))
            RoleRegistry[role.RoleId] = list = new();
        list.Add((localId, allocId));

        TheOtherRolesPlugin.Logger.LogDebug(
            $"[RoleRpcManager] Registered {role.RoleName}.localId={localId} → allocId={allocId}");

        return allocId;
    }

   
    /// <summary>
    ///     Entry point called from <c>RPCProcedure</c> when a <c>CustomRPC.RoleRpc</c> packet
    ///     arrives.  Reads <c>roleId</c> and <c>allocId</c>, then delegates to the handler.
    /// </summary>
    public static void Dispatch(MessageReader reader)
    {
        var roleId  = reader.ReadInt32();
        var allocId = reader.ReadUInt32();

        if (!DispatchTable.TryGetValue(allocId, out var handler))
        {
            TheOtherRolesPlugin.Logger.LogWarning(
                $"[RoleRpcManager] Unknown allocId={allocId} (roleId={roleId}). Packet discarded.");
            return;
        }

        try
        {
            handler.InvokeReceive(reader);
        }
        catch (System.Exception ex)
        {
            TheOtherRolesPlugin.Logger.LogError($"[RoleRpcManager] Exception dispatching allocId={allocId}: {ex}");
        }
    }

    // ── Lookup ────────────────────────────────────────────────────────────────

    /// <summary>Returns the handler for <paramref name="allocatedId"/>, or <c>null</c>.</summary>
    internal static IRoleRpc? GetByAllocatedId(uint allocatedId)
        => DispatchTable.TryGetValue(allocatedId, out var h) ? h : null;

    /// <summary>Returns a snapshot of the registry for debugging.</summary>
    public static IReadOnlyDictionary<int, List<(int LocalId, uint AllocId)>> GetRegistry()
        => RoleRegistry;
}
