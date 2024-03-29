# NetWorkflow

NetWorkflow is a light weight C# .NET Workflow library that leverages the fluent syntax and allows a user to explicitly define their WorkflowSteps in a single location with compile time validation between WorkflowStep inputs and outputs.

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## HelloWorld Workflow Example

```csharp

using NetWorkflow;

public class HelloWorldWorkflow : Workflow<bool>
{
    private const string _helloWorld = "HelloWorld";

    public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
        builder
            .StartWith(() => new HelloWorld())
                .Then(() => new GoodbyeWorld());

    private class HelloWorld : IWorkflowStep<string>
    {
        public string Run(CancellationToken token = default)
        {
            return _helloWorld;
        }
    }

    private class GoodbyeWorld : IWorkflowStep<string, bool>
    {
        public bool Run(string args, CancellationToken token = default)
        {
            return args == _helloWorld;
        }
    }
}

```

## Conditional Workflow Example

```csharp

using NetWorkflow;

public class ConditionalWorkflow : Workflow<int>
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
                .Else()
                    .Retry(TimeSpan.FromSeconds(10), 2) // Max retry 2 times
            .EndIf()
                .Then(() => new FinalStep());


    // Example WorkflowStep
    private class FirstStep : IWorkflowStep<string>
    {
        private readonly string _message;

        internal FirstStep(string message)
        {
            _message = message;
        }

        public string Run(CancellationToken token = default)
        {
            return _message;
        }
    }

    // Other WorkflowSteps are omitted to simplify example!
}

// Implicit cast to an int
int result = new ConditionalWorkflow().Run();

```

## Parallel Workflow Example

```csharp

using NetWorkflow;

public class ParallelWorkflow : Workflow<string[]>
{
    public override IWorkflowBuilder<string[]> Build(IWorkflowBuilder builder) =>
        builder
            .StartWith(() => new Step1())
                .Parallel(() => new IWorkflowStepAsync<Guid, string>[]
                {
                    new AsyncStep1(),
                    new AsyncStep2()
                })
                .ThenAsync(() => new AsyncStep3())
                    .ThenAsync(() => AsyncStep4());


    // Example Async WorkflowStep
    private class AsyncStep1 : IWorkflowStepAsync<Guid, string[]>
    {
        public Task<string[]> RunAsync(Guid args, CancellationToken token = default)
        {
            return Task.Delay(500, token).ContinueWith(t => new string[] 
            {
                "Hello",
                "World"
            }, token);
        }
    }

    // Other WorkflowSteps are omitted to simplify example!
}

// Implicit cast to a string array
string[] results = new ParallelWorkflow()
    .Run(new CancellationTokenSource().Token);

```

## WorkflowScheduler AtFrequency Example

```csharp

using NetWorkflow.Scheduler;

var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
{
    // Will execute every 10 seconds until the Workflow has been executed 5 times.
    config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromSeconds(10)).Until(5);
    config.OnExecuted = (WorkflowResult<bool> result) => 
    {
        Console.WriteLine($"Workflow result: {result.Output}");
    }
});

CancellationTokenSource tokenSource = new CancellationTokenSource();

_ = scheduler.StartAsync(tokenSource.Token);

```

## WorkflowScheduler AtDateTime Example

```csharp

using NetWorkflow.Scheduler;

var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
{
    config.ExecuteAt = WorkflowTime.AtMinute(DateTime.Now.Minute);
    config.OnExecuted = (WorkfowResult<bool> result) => 
    {
        Console.WriteLine($"Workflow result: {result.Output}");
    }
});

CancellationTokenSource tokenSource = new CancellationTokenSource();

_ = scheduler.StartAsync(tokenSource.Token);

```

## Authors

- **Travis Arndt**

## License

This project is licensed under the MIT License - [LICENSE.md](LICENSE)
