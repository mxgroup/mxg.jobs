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
        private readonly Func<Type, SingleCallCronJob> _getJobInstance;

        public JobsApplication(IEnumerable<Type> jobTypes, Func<Type, SingleCallCronJob> getJobInstance = null)
        {
            _jobTypes = jobTypes;
            _getJobInstance = getJobInstance;
        }

        public void Run()
        {
            // TODO:
            /*
             * + Переименовать классы и интерфейсы
             * + Внести шедулер внутрь джоба
             * - Добавить листенеры на мисфаер, ошибки и прочее
             * - Написать тесты
             * - Добавить синхронизацию между машинами
             */

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IJobFactory customJobFactory = new CustomJobFactory(type => _getJobInstance(type));

            // NB: GetScheduler() возвращает Singleton - один и тот же инстанс IScheduler
            IScheduler scheduler = schedulerFactory.GetScheduler();
            scheduler.JobFactory = customJobFactory;

            List<SingleCallCronJob> jobs = _jobTypes.Select(x => _getJobInstance(x)).ToList();

            foreach (SingleCallCronJob job in jobs)
            {
                Type jobType = job.GetType();

                job.Scheduler = scheduler;

                IJobDetail jobDetail = JobBuilder.Create(jobType)
                    .WithIdentity(jobType.Name, jobType.Namespace)
                    .Build();
                job.JobDetail = jobDetail;

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(jobType.Name, jobType.Namespace)
                    .WithCronSchedule(job.CronExpression)
                    .Build();
                job.Trigger = trigger;

                scheduler.ScheduleJob(jobDetail, trigger);
            }

            // Ставим всё на паузу: для GUI ждём ручного запуска, для службы ждём вызова OnStart()
            scheduler.PauseAll();
            
            if (Environment.UserInteractive || Debugger.IsAttached)
            {
                var viewModel = new MainViewModel();
                viewModel.SetJobs(jobs);

                var window = new MainWindow { DataContext = viewModel };
                window.Show();
                window.Closed += (sender, args) =>
                {
                    foreach (var job in viewModel.Jobs)
                    {
                        job.StopCommand.Execute(null);
                    }
                    scheduler.Shutdown();
                    Stopped?.Invoke();
                };
                scheduler.Start();
                Started?.Invoke();
            }
            else
            {

                var jobService = new JobService(jobs);
                jobService.Started += () =>
                {
                    scheduler.Start();
                    Started?.Invoke();
                };
                jobService.Stopped += () =>
                {
                    scheduler.Shutdown();
                    Stopped?.Invoke();
                };

                ServiceBase.Run(new ServiceBase[] { jobService });
            }
        }
    }
}