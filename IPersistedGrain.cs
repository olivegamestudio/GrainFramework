namespace GrainFramework;

public interface IPersistedGrain : IGrain
{
    IGrainState CaptureState();

    void RestoreState(IGrainState state);
}
