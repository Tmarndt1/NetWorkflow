# NetWorkflow

Fluent .NET Workflow Library that allows a user to explicitly define their workflow steps in a single location with compile time validation.

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## HelloWorld Workflow Example

```csharp

public class HelloWorldWorkflow : Workflow<bool>
{
    private const string _helloWorld = "HelloWorld";

    public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
        builder
            .StartWith(() => new HelloWorld())
                .Then(() => new GoodByeWorld());

    private class HelloWorld : IWorkflowStep<string>
    {
        public string Run(CancellationToken token = default)
        {
            return _helloWorld;
        }
    }

    private class GoodByeWorld : IWorkflowStep<string, bool>
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
                    .Retry(3) // Retry 3 times
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
            .Parallel(() => new IWorkflowStepAsync<string[], string>[]
            {
                new AsyncStep3(),
                new AsyncStep4()
            });


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

## Authors

- **Travis Arndt**

## License

This project is licensed under the MIT License - [LICENSE.md](LICENSE)
