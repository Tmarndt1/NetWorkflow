using System.Linq.Expressions;

namespace NetWorkflow
{
    public class WorkflowExecutorConditional<TIn> : IWorkflowExecutor<TIn, object>
    {
        private readonly List<ExecutorWrapper> _next = new List<ExecutorWrapper>();

        public WorkflowExecutorConditional(Expression<Func<TIn, bool>> expression)
        {
            _next.Add(new ExecutorWrapper(expression));
        }

        public void Append(Expression<Func<TIn, bool>> expression)
        {
            _next.Add(new ExecutorWrapper(expression));
        }

        public void Append(IWorkflowExecutor<TIn, object> executor)
        {
            _next.Last().Executor = executor;
        }

        public void SetStop()
        {
            _next.Last().ShouldStop = true;
        }

        public void SetExceptionToThrow(Expression<Func<Exception>> func)
        {
            _next.Last().ExceptionFunc = func;
        }

        public void SetRetry(int maxRetries, Action onRetry)
        {
            ExecutorWrapper wrapper = _next.Last();

            wrapper.Retry = true;

            wrapper.RetryCount = maxRetries;

            wrapper.OnRetry = onRetry;
        }

        public object Run(TIn args, CancellationToken token = default)
        {
            var enumerator = _next.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (((Func<TIn, bool>)enumerator.Current.Expression.Compile()).Invoke(args))
                {
                    if (enumerator.Current.ShouldStop || token.IsCancellationRequested)
                    {
                        throw new WorkflowStoppedException();
                    }
                    else if (enumerator.Current.ExceptionFunc != null)
                    {
                        throw enumerator.Current.ExceptionFunc.Compile().Invoke();
                    }
                    else if (enumerator.Current.Retry)
                    {
                        if (enumerator.Current.RetryCount == 0) throw new WorkflowMaxRetryException();

                        --enumerator.Current.RetryCount;

                        if (enumerator.Current.OnRetry == null) throw new InvalidOperationException("Internal error with null OnRetry callback");

                        enumerator.Current.OnRetry();
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

            public IWorkflowExecutor<TIn, object> Executor { get; set; }

            public Expression<Func<Exception>> ExceptionFunc { get; set; }

            public bool Retry { get; set; }

            public int RetryCount { get; set; }

            public Action OnRetry { get; set; }

            public bool ShouldStop { get; set; } = false;

            public ExecutorWrapper(LambdaExpression expression)
            {
                Expression = expression;
            }
        }
    }
}
