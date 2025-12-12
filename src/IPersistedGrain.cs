namespace GrainFramework;

/// <summary>
/// A grain that can capture and restore its state for persistence.
/// </summary>
public interface IPersistedGrain : IGrain
{
    /// <summary>
    /// Capture a snapshot of the current state for persistence.
    /// </summary>
    IGrainState CaptureState();

    /// <summary>
    /// Restore state from a previously captured snapshot.
    /// </summary>
    void RestoreState(IGrainState state);
}
