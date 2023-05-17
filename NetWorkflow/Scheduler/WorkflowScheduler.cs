using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The WorkflowScheduler is a generic workflow scheduler that is responsible for scheduling and executing workflows.
    /// </summary>
    /// <typeparam name="TWorkflow">The Workflow to executed.</typeparam>
    public class WorkflowScheduler<TWorkflow, Tout> : IDisposable
        where TWorkflow : IWorkflow<Tout>
    {
        private Func<TWorkflow> _workflowFactory;

        private WorkflowSchedulerConfiguration _configuration = new WorkflowSchedulerConfiguration();

        private MethodInfo _executingMethod;

        private bool _disposedValue;

        /// <summary>
        /// Designates the WorkflowScheduler to use the function to create the new Workflow.
        /// </summary>
        /// <param name="workflowFactory">A function that returns a Workflow.</param>
        /// <returns>The same instance of the WorkflowScheduler.</returns>
        public WorkflowScheduler(Func<TWorkflow> workflowFactory)
        {
            _workflowFactory = workflowFactory;

            _executingMethod = typeof(TWorkflow).GetMethod(nameof(IWorkflow<TWorkflow>.Run));
        }

        /// <summary>
        /// Configures the WorkflowScheduler based on the values set within the provided WorkflowSchedulerOptions.
        /// </summary>
        /// <param name="configuration">An action provides WorkflowSchedulerOptions to configure.</param>
        /// <returns>The same instance of the WorkflowScheduler.</returns>
        public WorkflowScheduler<TWorkflow, Tout> Configure(Action<WorkflowSchedulerConfiguration> configuration)
        {
            configuration.Invoke(_configuration);

            return this;
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
                throw new InvalidOperationException($"A {nameof(WorkflowScheduler<TWorkflow, Tout>)} requires a Workflow Factory function.");
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
                    var workflow = _workflowFactory.Invoke();
                    while (!token.IsCancellationRequested && !_disposedValue)
                    {
                        DateTime now = DateTime.Now;
                        _executingMethod.Invoke(workflow, new object[] { token });
                        if (token.IsCancellationRequested || _disposedValue) { break; }
                        var executionTime = (DateTime.Now.Subtract(now));
                        await Task.Delay(workflowFrequency.Frequency - executionTime);
                    }
                }
                else if (_configuration?.ExecuteAt is WorkflowDateTime workflowDateTime)
                {
                    // Note: Similar code being executed below. If separated out into a reusable async function that 
                    // would make so another state machine would be created and have to be awaited. Therefore to reduce the memory allocation
                    // the code to execute the Workflow was written within each WorkflowTime check.

                    if (workflowDateTime.Day != -1)
                    {
                        while (!token.IsCancellationRequested && !_disposedValue)
                        {
                            DateTime date = DateTime.Now;

                            if (date.Day == workflowDateTime.Day && date.Hour == workflowDateTime.Hour && date.Minute == workflowDateTime.Minute)
                            {
                                _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { token });

                                if (!workflowDateTime.Indefinitely || token.IsCancellationRequested || _disposedValue) break;

                                await Task.Delay(TimeSpan.FromMinutes(1)); // Delay 1 minute
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
                                _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { token });

                                if (!workflowDateTime.Indefinitely || token.IsCancellationRequested || _disposedValue) break;

                                await Task.Delay(TimeSpan.FromMinutes(1)); // Delay 1 minute
                            }
                        }
                    }
                    else if (workflowDateTime.Minute != -1)
                    {
                        while (!token.IsCancellationRequested && !_disposedValue)
                        {
                            if (DateTime.Now.Minute == workflowDateTime.Minute)
                            {
                                _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { token });

                                if (!workflowDateTime.Indefinitely || token.IsCancellationRequested || _disposedValue) break;

                                await Task.Delay(TimeSpan.FromMinutes(1)); // Delay 1 minute
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("A WorkflowTime must specify to run at the Day, Hour, or Minute mark");
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
                    _executingMethod = null;
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
