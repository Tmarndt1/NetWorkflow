using NetWorkflow.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace NetWorkflow
{
    /// <summary>
    /// Allows the execution step to pass a value from one step to another by returning the same arguments in the Run method.
    /// </summary>
    public class WorkflowExecutorNext : IWorkflowExecutor
    {
        public object? Run(object? args, CancellationToken token = default)
        {
            return args;
        }
    }

    public class WorkflowExecutor<TContext, TResult> : IWorkflowExecutor
    {
        private readonly LambdaExpression _expression;

        private readonly TContext _context;

        public WorkflowExecutor(LambdaExpression expression, TContext context)
        {
            _expression = expression;

            _context = context;
        }

        public object? Run(object? args, CancellationToken token = default)
        {
            MethodInfo? executor = null;

            object? body = null;

            body = _expression.Parameters.Count == 1 ? _expression.Compile().DynamicInvoke(_context) : _expression.Compile().DynamicInvoke();

            if (body == null) throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (body is IWorkflowStep step)
            {
                executor = step.GetType().GetMethod(nameof(IWorkflowStep<TResult>.Run));

                if (executor == null) throw new InvalidOperationException("Run method not found");

                int count = executor.GetParameters().Length;

                if (count == 1)
                {
                    return (TResult?)executor.Invoke(body, new object[] { token });
                }
                else
                {
                    return (TResult?)executor.Invoke(body, new object?[] { args, token });
                }
            }
            else if (body is IEnumerable<IWorkflowStepAsync> asyncSteps)
            {
                Task<TResult>?[] tasks = asyncSteps.Select(x =>
                {
                    executor = x.GetType().GetMethod(nameof(IWorkflowStepAsync<TResult>.RunAsync));

                    if (executor == null) throw new InvalidOperationException("Internal error");

                    if (executor.GetParameters().Length > 1)
                    {
                        return (Task<TResult>)executor.Invoke(x, new object?[] { args, token });
                    }
                    else
                    {
                        return (Task<TResult>)executor.Invoke(x, new object[] { token });
                    }
                }).ToArray();

                Task.WaitAll(tasks, token);

                return tasks.Where(x => x != null).Select(x => x.Result).ToArray();
            }

            throw new InvalidOperationException("Internal error");
        }
    }

    public class WorkflowExecutorConditional<Tin> : IWorkflowExecutor
    {
        public bool Stopped { get; private set; }

        private readonly List<ExecutorWrapper> _next = new List<ExecutorWrapper>();

        public WorkflowExecutorConditional(Expression<Func<Tin, bool>> expression)
        {
            _next.Add(new ExecutorWrapper(expression));
        }

        public void Append(Expression<Func<Tin, bool>> expression)
        {
            _next.Add(new ExecutorWrapper(expression));
        }

        public void Append(IWorkflowExecutor executor)
        {
            _next.Last().Executor = executor;
        }

        public void SetStop()
        {
            _next.Last().ShouldStop = true;
        }

        public object? Run(object? args, CancellationToken token = default)
        {
            var enumerator = _next.GetEnumerator();

            while (enumerator.MoveNext() && !token.IsCancellationRequested)
            {
                if (((Func<Tin, bool>)enumerator.Current.Expression.Compile()).Invoke((Tin)args))
                {
                    if (enumerator.Current.ShouldStop)
                    {
                        Stopped = true;

                        return new WorkflowStoppedResult(args);
                    }

                    return enumerator.Current.Executor?.Run(args, token);
                }
            }

            // If no condition was met then default to a WorkflowConditionResult
            return new WorkflowResult("No condition was met");
        }

        private sealed class ExecutorWrapper
        {
            public LambdaExpression Expression { get; set; }

            public IWorkflowExecutor? Executor { get; set; }

            public bool ShouldStop { get; set; }

            public ExecutorWrapper(LambdaExpression expression)
            {
                Expression = expression;
            }
        }
    }
}
