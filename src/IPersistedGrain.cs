namespace GrainFramework;

/// <summary>
/// Grain contract for persistence integration.
/// </summary>
public interface IPersistedGrain
{
    /// <summary>
    /// Capture the current state of the grain for persistence.
    /// </summary>
    /// <returns>Serializable state object.</returns>
    object CaptureState();

    /// <summary>
    /// Restore the grain's state from previously captured data.
    /// </summary>
    /// <param name="state">State object produced by <see cref="CaptureState"/>.</param>
    void RestoreState(object state);

    /// <summary>
    /// Indicates whether the grain's state has been modified since the last persistence operation.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Clears the dirty flag, indicating that the grain's state is now in sync with the persisted state.
    /// </summary>
    void MarkClean();
}
