using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NetWorkflow.Extensions
{
    internal sealed class WorkflowRunner<TWorkflow, TResult> : IWorkflowRunner<TWorkflow, TResult>
        where TWorkflow : IWorkflow<TResult>
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkflowRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task<WorkflowResult<TResult>> RunAsync(CancellationToken token = default)
        {
            var workflow = _serviceProvider.GetRequiredService<TWorkflow>();

            return workflow.RunAsync(token);
        }
    }
}
