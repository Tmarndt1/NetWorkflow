namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<int, int>
    {
        public HelloWorldWorkflow(int context) : base(context)
        {
        }

        public override IWorkflowBuilderNext<int, int> Build(IWorkflowBuilder<int> builder) =>
            builder.StartWith(ctx => new HelloWorld())
                .Then(ctx => new HelloWorld2(ctx));
    }

    public class HelloWorldWorkflow2 : Workflow<int, object[]>
    {
        public HelloWorldWorkflow2(int context) : base(context)
        {
        }

        public override IWorkflowBuilderNext<int, object[]> Build(IWorkflowBuilder<int> builder) =>
            builder.StartWith(ctx => new HelloWorld())
                .Then(ctx => new HelloWorld2(ctx))
                .Parallel(ctx => new WorkflowStepAsync<int, string>[]
                {
                    new HelloWorld3()
                });
    }

    public class HelloWorld : WorkflowStep<string>
    {
        public override string Run(CancellationToken token = default)
        {
            return "Hello World";
        }
    }

    public class HelloWorld2 : WorkflowStep<string, int>
    {
        private readonly int _age;

        public HelloWorld2(int context)
        {
            _age = context;
        }

        public override int Run(string args, CancellationToken token = default)
        {
            return _age;
        }
    }

    public class HelloWorld3 : WorkflowStepAsync<int, string>
    {
        public override Task<string> RunAsync(int args, CancellationToken token = default)
        {
            return Task.FromResult($"{nameof(HelloWorld3)} ran");
        }
    }
}
