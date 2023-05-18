using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class WorkflowParallelExecutor<TIn, TOut> : IWorkflowExecutor<TIn, IEnumerable<TOut>>
    {
        private readonly LambdaExpression _expression;

        private bool _disposedValue;

        public WorkflowParallelExecutor(LambdaExpression expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public IEnumerable<TOut> Run(TIn args, CancellationToken token = default)
        {
            if (_expression.Parameters.Count > 0)
                throw new InvalidOperationException("Parameter count within lambda cannot be greater than 0");

            var body = _expression.Compile().DynamicInvoke();

            if (body == null)
                throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (token.IsCancellationRequested)
                throw new WorkflowStoppedException();

            if (body is IEnumerable<IWorkflowStepAsync> asyncSteps)
            {
                var tasks = asyncSteps.Select(x =>
                {
                    var executor = x.GetType().GetMethod(nameof(IWorkflowStepAsync<TOut>.RunAsync));

                    if (executor == null)
                        throw new InvalidOperationException("Internal error");

                    if (executor.GetParameters().Length > 1)
                        return (Task<TOut>)executor.Invoke(x, new object[] { args, token });
                    else
                        return (Task<TOut>)executor.Invoke(x, new object[] { token });

                }).ToArray();

                if (token.IsCancellationRequested)
                    throw new WorkflowStoppedException();

                Task.WaitAll(tasks, token);

                return tasks.Select(x => x.Result);
            }

            throw new InvalidOperationException("Internal error");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    (_expression as IDisposable)?.Dispose();
                }

                // Clean up unmanaged resources
                // (none in this case)

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
