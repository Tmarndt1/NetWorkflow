using System.Linq.Expressions;
using System.Reflection;

namespace NetWorkflow
{
    public class WorkflowStepAsyncExecutor<TIn, TOut> : IWorkflowExecutor<TIn, TOut>
    {
        private readonly LambdaExpression _expression;

        public WorkflowStepAsyncExecutor(LambdaExpression expression)
        {
            _expression = expression;
        }

        public TOut Run(TIn args, CancellationToken token = default)
        {
            MethodInfo executingMethod = null;

            object body = null;

            if (_expression.Parameters.Count > 0) throw new InvalidOperationException("Parameter count within lambda cannot be greater than 0");

            body = _expression.Compile().DynamicInvoke();

            if (body == null) throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (token.IsCancellationRequested)
            {
                throw new WorkflowStoppedException();
            }

            if (body is IWorkflowStepAsync<TIn, TOut> stepAsync)
            {
                executingMethod = stepAsync.GetType().GetMethod(nameof(IWorkflowStepAsync<TOut>.RunAsync));

                if (executingMethod == null) throw new InvalidOperationException("WorkflowStep executing method not found");

                Task<TOut> task = (Task<TOut>)executingMethod.Invoke(body, new object[] { args, token });

                Task.WaitAll(task);

                return task.Result;
            }

            throw new InvalidOperationException("Internal error");
        }
    }
}
