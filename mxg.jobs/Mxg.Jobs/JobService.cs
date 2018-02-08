using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace Mxg.Jobs
{
    internal class JobService : ServiceBase
    {
        private readonly List<SingleCallCronJob> _jobs;
        private readonly bool _cluster;
        public event Action Started;
        public event Action Stopped;

        public JobService(List<SingleCallCronJob> jobs, bool cluster)
        {
            _jobs = jobs;
            _cluster = cluster;
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
            StopJobs(_cluster);
            Stopped?.Invoke();
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopJobs(_cluster);
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            StartJobs();
        }

        private void StartJobs()
        {
            foreach (var job in _jobs)
            {
                job.Start();
            }
        }

        private void StopJobs(bool cluster)
        {
            foreach (var job in _jobs)
            {
                job.Stop(cluster);
            }
        }
    }
}
