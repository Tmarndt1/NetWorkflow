using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    public class WorkflowStepAsyncExecutor<TIn, TResult> : IWorkflowExecutor<TIn, TResult>
    {
        private readonly LambdaExpression _expression;

        public WorkflowStepAsyncExecutor(LambdaExpression expression)
        {
            _expression = expression;
        }

        public TResult Run(TIn args, CancellationToken token = default)
        {
            object body = null;

            if (_expression.Parameters.Count > 0) throw new InvalidOperationException("Parameter count within lambda cannot be greater than 0");

            body = _expression.Compile().DynamicInvoke();

            if (body == null) throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (token.IsCancellationRequested)
            {
                throw new WorkflowStoppedException();
            }

            if (body is IWorkflowStepAsync<TIn, TResult> stepAsync)
            {
                var executingMethod = stepAsync.GetType().GetMethod(nameof(IWorkflowStepAsync<TResult>.RunAsync));

                if (executingMethod == null) throw new InvalidOperationException("WorkflowStep executing method not found");

                Task<TResult> task = (Task<TResult>)executingMethod.Invoke(body, new object[] { args, token });

                Task.WaitAll(task);

                return task.Result;
            }

            throw new InvalidOperationException("Internal error");
        }
    }
}
