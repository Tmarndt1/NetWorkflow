# NetWorkflow

Fluent .NET Workflow Library

## Parallel Workflow Example

```csharp

public class ParallelWorkflow : Workflow<object, string[]>
{
    public ParallelWorkflow(object context) : base(context)
    {
    }

    public override IWorkflowBuilderNext<object, string[]> Build(IWorkflowBuilderInitial<object> builder) =>
        builder.StartWith(ctx => new Step1())
            .Parallel(ctx => new WorkflowStepAsync<Guid, string>[]
            {
                new Step2(1000),
                new Step2(2000)
            })
            .Parallel(ctx => new WorkflowStepAsync<string[], string>[]
            {
                new Step3(),
                new Step4()
            });
}

var workflow = new ParallelWorkflow(1991);

```

## Authors

- **Travis Arndt**

## License

This project is licensed under the MIT License - [LICENSE.md](LICENSE)
