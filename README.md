# NetWorkflow

NetWorkflow is a lightweight C#/.NET workflow library with a fluent API for defining workflow steps in one place while preserving compile-time validation between step inputs and outputs.

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Hello World Workflow

```csharp
using NetWorkflow;

public sealed class HelloWorldWorkflow : Workflow<bool>
{
    private const string HelloWorld = "HelloWorld";

    public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
        builder
            .StartWith(() => new HelloWorldStep())
            .Then(() => new GoodbyeWorldStep());

    private sealed class HelloWorldStep : IWorkflowStep<string>
    {
        public string Run(CancellationToken token = default)
        {
            return HelloWorld;
        }
    }

    private sealed class GoodbyeWorldStep : IWorkflowStep<string, bool>
    {
        public bool Run(string args, CancellationToken token = default)
        {
            return args == HelloWorld;
        }
    }
}

WorkflowResult<bool> result = new HelloWorldWorkflow().Run();

if (result.Status == WorkflowResultStatus.Completed)
{
    bool output = result.Output;
}
```

## Conditional Workflow

```csharp
using NetWorkflow;

public sealed class ConditionalWorkflow : Workflow<int>
{
    private readonly string _message;

    public ConditionalWorkflow(string message)
    {
        _message = message;
    }

    public override IWorkflowBuilder<int> Build(IWorkflowBuilder builder) =>
        builder
            .StartWith(() => new FirstStep(_message))
            .If(x => x == "Success")
                .Do(() => new ConditionalStep())
            .ElseIf(x => x == "Failed")
                .Do(() => new ConditionalStep())
            .EndIf()
            .Then(() => new FinalStep());

    private sealed class FirstStep : IWorkflowStep<string>
    {
        private readonly string _message;

        public FirstStep(string message)
        {
            _message = message;
        }

        public string Run(CancellationToken token = default)
        {
            return _message;
        }
    }

    private sealed class ConditionalStep : IWorkflowStep<string, string>
    {
        public string Run(string args, CancellationToken token = default)
        {
            return args;
        }
    }

    private sealed class FinalStep : IWorkflowStep<object, int>
    {
        public int Run(object args, CancellationToken token = default)
        {
            return args?.ToString() == "Success" ? 1 : 0;
        }
    }
}

int result = new ConditionalWorkflow("Success").Run();
```

## Parallel Async Workflow

Use `RunAsync()` when workflows contain async or parallel steps and the caller should not block.

```csharp
using NetWorkflow;

public sealed class ParallelWorkflow : Workflow<IEnumerable<string>>
{
    public override IWorkflowBuilder<IEnumerable<string>> Build(IWorkflowBuilder builder) =>
        builder
            .StartWith(() => new CreateIdStep())
            .Parallel(() => new IWorkflowStepAsync<Guid, string>[]
            {
                new FirstAsyncStep(),
                new SecondAsyncStep()
            });

    private sealed class CreateIdStep : IWorkflowStep<Guid>
    {
        public Guid Run(CancellationToken token = default)
        {
            return Guid.NewGuid();
        }
    }

    private sealed class FirstAsyncStep : IWorkflowStepAsync<Guid, string>
    {
        public async Task<string> RunAsync(Guid args, CancellationToken token = default)
        {
            await Task.Delay(500, token);
            return "Hello";
        }
    }

    private sealed class SecondAsyncStep : IWorkflowStepAsync<Guid, string>
    {
        public async Task<string> RunAsync(Guid args, CancellationToken token = default)
        {
            await Task.Delay(500, token);
            return "World";
        }
    }
}

WorkflowResult<IEnumerable<string>> result = await new ParallelWorkflow().RunAsync();
```

## Workflow Results

`WorkflowResult<T>` uses `Status` as the source of truth.

```csharp
WorkflowResult<bool> result = await new HelloWorldWorkflow().RunAsync();

switch (result.Status)
{
    case WorkflowResultStatus.Completed:
        Console.WriteLine(result.Output);
        break;
    case WorkflowResultStatus.Canceled:
        Console.WriteLine("Workflow was canceled.");
        break;
    case WorkflowResultStatus.Faulted:
        Console.WriteLine(result.Exception);
        break;
}
```

For safer output access:

```csharp
if (result.TryGetOutput(out bool output))
{
    Console.WriteLine(output);
}

bool requiredOutput = result.GetOutputOrThrow();
```

## Workflow Options

```csharp
var options = new WorkflowOptions
{
    RethrowExceptions = true
};
```

## Scheduler

### Frequency Schedule

```csharp
using NetWorkflow;
using NetWorkflow.Scheduler;

var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
{
    config.Schedule = WorkflowSchedule.AtFrequency(TimeSpan.FromSeconds(10)).Until(5);
    config.RunImmediately = true;
    config.OnExecuted = result =>
    {
        Console.WriteLine($"Workflow status: {result.Status}");
    };
    config.OnExecutedAsync = (result, token) =>
    {
        Console.WriteLine($"Workflow result: {result.Output}");
        return Task.CompletedTask;
    };
});

using var tokenSource = new CancellationTokenSource();

await scheduler.StartAsync(tokenSource.Token);
```

### Calendar Schedule

```csharp
using NetWorkflow;
using NetWorkflow.Scheduler;

var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
{
    config.Schedule = WorkflowSchedule.AtMinute(DateTime.Now.Minute);
    config.OnExecuted = (WorkflowResult<bool> result) =>
    {
        Console.WriteLine($"Workflow status: {result.Status}");
    };
});

using var tokenSource = new CancellationTokenSource();

_ = scheduler.StartAsync(tokenSource.Token);
```

### Overlap Policy

```csharp
var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
{
    config.Schedule = WorkflowSchedule.AtFrequency(TimeSpan.FromSeconds(10));
    config.OverlapPolicy = WorkflowOverlapPolicy.Wait;
});
```

Available policies:

- `Wait`: wait for each execution to complete before scheduling the next execution.
- `Skip`: skip a due execution when the previous execution is still running.
- `AllowConcurrent`: allow multiple executions to run at the same time.

## Dependency Injection

Install/use `NetWorkflow.Extensions` for Microsoft dependency injection helpers.

```csharp
using Microsoft.Extensions.DependencyInjection;
using NetWorkflow.Extensions;

var services = new ServiceCollection()
    .AddWorkflow<HelloWorldWorkflow, bool>()
    .BuildServiceProvider();

var workflow = services.GetRequiredService<HelloWorldWorkflow>();
var result = await workflow.RunAsync();
```

Register workflow steps with constructor dependencies:

```csharp
using NetWorkflow;
using NetWorkflow.Extensions;

services
    .AddSingleton<IMessageProvider, MessageProvider>()
    .AddWorkflowStep<CreateMessageStep>()
    .AddWorkflow<MessageWorkflow, string>();

public interface IMessageProvider
{
    string GetMessage();
}

public sealed class MessageProvider : IMessageProvider
{
    public string GetMessage()
    {
        return "Hello from DI";
    }
}

public sealed class CreateMessageStep : IWorkflowStep<string>
{
    private readonly IMessageProvider _messageProvider;

    public CreateMessageStep(IMessageProvider messageProvider)
    {
        _messageProvider = messageProvider;
    }

    public string Run(CancellationToken token = default)
    {
        return _messageProvider.GetMessage();
    }
}

public sealed class MessageWorkflow : Workflow<string>
{
    private readonly CreateMessageStep _step;

    public MessageWorkflow(CreateMessageStep step)
    {
        _step = step;
    }

    public override IWorkflowBuilder<string> Build(IWorkflowBuilder builder) =>
        builder.StartWith(() => _step);
}
```

Use a typed workflow runner:

```csharp
var runner = services.GetRequiredService<IWorkflowRunner<HelloWorldWorkflow, bool>>();
WorkflowResult<bool> result = await runner.RunAsync();
```

## ASP.NET Hosted Workflows

Hosted workflows resolve each scheduled workflow execution from a fresh dependency injection scope.

```csharp
using NetWorkflow.Extensions;
using NetWorkflow.Scheduler;

builder.Services.AddHostedWorkflow<HelloWorldWorkflow, bool>(config =>
{
    config.Schedule = WorkflowSchedule.AtFrequency(TimeSpan.FromMinutes(5));
    config.RunImmediately = true;
});
```

## Authors

- **Travis Arndt**

## License

This project is licensed under the MIT License - [LICENSE](LICENSE)
