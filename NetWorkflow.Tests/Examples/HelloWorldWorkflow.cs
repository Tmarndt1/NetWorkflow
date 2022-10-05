
namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<int>
    {
        private readonly int _age;

        public HelloWorldWorkflow(int age)
        {
            _age = age;
        }

        public override IWorkflowBuilder<int> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new HelloWorld())
                    .Then(() => new HelloWorld2(_age));
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
