using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Quartz;

namespace Mxg.Jobs
{
    internal class JobService : ServiceBase
    {
        private readonly Dictionary<QuartzJob4, IScheduler> _jobDictionary;
        //private readonly IScheduler _scheduler;
        public event Action Started;
        public event Action Stopped;

        //private readonly IEnumerable<IQuartzJob> _jobs;

        public JobService(Dictionary<QuartzJob4, IScheduler> jobDictionary)
        {
            _jobDictionary = jobDictionary;
            //_scheduler = scheduler;
            //_jobs = jobs;
            ServiceName = GetType().Name;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            StartJobs();
            Started?.Invoke();
        }

        protected override void OnStop()
        {
            base.OnStop();
            StopJobs();
            Stopped?.Invoke();
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopJobs();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            StartJobs();
        }

        private void StartJobs()
        {
            //foreach (var job in _jobs)
            //{
            //    //job.Start();
            //}
        }

        private void StopJobs()
        {
            //foreach (var job in _jobs)
            //{
            //    //job.Stop();
            //}
        }
    }
}
