using System;
using Hazel;
using System.Collections.Generic;

namespace TheOtherRoles.Roles.Core.Rpc;
internal interface IRoleRpc
{
    uint AllocatedId { get; internal set; }
    void InvokeReceive(MessageReader reader);
}


/// <summary>
///     A role-scoped RPC field.  Create one per network action via
///     <see cref="RoleBase.NewRpc(int)"/> or <see cref="RoleBase.NewRpc(Enum)"/>.
/// </summary>
public sealed class RoleRpc : IRoleRpc
{
    private readonly RoleBase                          _owner;
    private Action<RoleRpcReceiveContext>?             _receiveHandler;

    internal RoleRpc(RoleBase owner) { _owner = owner; }

    uint IRoleRpc.AllocatedId { get; set; }

    public uint AllocatedId => ((IRoleRpc)this).AllocatedId;

    void IRoleRpc.InvokeReceive(MessageReader reader)
    {
        try
        {
            _receiveHandler?.Invoke(new RoleRpcReceiveContext(reader));
        }
        catch (Exception ex)
        {
            TheOtherRolesPlugin.Logger.LogError(
                $"[RoleRpc] Exception in receive handler for {_owner.RoleName} " +
                $"(allocId={AllocatedId}): {ex}");
        }
    }

    /// <summary>
    ///     Registers the delegate called when this RPC arrives over the network.
    ///     Use <see cref="RoleRpcReceiveContext.Read{T}"/> to pull payload values
    ///     in the same order they were written.
    /// </summary>
    public RoleRpc Receive(Action<RoleRpcReceiveContext> handler)
    {
        _receiveHandler = handler;
        return this;
    }

    /// <summary>Starts building an outgoing RPC packet.  Chain Write&lt;T&gt; then call Send or Perform.</summary>
    public RoleRpcBuildContext Create() => new(_owner, AllocatedId);
}

/// <summary>Strongly-typed single-argument RoleRpc.  Created via <see cref="RoleBase.NewRpc{T}"/>.</summary>
public sealed class RoleRpc<T> : IRoleRpc where T : notnull
{
    private readonly RoleBase              _owner;
    private readonly Action<T>             _onPerform;
    private readonly Action<MessageWriter, T>   _onSerialize;
    private readonly Func<MessageReader, T>     _onDeserialize;

    uint IRoleRpc.AllocatedId { get; set; }
    public uint AllocatedId => ((IRoleRpc)this).AllocatedId;

    void IRoleRpc.InvokeReceive(MessageReader reader)
    {
        try { _onPerform(_onDeserialize(reader)); }
        catch (Exception ex)
        {
            TheOtherRolesPlugin.Logger.LogError($"[RoleRpc<{typeof(T).Name}>] Exception in {_owner.RoleName}: {ex}");
        }
    }

    internal RoleRpc(RoleBase owner, Action<T> onPerform)
        : this(owner, onPerform,
               (w, v) => RoleRpcAutoSerializer.Write(w, v),
               r      => RoleRpcAutoSerializer.Read<T>(r)) { }

    internal RoleRpc(RoleBase owner, Action<T> onPerform,
                     Action<MessageWriter, T> onSerialize, Func<MessageReader, T> onDeserialize)
    {
        _owner         = owner;
        _onPerform     = onPerform;
        _onSerialize   = onSerialize;
        _onDeserialize = onDeserialize;
    }

    public void Send(T arg, PlayerControl? sender = null)
        => SendCore(arg, sender ?? PlayerControl.LocalPlayer);

    public void PerformAndSend(T arg, PlayerControl? sender = null)
    {
        _onPerform(arg);
        SendCore(arg, sender ?? PlayerControl.LocalPlayer);
    }

    private void SendCore(T arg, PlayerControl sender)
    {
        var w = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)CustomRPC.RoleRpc, SendOption.Reliable);
        w.Write(_owner.RoleId);
        w.Write(AllocatedId);
        _onSerialize(w, arg);
        AmongUsClient.Instance.FinishRpcImmediately(w);
    }
}

/// <summary>Strongly-typed two-argument RoleRpc.  Created via <see cref="RoleBase.NewRpc{T1,T2}"/>.</summary>
public sealed class RoleRpc<T1, T2> : IRoleRpc
    where T1 : notnull
    where T2 : notnull
{
    private readonly RoleBase                       _owner;
    private readonly Action<T1, T2>                 _onPerform;
    private readonly Action<MessageWriter, T1, T2>  _onSerialize;
    private readonly Func<MessageReader, (T1, T2)>  _onDeserialize;

    uint IRoleRpc.AllocatedId { get; set; }
    public uint AllocatedId => ((IRoleRpc)this).AllocatedId;

    void IRoleRpc.InvokeReceive(MessageReader reader)
    {
        try { var (a, b) = _onDeserialize(reader); _onPerform(a, b); }
        catch (Exception ex)
        {
            TheOtherRolesPlugin.Logger.LogError($"[RoleRpc<{typeof(T1).Name},{typeof(T2).Name}>] Exception in {_owner.RoleName}: {ex}");
        }
    }

    internal RoleRpc(RoleBase owner, Action<T1, T2> onPerform)
        : this(owner, onPerform,
               (w, a, b) => { RoleRpcAutoSerializer.Write(w, a); RoleRpcAutoSerializer.Write(w, b); },
               r => (RoleRpcAutoSerializer.Read<T1>(r), RoleRpcAutoSerializer.Read<T2>(r))) { }

    internal RoleRpc(RoleBase owner, Action<T1, T2> onPerform,
                     Action<MessageWriter, T1, T2> onSerialize, Func<MessageReader, (T1, T2)> onDeserialize)
    {
        _owner = owner; _onPerform = onPerform; _onSerialize = onSerialize; _onDeserialize = onDeserialize;
    }

    public void Send(T1 a, T2 b, PlayerControl? sender = null)
    {
        sender ??= PlayerControl.LocalPlayer;
        var w = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)CustomRPC.RoleRpc, SendOption.Reliable);
        w.Write(_owner.RoleId); w.Write(AllocatedId);
        _onSerialize(w, a, b);
        AmongUsClient.Instance.FinishRpcImmediately(w);
    }

    public void PerformAndSend(T1 a, T2 b, PlayerControl? sender = null)
    {
        _onPerform(a, b);
        Send(a, b, sender);
    }
}

/// <summary>Strongly-typed three-argument RoleRpc.  Created via <see cref="RoleBase.NewRpc{T1,T2,T3}"/>.</summary>
public sealed class RoleRpc<T1, T2, T3> : IRoleRpc
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
    private readonly RoleBase                          _owner;
    private readonly Action<T1, T2, T3>                _onPerform;
    private readonly Action<MessageWriter, T1, T2, T3> _onSerialize;
    private readonly Func<MessageReader, (T1, T2, T3)> _onDeserialize;

    uint IRoleRpc.AllocatedId { get; set; }
    public uint AllocatedId => ((IRoleRpc)this).AllocatedId;

    void IRoleRpc.InvokeReceive(MessageReader reader)
    {
        try { var (a, b, c) = _onDeserialize(reader); _onPerform(a, b, c); }
        catch (Exception ex)
        {
            TheOtherRolesPlugin.Logger.LogError($"[RoleRpc<{typeof(T1).Name},{typeof(T2).Name},{typeof(T3).Name}>] Exception in {_owner.RoleName}: {ex}");
        }
    }

    internal RoleRpc(RoleBase owner, Action<T1, T2, T3> onPerform)
        : this(owner, onPerform,
               (w, a, b, c) => { RoleRpcAutoSerializer.Write(w, a); RoleRpcAutoSerializer.Write(w, b); RoleRpcAutoSerializer.Write(w, c); },
               r => (RoleRpcAutoSerializer.Read<T1>(r), RoleRpcAutoSerializer.Read<T2>(r), RoleRpcAutoSerializer.Read<T3>(r))) { }

    internal RoleRpc(RoleBase owner, Action<T1, T2, T3> onPerform,
                     Action<MessageWriter, T1, T2, T3> onSerialize, Func<MessageReader, (T1, T2, T3)> onDeserialize)
    {
        _owner = owner; _onPerform = onPerform; _onSerialize = onSerialize; _onDeserialize = onDeserialize;
    }

    public void Send(T1 a, T2 b, T3 c, PlayerControl? sender = null)
    {
        sender ??= PlayerControl.LocalPlayer;
        var w = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)CustomRPC.RoleRpc, SendOption.Reliable);
        w.Write(_owner.RoleId); w.Write(AllocatedId);
        _onSerialize(w, a, b, c);
        AmongUsClient.Instance.FinishRpcImmediately(w);
    }

    public void PerformAndSend(T1 a, T2 b, T3 c, PlayerControl? sender = null)
    {
        _onPerform(a, b, c);
        Send(a, b, c, sender);
    }
}

/// <summary>Fluent builder returned by <see cref="RoleRpc.Create()"/>.</summary>
public sealed class RoleRpcBuildContext
{
    private readonly RoleBase                  _owner;
    private readonly uint                      _allocatedId;
    private readonly List<Action<MessageWriter>> _writes = new();

    internal RoleRpcBuildContext(RoleBase owner, uint allocatedId)
    {
        _owner = owner; _allocatedId = allocatedId;
    }

    /// <summary>Appends <paramref name="value"/> to the payload using the auto-serialiser.</summary>
    public RoleRpcBuildContext Write<T>(T value) where T : notnull
    {
        var captured = value;
        _writes.Add(w => RoleRpcAutoSerializer.Write(w, captured));
        return this;
    }

    /// <summary>Returns a send context.  Call <c>.By(sender)</c> to fire the packet.</summary>
    public RoleRpcSendContext Send() => new(_owner, _allocatedId, _writes);

    /// <summary>Broadcasts from <see cref="PlayerControl.LocalPlayer"/>.</summary>
    public void SendByLocal() => Send().By(PlayerControl.LocalPlayer);

    /// <summary>Broadcasts from <paramref name="sender"/>.</summary>
    public void SendBy(PlayerControl sender) => Send().By(sender);

    /// <summary>
    ///     Sends over the network AND executes the receive handler locally so
    ///     the sender experiences the effect immediately.
    /// </summary>
    public void Perform(PlayerControl? sender = null)
    {
        sender ??= PlayerControl.LocalPlayer;

        // Serialise payload to a temporary buffer
        var msgWriter = MessageWriter.Get(SendOption.Reliable);
        foreach (var write in _writes) write(msgWriter);
        var payloadBytes = msgWriter.ToByteArray(false);
        msgWriter.Recycle();

        // Execute receive handler locally
        var localReader = MessageReader.Get(payloadBytes);
        var rpc = RoleRpcManager.GetByAllocatedId(_allocatedId);
        try   { rpc?.InvokeReceive(localReader); }
        finally { localReader.Recycle(); }

        // Broadcast over the network
        Send().By(sender);
    }
}

/// <summary>Returned by <see cref="RoleRpcBuildContext.Send()"/>. Call <see cref="By"/> to fire the packet.</summary>
public sealed class RoleRpcSendContext
{
    private readonly RoleBase                  _owner;
    private readonly uint                      _allocatedId;
    private readonly List<Action<MessageWriter>> _writes;
    private bool                               _sent;

    internal RoleRpcSendContext(RoleBase owner, uint allocatedId, List<Action<MessageWriter>> writes)
    {
        _owner = owner; _allocatedId = allocatedId; _writes = writes;
    }

    /// <summary>Writes the full packet (header + payload) and fires the RPC.</summary>
    public void By(PlayerControl? sender = null)
    {
        if (_sent) return;
        _sent = true;

        sender ??= PlayerControl.LocalPlayer;
        var w = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)CustomRPC.RoleRpc, SendOption.Reliable);
        w.Write(_owner.RoleId);
        w.Write(_allocatedId);
        foreach (var write in _writes) write(w);
        AmongUsClient.Instance.FinishRpcImmediately(w);
    }
}

/// <summary>
///     Passed to the delegate registered with <see cref="RoleRpc.Receive"/>.
///     Call <see cref="Read{T}"/> in the same order as the sender's <c>Write&lt;T&gt;</c> calls.
/// </summary>
public sealed class RoleRpcReceiveContext
{
    private readonly MessageReader _reader;
    internal RoleRpcReceiveContext(MessageReader reader) => _reader = reader;

    /// <summary>Reads the next value from the RPC payload.</summary>
    public T Read<T>() => RoleRpcAutoSerializer.Read<T>(_reader);
}
