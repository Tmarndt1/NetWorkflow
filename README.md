# NetWorkflow

NetWorkflow is a powerful and flexible workflow library for .NET that enables you to define and execute complex workflows with ease. It provides a fluent and intuitive API for building workflows, supporting sequential, conditional, parallel, and scheduled execution of workflow steps.

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Features

* **Sequential Execution:** Easily define and execute workflows with a sequence of steps.
* **Conditional Branching:** Create conditional branches based on runtime values and conditions.
* **Parallel Execution:** Execute multiple workflow steps in parallel, improving efficiency and performance.
* **Asynchronous Workflows:** Seamlessly integrate asynchronous workflows for handling time-consuming operations.
* **Workflow Scheduling:** Schedule workflows to execute at specific times or frequencies.
* **Flexible Configuration:** Configure execution options and event hooks to tailor workflows to your specific needs.

## Installation
### NuGet Package Manager
Open the NuGet Package Manager console in Visual Studio.

Run the following command:
```
dotnet add package NetWorkflow
```

## Getting Started
To get started with NetWorkflow, follow these steps:

1. Define your workflow by inheriting from the appropriate base class (Workflow<T> or AsyncWorkflow<T>).
2. Implement the Build method to configure the workflow steps using the fluent API.
3. Execute the workflow using the Run or RunAsync method.

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

## Contributing

Contributions to the NetWorkflow library are highly appreciated! If you find any issues or have suggestions for improvements, please create a new issue or submit a pull request.

## Authors

- **Travis Arndt**

## License

This project is licensed under the MIT License - [LICENSE.md](LICENSE)
