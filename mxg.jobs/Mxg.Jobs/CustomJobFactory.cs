using System;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace Mxg.Jobs
{
    public class CustomJobFactory : SimpleJobFactory
    {
        private readonly Func<Type, IJob> _getJobFunc;

        public CustomJobFactory(Func<Type, IJob> getJobFunc)
        {
            _getJobFunc = getJobFunc;
        }

        /// <inheritdoc />
        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _getJobFunc(bundle.JobDetail.JobType);
        }
    }
}
