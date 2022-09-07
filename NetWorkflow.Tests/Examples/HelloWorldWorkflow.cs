namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<int, int>
    {
        public HelloWorldWorkflow(int context) : base(context)
        {
        }

        public override void Build(IWorkflowBuilder<int> builder) =>
            builder.StartWith(ctx => new HelloWorld())
                .Then(ctx => new HelloWorld2(ctx));
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
}
