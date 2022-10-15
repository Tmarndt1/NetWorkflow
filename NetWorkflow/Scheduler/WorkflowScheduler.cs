using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Scheduler
{
    public class WorkflowScheduler<TWorkflow>
        where TWorkflow : IWorkflow
    {
        private Func<TWorkflow> _workflowFactory;

        private readonly WorkflowSchedulerConfiguration _configuration = new WorkflowSchedulerConfiguration();

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
        /// <param name="configuration">An action provides WorkflowSchedulerOptions to configure.</param>
        /// <returns>The same instance of the WorkflowScheduler.</returns>
        public WorkflowScheduler<TWorkflow> Configure(Action<WorkflowSchedulerConfiguration> configuration)
        {
            configuration.Invoke(_configuration);

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
            if (_configuration.ExecuteAt == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowSchedulerConfiguration.ExecuteAt)} has not been set.");
            }

            _runningTask = Task.Run(async () =>
            {
                switch (_configuration.ExecuteAt)
                {
                    case WorkflowFrequency workflowFrequency:
                        {
                            while (!_tokenSource.IsCancellationRequested)
                            {
                                await Task.Delay(workflowFrequency.Frequency).ContinueWith(async t =>
                                {
                                    if (_configuration.ExecuteAsync)
                                    {
                                        await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }
                                    else
                                    {
                                        _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }
                                });
                            }

                            break;
                        }
                    // Note: Similer code being executed below. If separated out into a reusable async function that 
                    // would make so another state machine would be created and have to be awaited. Therefore to reduce the memory allocation
                    // the code to execute the Workflow was written within each WorkflowTime check.
                    case WorkflowDateTime workflowDateTime when workflowDateTime.Day != -1:
                        {
                            while (!_tokenSource.IsCancellationRequested)
                            {
                                DateTime date = DateTime.Now;

                                if (date.Day == workflowDateTime.Day && date.Hour == workflowDateTime.Hour && date.Minute == workflowDateTime.Minute)
                                {
                                    if (_configuration.ExecuteAsync)
                                    {
                                        await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }
                                    else
                                    {
                                        _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }

                                    if (!workflowDateTime.Indefinitely) break;

                                    await Task.Delay(60000); // Delay 1 minute
                                }
                            }

                            break;
                        }
                    case WorkflowDateTime workflowDateTime when workflowDateTime.Hour != -1:
                        {
                            while (!_tokenSource.IsCancellationRequested)
                            {
                                DateTime date = DateTime.Now;

                                if (date.Hour == workflowDateTime.Hour && date.Minute == workflowDateTime.Minute)
                                {
                                    if (_configuration.ExecuteAsync)
                                    {
                                        await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }
                                    else
                                    {
                                        _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }

                                    if (!workflowDateTime.Indefinitely) break;

                                    await Task.Delay(60000); // Delay 1 minute
                                }
                            }

                            break;
                        }
                    case WorkflowDateTime workflowDateTime when workflowDateTime.Minute != -1:
                        {
                            while (!_tokenSource.IsCancellationRequested)
                            {
                                if (DateTime.Now.Minute == workflowDateTime.Minute)
                                {
                                    if (_configuration.ExecuteAsync)
                                    {
                                        await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }
                                    else
                                    {
                                        _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { _tokenSource.Token });
                                    }

                                    if (!workflowDateTime.Indefinitely) break;

                                    await Task.Delay(60000); // Delay 1 minute
                                }
                            }

                            break;
                        }
                    case WorkflowDateTime workflowDateTime:
                        throw new InvalidOperationException("A WorkflowTime must specify to run at the Day, Hour, or Minute mark");
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
