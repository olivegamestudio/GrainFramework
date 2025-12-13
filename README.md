# GrainFramework

Lightweight abstractions for building tick-driven grains/actors with typed messages, outputs, and persistable state, plus a minimal runtime for registering grains and ticking them deterministically. Designed for deterministic loops (games, simulations, headless services) where you want explicit stepping instead of background tasks.

## Why GrainFramework
GrainFramework keeps your loop in the driver's seat: you control every tick, outputs stay deterministic, and replayability is trivial. Messages, outputs, events, and state are all typed so you don't lose intent to stringly APIs. There are no surprise background threads or schedulers, making it a natural fit for games, simulations, and headless services that already have a tight loop. It is tiny - just a handful of interfaces and a runtime you can swap out - and it runs anywhere .NET Standard 2.0 does. Persistence hooks are built in so you can capture and restore grain state without bolting on another abstraction.

## Grain basics
- A grain is a unit of state plus behavior you tick from your host loop.
- Work arrives through `Enqueue(IGrainMessage)`, gets processed in `Tick(deltaTime)`, and is emitted via `DrainOutputs()`.
- Use `IGrainEvent` for internally generated events, and `IGrainOutput` for external effects you forward to your own transports/loggers/UI.
- Implement `IPersistedGrain` when you want the runtime to load and save grain state through an `IGrainStateStore`.

## Interfaces at a glance
- `GrainId` strongly typed identifier for routing/logging.
- `IGrain` core contract with `Enqueue`, `Tick`, and `DrainOutputs`.
- `IGrainRuntime` host for registering grains, routing messages, ticking deterministically, draining all outputs, and saving state for persisted grains.
- `GrainRuntime` default implementation with deterministic ordering by `GrainId.Value` and persistence integration.
- `IGrainLifecycle` optional activation hooks (`OnActivated`, `OnDeactivated`) fired by the runtime.
- Marker interfaces for your domain types: `IGrainMessage`, `IGrainEvent`, `IGrainOutput`.
- `IPersistedGrain` adds `CaptureState` / `RestoreState` plus dirty tracking so grains can snapshot and reload state.
- `IGrainStateStore` abstraction for loading and saving state for persisted grains.

## Install
- From source: `dotnet add reference src/GrainFramework.csproj`
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

    public bool IsDirty { get; private set; }

    public void Enqueue(IGrainMessage message) => _inbox.Enqueue(message);

    public void Tick(TimeSpan deltaTime)
    {
        while (_inbox.TryDequeue(out var message))
        {
            if (message is Increment increment)
            {
                _count += increment.Amount;
                IsDirty = true;
            }
        }

        _outputs.Add(new CounterChanged(_count));
    }

    public IReadOnlyList<IGrainOutput> DrainOutputs()
    {
        var snapshot = new List<IGrainOutput>(_outputs);
        _outputs.Clear();
        return snapshot;
    }

    public object CaptureState() => new CounterState(_count);

    public void RestoreState(object state)
    {
        _count = ((CounterState)state).Value;
        IsDirty = false;
    }

    public void MarkClean() => IsDirty = false;
}

public sealed record Increment(int Amount) : IGrainMessage;
public sealed record CounterChanged(int Value) : IGrainOutput;
public sealed record CounterState(int Value);
```

Call `Enqueue` to stage work, step the grain via `Tick(deltaTime)`, then read external effects from `DrainOutputs()`. If persistence is required, the runtime will restore the grain on register and save it when you call `SaveAll`.

## Runtime host
Use `GrainRuntime` when you want to register multiple grains, route messages by id, tick them in a deterministic order, and gather outputs in one pass. The runtime also restores and saves persisted grain state through an `IGrainStateStore`.
```csharp
public sealed class InMemoryStore : IGrainStateStore
{
    private readonly Dictionary<GrainId, object> _state = new();

    public bool TryLoad(GrainId id, out object state) => _state.TryGetValue(id, out state!);
    public void Save(GrainId id, object state) => _state[id] = state;
}

var runtime = new GrainRuntime(new InMemoryStore());
var grain = new CounterGrain(new GrainId("counter-1"));

runtime.Register(grain); // loads persisted state if present
runtime.Enqueue(grain.Id, new Increment(5));

// drive from your game/sim/main loop or a timer
runtime.TickAll(TimeSpan.FromMilliseconds(16));

foreach (var output in runtime.DrainAllOutputs())
{
    // route to network/UI/logging/etc.
}

runtime.SaveAll(); // persists dirty grains via the store
```

`GrainRuntime` calls `OnActivated` on grains that implement `IGrainLifecycle` when they are registered. `OnDeactivated` is available for hosts that support teardown.

## Persistence hook
`GrainRuntime` will call `RestoreState` for persisted grains during `Register` when the store has data, and `CaptureState` + `MarkClean` via `SaveAll` when grains report `IsDirty`.

## Target frameworks
- netstandard2.0

## Development
- Build locally: `dotnet build`

## License
MIT
