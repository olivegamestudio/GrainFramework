namespace GrainFramework;

public interface IGrain
{
    GrainId Id { get; }

    /// <summary>
    /// Enqueue an input message for this grain.
    /// </summary>
    void Enqueue(IGrainMessage message);

    /// <summary>
    /// Advance the grain by one simulation tick.
    /// </summary>
    void Tick(TimeSpan deltaTime);

    /// <summary>
    /// Drain outputs emitted since the last tick.
    /// </summary>
    IReadOnlyList<IGrainOutput> DrainOutputs();
}
