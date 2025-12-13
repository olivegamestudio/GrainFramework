namespace GrainFramework;

/// <summary>
/// Hosts grains, delivers messages, advances them deterministically via ticks,
/// and exposes their outputs. Implementations may also restore and persist grain
/// state via an <see cref="IGrainStateStore"/>.
/// </summary>
public interface IGrainRuntime
{
    /// <summary>
    /// Register a grain instance with the runtime.
    /// </summary>
    void Register(IGrain grain);

    /// <summary>
    /// Unregister a grain instance with the runtime.
    /// </summary>
    void Unregister(IGrain grain);

    /// <summary>
    /// Save the state of all persisted grains via the configured state store.
    /// </summary>
    void SaveAll();

    /// <summary>
    /// Returns true if a grain with the given id is registered.
    /// </summary>
    bool Contains(GrainId id);

    /// <summary>
    /// Try get a registered grain.
    /// </summary>
    bool TryGet(GrainId id, out IGrain grain);

    /// <summary>
    /// Enqueue a message to a specific grain.
    /// </summary>
    void Enqueue(GrainId target, IGrainMessage message);

    /// <summary>
    /// Advance all grains by one simulation tick.
    /// Implementations must be deterministic (stable grain processing order).
    /// </summary>
    void TickAll(TimeSpan deltaTime);

    /// <summary>
    /// Drain outputs emitted by all grains since the last drain.
    /// </summary>
    IReadOnlyList<IGrainOutput> DrainAllOutputs();
}
