using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetWorkflow.Scheduler;

namespace NetWorkflow.Extensions
{
    internal sealed class HostedWorkflowService<TWorkflow, TResult> : BackgroundService
        where TWorkflow : class, IWorkflow<TResult>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Action<WorkflowSchedulerConfiguration<TResult>> _configuration;

        public HostedWorkflowService(
            IServiceScopeFactory serviceScopeFactory,
            Action<WorkflowSchedulerConfiguration<TResult>> configuration)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scheduler = new WorkflowScheduler<TWorkflow, TResult>(
                RunWorkflowInScopeAsync,
                _configuration,
                WorkflowSchedulerRunnerMode.CustomRunner);

            return scheduler.StartAsync(stoppingToken);
        }

        private async Task<WorkflowResult<TResult>> RunWorkflowInScopeAsync(CancellationToken token)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var workflow = scope.ServiceProvider.GetRequiredService<TWorkflow>();

            return await workflow.RunAsync(token).ConfigureAwait(false);
        }
    }
}
