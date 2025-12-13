namespace GrainFramework;

/// <summary>
/// Abstraction for loading and saving grain state.
/// </summary>
public interface IGrainStateStore
{
    /// <summary>
    /// Tries to load the state for the given grain id.
    /// </summary>
    /// <param name="id">Identifier of the grain to load.</param>
    /// <param name="state">State data if found.</param>
    /// <returns>True if state was loaded.</returns>
    bool TryLoad(GrainId id, out object state);

    /// <summary>
    /// Saves the state for the given grain id.
    /// </summary>
    /// <param name="id">Identifier of the grain to save.</param>
    /// <param name="state">State data to persist.</param>
    void Save(GrainId id, object state);
}
