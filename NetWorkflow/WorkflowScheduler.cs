using System.Reflection;
using System.Timers;

namespace NetWorkflow
{
    public class WorkflowScheduler<TWorkflow>
        where TWorkflow : IWorkflow
    {
        private Func<TWorkflow> _workflowFactory;

        private readonly WorkflowSchedulerOptions _options = new WorkflowSchedulerOptions();

        private Task _runningTask;

        private CancellationTokenSource _tokenSource;

        private MethodInfo _executingMethod;

        private MethodInfo _executingMethodAsync;

        /// <summary>
        /// Designates the WorkflowScheduler to use the function to create the new Workflow.
        /// </summary>
        /// <param name="workflowFactory">A function that returns a Workflow.</param>
        /// <returns>The same instance of the WorkflowScheduler.</returns>
        public WorkflowScheduler<TWorkflow> Use(Func<TWorkflow> workflowFactory)
        {
            _workflowFactory = workflowFactory;

            _executingMethod = typeof(TWorkflow).GetMethod("Run");

            _executingMethodAsync = typeof(TWorkflow).GetMethod("RunAsync");

            return this;
        }

        /// <summary>
        /// Configures the WorkflowScheduler based on the values set within the provided WorkflowSchedulerOptions.
        /// </summary>
        /// <param name="configureOptions">An action provides WorkflowSchedulerOptions to configure.</param>
        /// <returns>The same instance of the WorkflowScheduler.</returns>
        public WorkflowScheduler<TWorkflow> Configure(Action<WorkflowSchedulerOptions> configureOptions)
        {
            configureOptions.Invoke(_options);

            return this;
        }

        /// <summary>
        /// Starts the WorkflowScheduler and returns the Task/Thread the WorkflowScheduler is running on.
        /// </summary>
        /// <returns>A long running Task.</returns>
        public Task StartAsync()
        {
            _tokenSource = new CancellationTokenSource();

            if (_workflowFactory == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowScheduler<TWorkflow>)} requires a Workflow Factory function provided in the {nameof(WorkflowScheduler<TWorkflow>.Use)} method");
            }

            // A user should specify a WorkflowScheduler to execute a specific frequency or time.
            if (!_options._frequencySet && !_options._atTimeSet)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowSchedulerOptions.Frequency)} or {nameof(WorkflowSchedulerOptions.AtTime)} has not been set.");
            }

            _runningTask = Task.Run(async () =>
            {
                if (_options._frequencySet)
                {
                    while (!_tokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(_options.Frequency).ContinueWith(t =>
                        {
                            if (_options.ExecuteAsync)
                            {
                                _executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                            }
                            else
                            {
                                _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                            }
                        });
                    }
                }
                else if (_options._atTimeSet)
                {
                    while (!_tokenSource.IsCancellationRequested)
                    {
                        if (_options.AtTime > DateTime.Now.TimeOfDay)
                        {
                            if (_options.ExecuteAsync)
                            {
                                _executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                            }
                            else
                            {
                                _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                            }

                            // Sleep a second after 
                            await Task.Delay(1000);
                        }
                    }
                }

            }, _tokenSource.Token);

            return _runningTask;
        }

        /// <summary>
        /// Stops the WorkflowScheduler by cancelling the CancellationToken that is passed in to each instance of the Workflow.
        /// </summary>
        public void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}
