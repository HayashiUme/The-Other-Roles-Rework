using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;

namespace TheOtherRoles.Roles.Core.Events;

/// <summary>
///     Central hub for all in-game events fired by <see cref="RoleManager"/>-routed patches.
///     Roles that want to handle events must implement handler methods decorated with
///     <see cref="EventHandlerAttribute"/> and pass <c>this</c> to
///     <see cref="Register(object)"/> (done automatically by <see cref="RoleBase"/> during load).
/// </summary>
public static class EventBus
{
    private sealed class Handler
    {
        public object            Listener        { get; }
        public MethodInfo        Method          { get; }
        public Type              EventType       { get; }
        public EventHandlerType  HandlerType     { get; }

        public Handler(object listener, MethodInfo method, EventHandlerType type)
        {
            Listener    = listener;
            Method      = method;
            HandlerType = type;

            var p = method.GetParameters();
            if (p.Length != 1)
                throw new ArgumentException($"EventHandler method '{method.Name}' must have exactly one parameter.");

            EventType = p[0].ParameterType;
            if (!EventType.IsSubclassOf(typeof(RoleEvent)))
                throw new ArgumentException($"EventHandler method '{method.Name}' parameter must be a RoleEvent subclass.");
        }
    }

    private static readonly List<Handler>        _handlers   = new();
    private static          ManualLogSource?     _logger;

    internal static void SetLogger(ManualLogSource logger) => _logger = logger;

    /// <summary>
    ///     Scans <paramref name="listener"/> for all methods tagged with
    ///     <see cref="EventHandlerAttribute"/> and registers them.
    ///     Called automatically for every <see cref="RoleBase"/> instance after instantiation.
    /// </summary>
    public static void Register(object listener)
    {
        var methods = listener.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<EventHandlerAttribute>();
            if (attr == null) continue;

            try
            {
                _handlers.Add(new Handler(listener, method, attr.EventHandlerType));
                _logger?.LogDebug($"[EventBus] Registered {listener.GetType().Name}.{method.Name} ({attr.EventHandlerType})");
            }
            catch (Exception ex)
            {
                _logger?.LogError($"[EventBus] Failed to register {listener.GetType().Name}.{method.Name}: {ex.Message}");
            }
        }
    }

    /// <summary>Removes all handlers belonging to <paramref name="listener"/>.</summary>
    public static void Unregister(object listener)
    {
        _handlers.RemoveAll(h => ReferenceEquals(h.Listener, listener));
    }

    /// <summary>Removes all registered handlers (e.g. on game end / lobby return).</summary>
    public static void Clear() => _handlers.Clear();

    /// <summary>
    ///     Fires <paramref name="evt"/> through all matching handlers of <paramref name="type"/>.
    ///     For Prefix handlers that return <c>bool</c>, returning <c>false</c> marks the event
    ///     as cancelled and this method returns <c>false</c>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if vanilla code should proceed; <c>false</c> if any Prefix handler
    ///     cancelled the event.
    /// </returns>
    public static bool Dispatch<TEvent>(TEvent evt, EventHandlerType type) where TEvent : RoleEvent
    {
        var toRun = _handlers
            .Where(h => h.HandlerType == type && h.EventType.IsInstanceOfType(evt))
            .ToList(); // snapshot to avoid modification-during-iteration crashes

        var allow = true;

        for (var i = 0; i < toRun.Count; i++)
        {
            var h = toRun[i];
            try
            {
                var result = h.Method.Invoke(h.Listener, new object[] { evt });
                if (type == EventHandlerType.Prefix && h.Method.ReturnType == typeof(bool))
                    if (!(bool)result!)
                        allow = false;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"[EventBus] Exception in {h.Listener.GetType().Name}.{h.Method.Name}: {ex}");
            }
        }

        return allow && !evt.IsCancelled;
    }

    /// <summary>Fires Prefix handlers then Postfix handlers for <paramref name="evt"/>.</summary>
    public static bool DispatchAll<TEvent>(TEvent evt) where TEvent : RoleEvent
    {
        var allow = Dispatch(evt, EventHandlerType.Prefix);
        Dispatch(evt, EventHandlerType.Postfix);
        return allow;
    }
}
