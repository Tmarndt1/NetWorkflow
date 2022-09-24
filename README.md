# NetWorkflow

Fluent .NET Workflow Library

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Conditional Workflow Example

```csharp

public class ConditionalWorkflow : Workflow<object, int>
{
    public ConditionalWorkflow(object context) : base(context) { }

    public override IWorkflowBuilder<object, int> Build(IWorkflowBuilder<object> builder) =>
        builder
            .StartWith(() => new FirstStep())
            .If(x => x == "Success")
                .Do(() => new ConditionalStep(1))
            .ElseIf(x => x == "Failed")
                .Do(() => new ConditionalStep(-1))
            .EndIf()
                .Then(() => new FinalStep());
}

int result = new ConditionalWorkflow(new object())
    .Run(new CancellationToken());

```

## Parallel Workflow Example

```csharp

public class ParallelWorkflow : Workflow<object, string[]>
{
    public ParallelWorkflow(object context) : base(context) { }

    public override IWorkflowBuilder<object, string[]> Build(IWorkflowBuilder<object> builder) =>
        builder
            .StartWith(() => new Step1())
            .Parallel(() => new WorkflowStepAsync<Guid, string>[]
            {
                new Step2(1000),
                new Step2(2000)
            })
            .Parallel(() => new WorkflowStepAsync<string[], string>[]
            {
                new Step3(),
                new Step4()
            });
}

string[] results = new ParallelWorkflow(new object())
    .Run(new CancellationToken());

```

## Authors

- **Travis Arndt**

## License

This project is licensed under the MIT License - [LICENSE.md](LICENSE)
