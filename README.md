# GrainFramework

Lightweight abstractions for building tick-driven grains/actors with typed messages, outputs, and persistable state.

## What it provides
- `GrainId` strongly typed identifier for routing and logging.
- `IGrain` core contract with `Enqueue`, `Tick`, and `DrainOutputs` for deterministic stepping.
- Marker interfaces for your domain types: `IGrainMessage`, `IGrainEvent`, `IGrainOutput`, and `IGrainState`.
- `IPersistedGrain` adds `CaptureState`/`RestoreState` so grains can snapshot and reload state.

## Install
- From source: `dotnet add reference GrainFramework.csproj`
- From NuGet (when published): `dotnet add package GrainFramework`

## Quick start
```csharp
using GrainFramework;
using System;
using System.Collections.Generic;

public sealed class CounterGrain : IPersistedGrain
{
    private readonly Queue<IGrainMessage> _inbox = new();
    private readonly List<IGrainOutput> _outputs = new();
    private int _count;

    public CounterGrain(GrainId id) => Id = id;

    public GrainId Id { get; }

    public void Enqueue(IGrainMessage message) => _inbox.Enqueue(message);

    public void Tick(TimeSpan deltaTime)
    {
        while (_inbox.TryDequeue(out var message))
        {
            if (message is Increment increment)
                _count += increment.Amount;
        }

        _outputs.Add(new CounterChanged(_count));
    }

    public IReadOnlyList<IGrainOutput> DrainOutputs()
    {
        var snapshot = new List<IGrainOutput>(_outputs);
        _outputs.Clear();
        return snapshot;
    }

    public IGrainState CaptureState() => new CounterState(_count);

    public void RestoreState(IGrainState state) =>
        _count = ((CounterState)state).Value;
}

public sealed record Increment(int Amount) : IGrainMessage;
public sealed record CounterChanged(int Value) : IGrainOutput;
public sealed record CounterState(int Value) : IGrainState;
```

Call `Enqueue` to stage work, step the grain via `Tick(deltaTime)`, then read external effects from `DrainOutputs()`. If persistence is required, capture a snapshot before shutdown and call `RestoreState` when rebuilding the grain.

## Development
- Build locally: `dotnet build`

## License
MIT
