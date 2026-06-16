using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Scheduler
{
    /// <summary>
    /// The WorkflowScheduler is a generic workflow scheduler that is responsible for scheduling and executing workflows.
    /// </summary>
    /// <typeparam name="TWorkflow">The Workflow to executed.</typeparam>
    /// <typeparam name="TOut">The output type of the Workflow.</typeparam>
    public class WorkflowScheduler<TWorkflow, TOut> : IDisposable
        where TWorkflow : IWorkflow<TOut>
    {
        private Func<TWorkflow> _workflowFactory;
        private Func<CancellationToken, Task<WorkflowResult<TOut>>> _workflowRunner;

        private WorkflowSchedulerConfiguration<TOut> _configuration = new WorkflowSchedulerConfiguration<TOut>();

        private int _count = 0;

        private bool _disposedValue;

        /// <summary>
        /// Designates the WorkflowScheduler to use the function to create the new Workflow.
        /// </summary>
        /// <param name="workflowFactory">A function that returns a Workflow.</param>
        /// <param name="configuration">The WorkflowScheduler configuration.</param>
        public WorkflowScheduler(Func<TWorkflow> workflowFactory, Action<WorkflowSchedulerConfiguration<TOut>> configuration)
        {
            if (workflowFactory == null) throw new ArgumentNullException(nameof(workflowFactory), "The WorkflowScheduler requires a workflow factory.");
            if (configuration == null) throw new ArgumentNullException(nameof(configuration), "The WorkflowScheduler requires a configuration to define when to execute the workflow.");

            _workflowFactory = workflowFactory;
            _workflowRunner = CreateWorkflowRunner(workflowFactory);

            configuration.Invoke(_configuration);
        }

        public WorkflowScheduler(
            Func<CancellationToken, Task<WorkflowResult<TOut>>> workflowRunner,
            Action<WorkflowSchedulerConfiguration<TOut>> configuration,
            WorkflowSchedulerRunnerMode runnerMode)
        {
            if (workflowRunner == null) throw new ArgumentNullException(nameof(workflowRunner), "The WorkflowScheduler requires a workflow runner.");
            if (configuration == null) throw new ArgumentNullException(nameof(configuration), "The WorkflowScheduler requires a configuration to define when to execute the workflow.");

            _workflowRunner = workflowRunner;

            configuration.Invoke(_configuration);
        }

        /// <summary>
        /// Starts the WorkflowScheduler and returns the Task the WorkflowScheduler is running on.
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the request.</param>
        /// <returns>A long running Task until canceled.</returns>
        public Task StartAsync(CancellationToken token = default)
        {
            if (_workflowRunner == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowScheduler<TWorkflow, TOut>)} requires a Workflow runner function.");
            }

            if (_configuration.Schedule == null)
            {
                throw new InvalidOperationException($"A {nameof(WorkflowSchedulerConfiguration<TOut>.Schedule)} has not been set.");
            }

            if (_configuration.Schedule is FrequencySchedule frequencySchedule)
            {
                return ExecuteAsync(frequencySchedule, token);
            }

            if (_configuration.Schedule is CalendarSchedule calendarSchedule)
            {
                return ExecuteAsync(calendarSchedule, token);
            }

            throw new InvalidOperationException("Invalid workflow execution configuration.");
        }

        private static Func<CancellationToken, Task<WorkflowResult<TOut>>> CreateWorkflowRunner(Func<TWorkflow> workflowFactory)
        {
            if (workflowFactory == null) throw new ArgumentNullException(nameof(workflowFactory), "The WorkflowScheduler requires a workflow factory.");

            return token => workflowFactory.Invoke().RunAsync(token);
        }

        private async Task ExecuteAsync(FrequencySchedule frequencySchedule, CancellationToken token)
        {
            var runningTasks = new List<Task>();

            try
            {
                if (_configuration.RunImmediately)
                {
                    await ExecuteScheduledWorkflowAsync(runningTasks, token).ConfigureAwait(false);
                }

                while (!token.IsCancellationRequested && !_disposedValue)
                {
                    await Task.Delay(frequencySchedule.Frequency, token).ConfigureAwait(false);

                    if (token.IsCancellationRequested || _disposedValue) break;

                    await ExecuteScheduledWorkflowAsync(runningTasks, token).ConfigureAwait(false);

                    if (frequencySchedule.ExecutionCount != -1 && _count >= frequencySchedule.ExecutionCount) break;
                }

                if (runningTasks.Count > 0)
                {
                    await Task.WhenAll(runningTasks).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
        }

        private async Task ExecuteAsync(CalendarSchedule calendarSchedule, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && !_disposedValue)
                {
                    if (calendarSchedule.IsNow(DateTimeOffset.Now))
                    {
                        await ExecuteWorkflowAsync(token).ConfigureAwait(false);

                        if (++_count == calendarSchedule.ExecutionCount) break;
                    }

                    await Task.Delay(60000, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
        }

        private async Task ExecuteScheduledWorkflowAsync(List<Task> runningTasks, CancellationToken token)
        {
            switch (_configuration.OverlapPolicy)
            {
                case WorkflowOverlapPolicy.AllowConcurrent:
                    var task = ExecuteWorkflowAndCountAsync(token);
                    runningTasks.Add(task);
                    runningTasks.RemoveAll(x => x.IsCompleted);
                    break;
                case WorkflowOverlapPolicy.Skip:
                    runningTasks.RemoveAll(x => x.IsCompleted);
                    if (runningTasks.Count == 0)
                    {
                        runningTasks.Add(ExecuteWorkflowAndCountAsync(token));
                    }
                    break;
                case WorkflowOverlapPolicy.Wait:
                    await ExecuteWorkflowAndCountAsync(token).ConfigureAwait(false);
                    break;
                default:
                    throw new InvalidOperationException("Invalid workflow overlap policy.");
            }
        }

        private async Task ExecuteWorkflowAndCountAsync(CancellationToken token)
        {
            await ExecuteWorkflowAsync(token).ConfigureAwait(false);
            _count++;
        }

        private async Task ExecuteWorkflowAsync(CancellationToken token)
        {
            try
            {
                WorkflowResult<TOut> output = await _workflowRunner.Invoke(token).ConfigureAwait(false);

                _configuration?.OnExecuted?.Invoke(output);

                if (_configuration?.OnExecutedAsync != null)
                {
                    await _configuration.OnExecutedAsync.Invoke(output, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (_configuration?.OnErrorAsync != null)
                {
                    await _configuration.OnErrorAsync.Invoke(ex, token).ConfigureAwait(false);
                    return;
                }

                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _workflowFactory = null;
                    _workflowRunner = null;
                    _configuration = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
