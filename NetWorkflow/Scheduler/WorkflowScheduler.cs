using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The WorkflowScheduler is a generic workflow scheduler that is responsible for scheduling and executing workflows.
    /// </summary>
    /// <typeparam name="TWorkflow">The Workflow to executed.</typeparam>
    public class WorkflowScheduler<TWorkflow, TResult> : IDisposable
        where TWorkflow : IWorkflow<TResult>
    {
        // Declare the event.
        public event EventHandler<TResult> OnExecuted = delegate { };

        private Func<TWorkflow> _workflowFactory;

        private WorkflowSchedulerConfiguration _configuration = new WorkflowSchedulerConfiguration();

        private int _count = 0;

        private bool _disposedValue;

        /// <summary>
        /// Designates the WorkflowScheduler to use the function to create the new Workflow.
        /// </summary>
        /// <param name="workflowFactory">A function that returns a Workflow.</param>
        /// <returns>The same instance of the WorkflowScheduler.</returns>
        public WorkflowScheduler(Func<TWorkflow> workflowFactory, Action<WorkflowSchedulerConfiguration> configuration)
        {
            if (workflowFactory == null) throw new ArgumentNullException(nameof(workflowFactory), "The WorkflowScheduler requires a workflow factory.");

            if (configuration == null) throw new ArgumentNullException(nameof(configuration), "The WorkflowScheduler requires a configuration to define when to execute thew workflow.");

            _workflowFactory = workflowFactory;

            configuration.Invoke(_configuration);
        }

        /// <summary>
        /// Starts the WorkflowScheduler and returns the Task/Thread the WorkflowScheduler is running on.
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the request.</param>
        /// <returns>A long running Task until canceled.</returns>
        public Task StartAsync(CancellationToken token = default)
        {
            if (_workflowFactory == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowScheduler<TWorkflow, TResult>)} requires a Workflow Factory function.");
            }

            // A user should specify a WorkflowScheduler to execute a specific frequency or time.
            if (_configuration.ExecuteAt == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowSchedulerConfiguration.ExecuteAt)} has not been set.");
            }

            return Task.Run(() =>
            {
                if (_configuration.ExecuteAt is WorkflowFrequency workflowFrequency)
                {
                    ExecuteAsync(workflowFrequency, token).Wait();
                }
                else if (_configuration.ExecuteAt is WorkflowDateTime workflowDateTime)
                {
                    ExecuteAsync(workflowDateTime, token).Wait();
                }
                else
                {
                    throw new InvalidOperationException("Invalid workflow execution configuration.");
                }
            }, token);
        }

        private async Task ExecuteAsync(WorkflowFrequency workflowFrequency, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(workflowFrequency.Frequency, token);

                OnExecuted.Invoke(this, _workflowFactory.Invoke().Run(token));

                if (++_count == workflowFrequency.ExecutionCount) break;
            }
        }

        private async Task ExecuteAsync(WorkflowDateTime workflowDateTime, CancellationToken token)
        {
            while (!token.IsCancellationRequested && !_disposedValue)
            {
                if (workflowDateTime.IsNow())
                {
                    OnExecuted.Invoke(this, _workflowFactory.Invoke().Run(token));

                    if (++_count == workflowDateTime.ExecutionCount) break;
                }

                await Task.Delay(60000, token); // Delay 1 minute
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _workflowFactory = null;
                    _configuration = null;
                }

                _disposedValue = true;
            }
        }

        ~WorkflowScheduler()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
