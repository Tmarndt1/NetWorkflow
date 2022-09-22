﻿using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilderConditional<TContext, Tin> : IWorkflowBuilder<TContext, Tin>
    {
        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<WorkflowStep<Tin, TNext>>> func);

        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<TContext, WorkflowStep<Tin, TNext>>> func);
    }

    public interface IWorkflowBuilderConditional<TContext, Tin, Tout> : IWorkflowBuilderConditional<TContext, Tout>
    {

    }
}