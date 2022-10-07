
namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<bool>
    {
        private readonly bool _success;

        public HelloWorldWorkflow(bool success)
        {
            _success = success;
        }

        public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new HelloWorld())
                    .Then(() => new HelloWorld2(_success));
    }

    public class HelloWorld : IWorkflowStep<string>
    {
        public string Run(CancellationToken token = default)
        {
            return "Hello World";
        }
    }

    public class HelloWorld2 : IWorkflowStep<string, bool>
    {
        private readonly bool _success;

        public HelloWorld2(bool success)
        {
            _success = success;
        }

        public bool Run(string args, CancellationToken token = default)
        {
            return _success;
        }
    }
}
