using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Mxg.Jobs.Gui;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Mxg.Jobs
{
    public class JobsApplication
    {
        public event Action Started;
        public event Action Stopped;

        public string[] Args { get; set; }

        private readonly IEnumerable<Type> _jobTypes;
        private readonly Func<Type, QuartzJob4> _getJobInstance;

        public JobsApplication(IEnumerable<Type> jobTypes, Func<Type, QuartzJob4> getJobInstance = null)
        {
            _jobTypes = jobTypes;
            _getJobInstance = getJobInstance;
        }

        public void Run()
        {
            // TODO:
            /*
             * Переименовать классы и интерфейсы
             * Внести шедулер внутрь джоба
             * Добавить листенеры на мисфаер, ошибки и прочее
             * Написать тесты
             * Добавить синхронизацию между машинами
             */

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IJobFactory customJobFactory = new CustomJobFactory(type => _getJobInstance(type));
            List<QuartzJob4> jobs = _jobTypes.Select(x => _getJobInstance(x)).ToList();

            var jobDictionary = new Dictionary<QuartzJob4, IScheduler>();

            foreach (QuartzJob4 job in jobs)
            {
                IScheduler scheduler = schedulerFactory.GetScheduler();
                scheduler.JobFactory = customJobFactory;
                Type jobType = job.GetType();

                IJobDetail jobDetail = JobBuilder.Create(jobType)
                    .WithIdentity(jobType.Name, jobType.Namespace)
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(jobType.Name, jobType.Namespace)
                    .WithCronSchedule(job.CronExpression)
                    .StartNow()
                    .Build();

                scheduler.ScheduleJob(jobDetail, trigger);

                jobDictionary.Add(job, scheduler);
            }
            
            if (Environment.UserInteractive || Debugger.IsAttached)
            {
                var window = new MainWindow();
                var viewModel = new MainViewModel();
                viewModel.SetJobs(jobDictionary);
                window.DataContext = viewModel;
                window.Show();
                window.Closed += (sender, args) =>
                {
                    foreach (var job in viewModel.Jobs)
                    {
                        job.StopCommand.Execute(null);
                    }
                    Stopped?.Invoke();
                };
                Started?.Invoke();
            }
            else
            {
                var jobService = new JobService(jobDictionary);
                jobService.Started += () => Started?.Invoke();
                jobService.Stopped += () => Stopped?.Invoke();
                ServiceBase.Run(new ServiceBase[] { jobService });
            }

            //}
        }
    }
}