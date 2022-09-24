using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilderConditionalNext<TContext, Tout>
    {
        public IWorkflowBuilderConditional<TContext, Tout> ElseIf(Expression<Func<Tout, bool>> func);

        public IWorkflowBuilderConditional<TContext, Tout> Else();

        public IWorkflowBuilderNext<TContext, object> EndIf();
    }
}
