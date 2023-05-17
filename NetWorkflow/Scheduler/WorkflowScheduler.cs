using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Scheduler
{
    public class WorkflowScheduler<TWorkflow> : IDisposable
        where TWorkflow : IWorkflow
    {
        private Func<TWorkflow> _workflowFactory;

        private WorkflowSchedulerConfiguration _configuration = new WorkflowSchedulerConfiguration();

        private Task _task;

        private MethodInfo _executingMethod;

        private MethodInfo _executingMethodAsync;

        private bool _disposedValue;

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
        /// <param name="token">The CancellationToken to cancel the request.</param>
        /// <returns>A long running Task.</returns>
        public Task StartAsync(CancellationToken token = default)
        {
            if (_workflowFactory == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowScheduler<TWorkflow>)} requires a Workflow Factory function provided in the {nameof(WorkflowScheduler<TWorkflow>.Use)} method");
            }

            // A user should specify a WorkflowScheduler to execute a specific frequency or time.
            if (_configuration.ExecuteAt == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowSchedulerConfiguration.ExecuteAt)} has not been set.");
            }

            _task = Task.Run(async () =>
            {
                if (_configuration?.ExecuteAt is WorkflowFrequency workflowFrequency)
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(workflowFrequency.Frequency).ContinueWith(async t =>
                        {
                            if (_configuration.ExecuteAsync)
                            {
                                await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { token });
                            }
                            else
                            {
                                _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { token });
                            }
                        });
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
                                if (_configuration.ExecuteAsync)
                                {
                                    await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { token });
                                }
                                else
                                {
                                    _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { token });
                                }

                                if (!workflowDateTime.Indefinitely) break;

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
                                if (_configuration.ExecuteAsync)
                                {
                                    await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { token });
                                }
                                else
                                {
                                    _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { token });
                                }

                                if (!workflowDateTime.Indefinitely) break;

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
                                if (_configuration.ExecuteAsync)
                                {
                                    await (Task)_executingMethodAsync.Invoke(_workflowFactory.Invoke(), new object[] { token });
                                }
                                else
                                {
                                    _executingMethod.Invoke(_workflowFactory.Invoke(), new object[] { token });
                                }

                                if (!workflowDateTime.Indefinitely) break;

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

            return _task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _workflowFactory = null;
                    _task = null;
                    _executingMethod = null;
                    _executingMethodAsync = null;
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
