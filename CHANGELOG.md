# Changelog

## 4.0.0.0

This release contains breaking API changes and a significant internal execution refactor from `3.0.0.0`.

### Breaking Changes

- `WorkflowResult<TOut>` now uses `Status` as the single workflow state property.
- Removed redundant `WorkflowResult<TOut>` boolean state properties:
  - `IsCompleted`
  - `IsCanceled`
  - `IsFaulted`
  - `IsSuccessful`
  - `HasOutput`
- Added `WorkflowResultStatus` with:
  - `Completed`
  - `Canceled`
  - `Faulted`
- Renamed `WorkflowOptions.Rethrow` to `WorkflowOptions.RethrowExceptions`.

### WorkflowResult

- Added `WorkflowResult<TOut>.Status`.
- Added `WorkflowResult<TOut>.TryGetOutput(out TOut output)`.
- Added `WorkflowResult<TOut>.ThrowIfFaulted()`.
- Added `WorkflowResult<TOut>.GetOutputOrThrow()`.
- Changed `WorkflowResult<TOut>.Message` to be derived from `Status`.
- Made `WorkflowResult<TOut>` internally immutable with get-only public properties.
- Kept `Output`, `Exception`, `Duration`, `Message`, and implicit conversion to `TOut`.

### Execution Behavior

- `Workflow.RunAsync()` now uses an async execution path instead of wrapping synchronous `Run()` in `Task.Run`.
- Async workflow steps are awaited without blocking when using `RunAsync()`.
- Parallel workflow steps now use `Task.WhenAll`.
- Synchronous `Workflow.Run()` remains blocking by design, but parallel steps still execute concurrently underneath.
- Workflows now build a fresh execution chain per run instead of reusing builder state across runs.
- Workflow duration is now measured with `Stopwatch` instead of `DateTime.Now`.

### Scheduler

- `WorkflowScheduler.StartAsync()` now returns the scheduler execution task directly instead of wrapping async work in `Task.Run` and blocking with `.Wait()`.
- Scheduler workflow execution now calls `RunAsync()`.
- Added validation for invalid schedule values:
  - `WorkflowSchedule.AtFrequency(...)` rejects zero or negative durations.
  - `WorkflowSchedule.Until(...)` rejects counts less than one.
- Renamed scheduler configuration:
  - `WorkflowSchedulerConfiguration<TResult>.ExecuteAt` -> `WorkflowSchedulerConfiguration<TResult>.Schedule`
  - `WorkflowTime` -> `WorkflowSchedule`
  - `WorkflowFrequency` -> `FrequencySchedule`
  - `WorkflowDateTime` -> `CalendarSchedule`
- Added `WorkflowSchedulerConfiguration<TResult>.OnExecutedAsync`.
- Added `WorkflowSchedulerConfiguration<TResult>.OnErrorAsync`.
- Added `WorkflowSchedulerConfiguration<TResult>.RunImmediately`.
- Added `WorkflowSchedulerConfiguration<TResult>.OverlapPolicy`.
- Added `WorkflowOverlapPolicy`.
- Added `AddHostedWorkflow<TWorkflow, TResult>(...)` in `NetWorkflow.Extensions` for ASP.NET hosted-service integration with scoped workflow resolution.

### Internal Refactoring

- Replaced the generic sync/async executor contract with a single async-first internal executor contract:
  - `IWorkflowExecutor.RunAsync(object args, CancellationToken token)`
- Removed reflection-based step invocation from normal workflow execution.
- Compiled step factory expressions once during workflow construction.
- Split start-step and sync-step execution into separate internal executors:
  - `StartStepExecutor<TOut>`
  - `SyncStepExecutor<TIn, TOut>`
- Renamed internal executors:
  - `WorkflowStepAsyncExecutor<TIn, TOut>` -> `AsyncStepExecutor<TIn, TOut>`
  - `WorkflowParallelExecutor<TIn, TOut>` -> `ParallelStepExecutor<TIn, TOut>`
  - `WorkflowExecutorConditional<TIn>` -> `ConditionalExecutor<TIn>`
  - `WorkflowMoveNextExecutor` -> `PassThroughExecutor`
- Renamed internal builder/runtime node classes:
  - `WorkflowBuilder<TIn, TOut>` -> `WorkflowExecutionNode<TIn, TOut>`
  - `WorkflowBuilderConditional<TIn>` -> `ConditionalWorkflowBuilder<TIn>`
- Removed unnecessary finalizers from builder and scheduler types.

### Tests

- Added coverage for true async parallel workflow execution.
- Added coverage for `WorkflowResultStatus` and result helper methods.
- Added coverage for invalid scheduler frequency and execution count values.
- Updated existing tests to use `WorkflowResult<TOut>.Status`.

### Migration Notes

Replace result boolean checks with `Status` checks:

```csharp
// 3.0.0.0
if (result.IsCompleted)
{
    var output = result.Output;
}

// 4.0.0.0
if (result.Status == WorkflowResultStatus.Completed)
{
    var output = result.Output;
}
```

Replace `WorkflowOptions.Rethrow` with `WorkflowOptions.RethrowExceptions`:

```csharp
// 3.0.0.0
new WorkflowOptions { Rethrow = true };

// 4.0.0.0
new WorkflowOptions { RethrowExceptions = true };
```

Prefer `RunAsync()` when workflows contain async or parallel steps and the caller should not block:

```csharp
WorkflowResult<TOut> result = await workflow.RunAsync(token);
```

Use `GetOutputOrThrow()` or `TryGetOutput(...)` when the caller wants safer output access:

```csharp
if (result.TryGetOutput(out var output))
{
    // Use output.
}
```
