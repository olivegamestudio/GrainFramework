namespace GrainFramework;

/// <summary>
/// Deterministic runtime that hosts grains, routes messages, and drains outputs.
/// </summary>
public sealed class GrainRuntime : IGrainRuntime
{
    private readonly Dictionary<GrainId, IGrain> _grains = new();

    /// <inheritdoc />
    public void Register(IGrain grain)
    {
        if (_grains.ContainsKey(grain.Id))
            throw new InvalidOperationException($"Grain already registered: {grain.Id}");

        _grains.Add(grain.Id, grain);

        if (grain is IGrainLifecycle lifecycle)
            lifecycle.OnActivated();
    }

    /// <inheritdoc />
    public bool Contains(GrainId id)
        => _grains.ContainsKey(id);

    /// <inheritdoc />
    public bool TryGet(GrainId id, out IGrain grain)
        => _grains.TryGetValue(id, out grain!);

    /// <inheritdoc />
    public void Enqueue(GrainId target, IGrainMessage message)
    {
        if (!_grains.TryGetValue(target, out var grain))
            throw new KeyNotFoundException($"Grain not found: {target}");

        grain.Enqueue(message);
    }

    /// <inheritdoc />
    public void TickAll(TimeSpan deltaTime)
    {
        // Deterministic ordering
        foreach (var grain in _grains.Values.OrderBy(g => g.Id.Value))
        {
            grain.Tick(deltaTime);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IGrainOutput> DrainAllOutputs()
    {
        var outputs = new List<IGrainOutput>();

        foreach (var grain in _grains.Values.OrderBy(g => g.Id.Value))
        {
            outputs.AddRange(grain.DrainOutputs());
        }

        return outputs;
    }
}
