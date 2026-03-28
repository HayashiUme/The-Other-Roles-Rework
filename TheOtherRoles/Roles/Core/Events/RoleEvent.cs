using System;

namespace TheOtherRoles.Roles.Core.Events;

/// <summary>
///     Base class for every event dispatched through <see cref="EventBus"/>.
///     Derive from this class to create a custom event.
/// </summary>
public abstract class RoleEvent
{
    protected RoleEvent()
    {
        Name = GetType().Name;
    }

    /// <summary>The simple class name of this event (e.g. "PlayerMurderEvent").</summary>
    public string Name { get; }

    /// <summary>
    ///     Set to <c>false</c> inside a Prefix handler to cancel the vanilla
    ///     behaviour associated with this event (when the patch supports it).
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>Cancels the event so downstream Prefix patches return <c>false</c>.</summary>
    public void Cancel() => IsCancelled = true;
}

/// <summary>Mirrors COG's EventHandlerType: when the handler executes relative to the patch.</summary>
public enum EventHandlerType
{
    /// <summary>Runs before the vanilla code.  Handler may cancel the event.</summary>
    Prefix,
    /// <summary>Runs after the vanilla code.</summary>
    Postfix
}

/// <summary>
///     Decorate a method inside a <see cref="RoleBase"/> subclass with this attribute
///     to make it an event handler registered automatically by <see cref="EventBus"/>.
/// </summary>
/// <example>
/// <code>
/// [EventHandler(EventHandlerType.Postfix)]
/// public void OnMurderPlayer(PlayerMurderEvent e)
/// {
///     if (e.Target.IsRole(this)) HandleKill(e.Killer);
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class EventHandlerAttribute : Attribute
{
    public EventHandlerAttribute(EventHandlerType type = EventHandlerType.Postfix)
    {
        EventHandlerType = type;
    }

    public EventHandlerType EventHandlerType { get; }
}
