using System.Linq.Expressions;
using System.Reflection;

namespace NetWorkflow
{
    public class WorkflowParallelExecutor<TIn, TOut> : IWorkflowExecutor<TIn, TOut[]>
    {
        private LambdaExpression _expression;

        private bool _disposedValue;

        public WorkflowParallelExecutor(LambdaExpression expression)
        {
            _expression = expression;
        }

        public TOut[] Run(TIn args, CancellationToken token = default)
        {
            MethodInfo executor = null;

            object body = null;

            if (_expression.Parameters.Count > 0) throw new InvalidOperationException("Parameter count within lambda cannot be greater than 0");

            body = _expression.Compile().DynamicInvoke();

            if (body == null) throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (token.IsCancellationRequested)
            {
                throw new WorkflowStoppedException();
            }

            if (body is IEnumerable<IWorkflowStepAsync> asyncSteps)
            {
                Task<TOut>[] tasks = asyncSteps.Select(x =>
                {
                    executor = x.GetType().GetMethod(nameof(IWorkflowStepAsync<TOut>.RunAsync));

                    if (executor == null) throw new InvalidOperationException("Internal error");

                    if (executor.GetParameters().Length > 1)
                    {
                        return (Task<TOut>)executor.Invoke(x, new object[] { args, token });
                    }
                    else
                    {
                        return (Task<TOut>)executor.Invoke(x, new object[] { token });
                    }
                }).ToArray();

                if (token.IsCancellationRequested)
                {
                    throw new WorkflowStoppedException();
                }

                Task.WaitAll(tasks, token);

                return tasks.Select(x => x.Result).ToArray();
            }

            throw new InvalidOperationException("Internal error");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _expression = null;
                }

                _disposedValue = true;
            }
        }

        ~WorkflowParallelExecutor()
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
