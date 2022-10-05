# NetWorkflow

Fluent .NET Workflow Library that allows a user to explicitly define their workflow steps in a single location with compile time validation.

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Conditional Workflow Example

```csharp

public class ConditionalWorkflow : Workflow<int>
{
    public override IWorkflowBuilder<int> Build(IWorkflowBuilder builder) =>
        builder
            .StartWith(() => new FirstStep())
                .If(x => x == "Success")
                    .Do(() => new ConditionalStep())
                .ElseIf(x => x == "Failed")
                    .Do(() => new ConditionalStep())
                .Else()
                    .Retry(3)
            .EndIf()
                .Then(() => new FinalStep());
}

var tokenSource = new CancellationTokenSource();

int result = new ConditionalWorkflow(new object())
    .Run(tokenSource.Token);

```

## Parallel Workflow Example

```csharp

public class ParallelWorkflow : Workflow<string[]>
{
    public ParallelWorkflow(object context) { }

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
}

var tokenSource = new CancellationTokenSource();

string[] results = new ParallelWorkflow(new object())
    .Run(tokenSource.Token);

```

## Authors

- **Travis Arndt**

## License

This project is licensed under the MIT License - [LICENSE.md](LICENSE)
