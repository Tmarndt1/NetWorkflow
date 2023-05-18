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
        /// <returns>A long running Task.</returns>
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

            return Task.Run(async () =>
            {
                if (_configuration?.ExecuteAt is WorkflowFrequency workflowFrequency)
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(workflowFrequency.Frequency);

                        OnExecuted.Invoke(this, _workflowFactory.Invoke().Run(token));

                        _count++;

                        if (workflowFrequency.ExecutionCount == _count) break;
                    }
                }
                else if (_configuration?.ExecuteAt is WorkflowDateTime workflowDateTime)
                {
                    // Note: Similer code being executed below. If separated out into a reusable async function that 
                    // would make so another state machine would be created and have to be awaited. Therefore to reduce the memory allocation
                    // the code to execute the Workflow was written within each WorkflowTime check.

                    if (workflowDateTime.Day != -1)
                    {
                        while (!token.IsCancellationRequested && !_disposedValue)
                        {
                            DateTime date = DateTime.Now;

                            if (date.Day == workflowDateTime.Day && date.Hour == workflowDateTime.Hour && date.Minute == workflowDateTime.Minute)
                            {
                                OnExecuted.Invoke(this, _workflowFactory.Invoke().Run(token));

                                _count++;

                                if (workflowDateTime.ExecutionCount == _count) break;

                                await Task.Delay(60000); // Delay 1 minute
                            }
                        }
                    }
                    else if (workflowDateTime.Hour != -1)
                    {
                        while (!token.IsCancellationRequested && !_disposedValue)
                        {
                            DateTime date = DateTime.Now;

                            if (date.Hour == workflowDateTime.Hour && date.Minute == workflowDateTime.Minute)
                            {
                                OnExecuted.Invoke(this, _workflowFactory.Invoke().Run(token));

                                _count++;

                                if (workflowDateTime.ExecutionCount == _count) break;

                                await Task.Delay(60000); // Delay 1 minute
                            }
                        }
                    }
                    else if (workflowDateTime.Minute != -1)
                    {
                        while (!token.IsCancellationRequested && !_disposedValue)
                        {
                            if (DateTime.Now.Minute == workflowDateTime.Minute)
                            {
                                OnExecuted.Invoke(this, _workflowFactory.Invoke().Run(token));

                                _count++;

                                if (workflowDateTime.ExecutionCount == _count) break;

                                await Task.Delay(60000); // Delay 1 minute
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("A WorkflowTime must specificy to run at the Day, Hour, or Minute mark");
                    }
                }

            }, token);
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
