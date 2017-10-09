﻿using Quartz;

namespace Mxg.Jobs
{
    [DisallowConcurrentExecution]
    public abstract class SingleCallCronJob : IJob
    {
        internal IScheduler Scheduler { get; set; }

        internal ITrigger Trigger { get; set; }

        internal IJobDetail JobDetail { get; set; }

        public abstract string CronExpression { get; }

        /// <inheritdoc />
        public void Execute(IJobExecutionContext context)
        {
            Execute();
        }

        public void Start()
        {
            Scheduler.ResumeJob(JobDetail.Key);
        }

        public void Stop()
        {
            Scheduler.PauseJob(JobDetail.Key);
        }

        public abstract void Execute();
    }
}
