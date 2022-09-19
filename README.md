# NetWorkflow

Fluent .NET Workflow Library

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Parallel Workflow Example

```csharp

public class ParallelWorkflow : Workflow<object, string[]>
{
    public ParallelWorkflow(object context) : base(context)
    {
    }

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
