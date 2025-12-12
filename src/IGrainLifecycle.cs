namespace GrainFramework;

/// <summary>
/// Optional lifecycle hooks for grains managed by a runtime.
/// </summary>
public interface IGrainLifecycle
{
    /// <summary>
    /// Invoked when the grain is registered/activated by the runtime.
    /// </summary>
    void OnActivated();

    /// <summary>
    /// Invoked when the grain is being removed by the runtime.
    /// </summary>
    void OnDeactivated();
}
