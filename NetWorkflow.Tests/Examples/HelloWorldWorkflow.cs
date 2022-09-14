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

    public class HelloWorldWorkflow2 : Workflow<int, string[]>
    {
        public HelloWorldWorkflow2(int context) : base(context)
        {
        }

        public override IWorkflowBuilderNext<int, string[]> Build(IWorkflowBuilder<int> builder) =>
            builder.StartWith(ctx => new HelloWorld())
                .Then(ctx => new HelloWorld2(ctx))
                .Parallel(ctx => new WorkflowStepAsync<int, string>[]
                {
                    new HelloWorld3(1000),
                    new HelloWorld3(2000)
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
        private readonly int _delay;

        public HelloWorld3(int delay)
        {
            _delay = delay;
        }

        public override Task<string> RunAsync(int args, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(_delay);

                return $"{nameof(HelloWorld3)} ran";
            });
        }
    }
}
