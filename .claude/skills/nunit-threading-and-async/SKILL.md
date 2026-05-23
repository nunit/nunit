---
name: nunit-threading-and-async
description: Use when writing or modifying async code in NUnit — async test lifecycle (setup/teardown), `TestExecutionContext`, `AsyncLocal`, `Task`/`ValueTask` continuations, blocking vs non-blocking waits, or conditional `Thread.Abort` code. Covers the threading/async conventions the NUnit runtime depends on.
---

# NUnit threading and async

NUnit's test execution model mixes synchronous and asynchronous tests in the same runner, relies on `AsyncLocal` state that follows the logical call flow, and must work on target frameworks with and without `Thread.Abort`. Small mistakes here (blocking in async, capturing the wrong context, feature-gating on the wrong symbol) produce intermittent test-runner failures that are hard to diagnose.

## No blocking in async code

Inside an `async` method or an async test, never do synchronous waits. They consume a thread-pool thread for the duration of the wait and can deadlock when running under a synchronization context.

```csharp
// Wrong — blocks a thread
public async Task PollsUntilReady()
{
    Thread.Sleep(100);
    var ready = GetStatusAsync().Result;
    ...
}

// Right — yields the thread
public async Task PollsUntilReady()
{
    await Task.Delay(100);
    var ready = await GetStatusAsync();
    ...
}
```

Same rule for `.Wait()`, `Task.WaitAll`, `Task.WaitAny`, and `GetAwaiter().GetResult()` in async contexts.

## `TestExecutionContext.CurrentContext` is `AsyncLocal`, not a local

`TestExecutionContext.CurrentContext` reads an `AsyncLocal` slot. It reflects *whatever test is currently running on this logical flow*, not a snapshot of when the surrounding code was written. If you capture it at the wrong moment, you may read the context of the *next* test that starts running on the same thread pool.

```csharp
// Risky — the context at await-resume time may not be the same as at registration time
public void RegisterCallback()
{
    _callback = async () =>
    {
        var ctx = TestExecutionContext.CurrentContext;   // which test's context?
        ...
    };
}

// Safer — capture at the moment you know the right context is active
public void RegisterCallback()
{
    var captured = TestExecutionContext.CurrentContext;
    _callback = async () =>
    {
        var ctx = captured;
        ...
    };
}
```

If you're writing a listener, callback, or anything that fires after an `await` point, capture the context you need up front and close over it.

**Agent Warning:** Do not use `Task.Run()` to artificially wrap synchronous code just to make it async. This breaks NUnit's thread tracking and can cause unhandled exceptions to escape the test context.

## `Thread.Abort` is only available on some target frameworks

`Thread.Abort` exists on .NET Framework and is gone on .NET 6+ (it throws `PlatformNotSupportedException` where still present). Code that depends on it must be feature-gated on the **feature constant**, not the platform:

```csharp
#if THREAD_ABORT
    thread.Abort();
#else
    cancellationTokenSource.Cancel();
#endif
```

`THREAD_ABORT` is defined in `Directory.Build.props` for the `net4*` TFMs. Don't use `#if NETFRAMEWORK` as the gate — feature-gating makes it explicit *why* the branch exists and survives adding or removing TFMs.

## Async test lifecycle

Returning `Task` (or `ValueTask`) from `[Test]`, `[SetUp]`, `[TearDown]`, `[OneTimeSetUp]`, `[OneTimeTearDown]` is supported and awaited by the runtime.

**Do not return `async void`** from NUnit lifecycle methods or from any method where you control the signature. An unobserved exception in `async void` is raised on the synchronization context and crashes the runner rather than failing the test.

The one legitimate exception is an **event handler** whose signature is fixed by the delegate (`EventHandler`, `PropertyChangedEventHandler`, etc.). You can't return `Task` there. If you need to do async work in an event handler, wrap the whole body in `try`/`catch` so the exception can't escape:

```csharp
private async void OnSomethingHappened(object sender, EventArgs e)
{
    try
    {
        await DoAsyncWorkAsync();
    }
    catch (Exception ex)
    {
        // Log, raise a TestListener event, or route into the fixture's state.
        // Never let an exception unwind out of async void.
        _log.Error(ex, "OnSomethingHappened failed");
    }
}
```

If you find yourself wanting `async void` anywhere else, change the signature to `async Task` instead.

## Cancellation and timeouts

Cancellation guidance depends on *where* you're writing the code.

**In tests (test-project code):**
- Respect `CancellationToken` when a method you're calling already takes one; thread it through to the `await`.
- When a test needs to bound its own wait, prefer `CancellationTokenSource.CancelAfter(...)` + pass the token to the awaited call, over wall-clock polling (`Thread.Sleep`, repeated `Task.Delay`).

**In framework internals (`DelayedConstraint`, polling utilities, engine wait loops, anything in `src/NUnitFramework`):**
- **Do not rewrite existing polling or wait mechanisms to use `CancellationToken`.** Several of these have load-bearing timing semantics (poll intervals, final-check-before-fail behaviour, delay-until-quiet) that downstream tests and constraints depend on. Changing the mechanism is a behavioural change, not a refactor.
- Only use token-based bounding when you are **writing new** wait code that has no prior contract, or when the public API already exposes a `CancellationToken` parameter that you need to honour.
- If you genuinely need to modernise an existing polling loop, open a separate PR with benchmarks, not a drive-by refactor.

**On `[Timeout]`:** `[Timeout]` is best-effort and depends on the runtime's ability to interrupt the thread. On .NET Core+, a timed-out async test is *not* forcibly aborted — it is reported as failed and left running in the background. Design tests that use `[Timeout]` with that in mind; if a hang would leak resources, bound the wait inside the test with a token too.

## `ValueTask` vs `Task`

NUnit's public API surface uses `Task`. Internal code may use `ValueTask` for allocation-sensitive hot paths, but only where the result is awaited exactly once. Do not store a `ValueTask` and await it twice, and do not convert a reference-type-returning method to `ValueTask<T>` for style — the memory optimisation only pays off for allocation-heavy sync completions.
