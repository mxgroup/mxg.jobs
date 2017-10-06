using Quartz;

namespace Mxg.Jobs
{
    [DisallowConcurrentExecution]
    public abstract class QuartzJob4 : IJob
    {
        public abstract string CronExpression { get; }

        /// <inheritdoc />
        public void Execute(IJobExecutionContext context)
        {
            Execute();
        }

        public abstract void Execute();
    }
}
