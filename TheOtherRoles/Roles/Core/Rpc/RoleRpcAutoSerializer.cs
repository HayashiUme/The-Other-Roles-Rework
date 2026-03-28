using Hazel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles.Core.Rpc;

/// <summary>
///     Type-aware serialiser/deserialiser registry for <see cref="RoleRpc"/> auto-write and auto-read.
///
///     Built-in bindings:
///     <list type="table">
///         <item><term>bool</term>              <description>ReadBoolean / Write(bool)</description></item>
///         <item><term>byte</term>              <description>ReadByte / Write(byte)</description></item>
///         <item><term>sbyte</term>             <description>ReadSByte / Write(sbyte)</description></item>
///         <item><term>int</term>               <description>ReadInt32 / Write(int)</description></item>
///         <item><term>uint</term>              <description>ReadUInt32 / Write(uint)</description></item>
///         <item><term>float</term>             <description>ReadSingle / Write(float)</description></item>
///         <item><term>string</term>            <description>ReadString / Write(string)</description></item>
///         <item><term>byte[]</term>            <description>ReadBytesAndSize / WriteBytesAndSize</description></item>
///         <item><term>Vector2</term>           <description>2× float</description></item>
///         <item><term>PlayerControl</term>     <description>PlayerId byte</description></item>
///         <item><term>NetworkedPlayerInfo</term><description>PlayerId byte</description></item>
///     </list>
///
///     Extend with <see cref="Register{T}"/> for custom types.
/// </summary>
public static class RoleRpcAutoSerializer
{
    private readonly struct Entry
    {
        internal readonly Action<MessageWriter, object?> Write;
        internal readonly Func<MessageReader, object>   Read;
        internal Entry(Action<MessageWriter, object?> write, Func<MessageReader, object> read)
        {
            Write = write; Read = read;
        }
    }

    private static readonly Dictionary<Type, Entry> _registry = new();

    static RoleRpcAutoSerializer()
    {
        // Primitives
        Reg<bool>  ((w, v) => w.Write(v),           r => r.ReadBoolean());
        Reg<byte>  ((w, v) => w.Write(v),           r => r.ReadByte());
        Reg<sbyte> ((w, v) => w.Write(v),           r => r.ReadSByte());
        Reg<int>   ((w, v) => w.Write(v),           r => r.ReadInt32());
        Reg<uint>  ((w, v) => w.Write(v),           r => r.ReadUInt32());
        Reg<float> ((w, v) => w.Write(v),           r => r.ReadSingle());
        Reg<string>((w, v) => w.Write(v),           r => r.ReadString());
        Reg<byte[]>((w, v) => w.WriteBytesAndSize(v), r => (byte[])r.ReadBytesAndSize());

        // Unity
        Reg<Vector2>(
            (w, v) => { w.Write(v.x); w.Write(v.y); },
            r => new Vector2(r.ReadSingle(), r.ReadSingle()));

        // Among Us — PlayerControl via PlayerId byte
        Reg<PlayerControl>(
            (w, v) => w.Write(v.PlayerId),
            r => Helpers.playerById(r.ReadByte()));

        // NetworkedPlayerInfo via PlayerId byte
        Reg<NetworkedPlayerInfo>(
            (w, v) => w.Write(v.PlayerId),
            r => GameData.Instance.GetPlayerById(r.ReadByte()));
    }

    private static void Reg<T>(Action<MessageWriter, T> write, Func<MessageReader, T> read) where T : notnull
    {
        _registry[typeof(T)] = new Entry(
            (w, v) => write(w, (T)v!),
            r      => read(r)
        );
    }

    /// <summary>
    ///     Registers a custom type binding.
    ///     Call this in your plugin entry point for types not covered by the built-in list.
    /// </summary>
    public static void Register<T>(Action<MessageWriter, T> write, Func<MessageReader, T> read) where T : notnull
        => Reg(write, read);

    /// <summary>Writes <paramref name="value"/> of type <typeparamref name="T"/> to <paramref name="writer"/>.</summary>
    /// <exception cref="NotSupportedException">Type has no registered binding.</exception>
    internal static void Write<T>(MessageWriter writer, T value)
    {
        var type = typeof(T);
        if (!_registry.TryGetValue(type, out var entry))
            throw new NotSupportedException(
                $"[RoleRpc] No auto-serialiser registered for type '{type.FullName}'. " +
                $"Call {nameof(RoleRpcAutoSerializer)}.{nameof(Register)}<{type.Name}>(...) first.");
        entry.Write(writer, value);
    }

    /// <summary>Reads a value of type <typeparamref name="T"/> from <paramref name="reader"/>.</summary>
    /// <exception cref="NotSupportedException">Type has no registered binding.</exception>
    internal static T Read<T>(MessageReader reader)
    {
        var type = typeof(T);
        if (!_registry.TryGetValue(type, out var entry))
            throw new NotSupportedException(
                $"[RoleRpc] No auto-deserialiser registered for type '{type.FullName}'.");
        return (T)entry.Read(reader);
    }
}
