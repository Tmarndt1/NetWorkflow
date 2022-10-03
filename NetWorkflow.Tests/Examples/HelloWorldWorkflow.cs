
namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<int, int>
    {
        public HelloWorldWorkflow(int context) : base(context)
        {
        }

        public override IWorkflowBuilder<int, int> Build(IWorkflowBuilder<int> builder) =>
            builder
                .StartWith(ctx => new HelloWorld())
                    .Then(ctx => new HelloWorld2(ctx));
    }

    public class HelloWorld : IWorkflowStep<string>
    {
        public string Run(CancellationToken token = default)
        {
            return "Hello World";
        }
    }

    public class HelloWorld2 : IWorkflowStep<string, int>
    {
        private readonly int _age;

        public HelloWorld2(int age)
        {
            _age = age;
        }

        public int Run(string args, CancellationToken token = default)
        {
            return _age;
        }
    }
}
