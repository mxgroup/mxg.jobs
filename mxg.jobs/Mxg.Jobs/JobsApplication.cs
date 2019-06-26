using Mxg.Jobs.Gui;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Mxg.Jobs
{
    public class JobsApplication
    {
        public event Action Started;
        public event Action Stopped;

        public string[] Args { get; set; }

        private readonly IEnumerable<Type> _jobTypes;
        private readonly Func<Type, SingleCallCronJob> _getJobInstance;
        private NameValueCollection propertiesDB;
        public JobsApplication(IEnumerable<Type> jobTypes, Func<Type, SingleCallCronJob> getJobInstance = null)
        {
            _jobTypes = jobTypes;
            _getJobInstance = getJobInstance;
        }

        
        public async Task Run(bool cluster)
        {
            // TODO:
            /*
             * + Переименовать классы и интерфейсы 
             * + Внести шедулер внутрь джоба
             * - Добавить листенеры на мисфаер, ошибки и прочее
             * - Написать тесты
             * - Добавить синхронизацию между машинами
             */
           ISchedulerFactory schedulerFactory;
            if (cluster)
            {
                if (propertiesDB != null)
                {
                    schedulerFactory = new StdSchedulerFactory(propertiesDB);
                }
                else
                {
                    throw new Exception("Нет настроек DB");
                }
            }
            else
            {
                schedulerFactory = new StdSchedulerFactory();
            }

            IJobFactory customJobFactory = new CustomJobFactory(type => _getJobInstance(type));

            // NB: GetScheduler() возвращает Singleton - один и тот же инстанс IScheduler
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            scheduler.JobFactory = customJobFactory;            
            List<SingleCallCronJob> jobs = _jobTypes.Select(x => _getJobInstance(x)).ToList();

            foreach (SingleCallCronJob job in jobs)
            {
                try
                {
                    Type jobType = job.GetType();

                    job.Scheduler = scheduler;

                    IJobDetail jobDetail = JobBuilder.Create(jobType)
                        .WithIdentity(jobType.Name, jobType.Namespace)
                        .RequestRecovery()
                        .Build();
                    job.JobDetail = jobDetail;

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(jobType.Name, jobType.Namespace)
                        .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second))
                        .WithCronSchedule(job.CronExpression)
                        .Build();

                    job.Trigger = trigger;
                    // Если задача уже добавлена, то не добавляем ее
                    if (await scheduler.GetJobDetail(jobDetail.Key) == null)
                    {
                        await scheduler.ScheduleJob(jobDetail, trigger);
                    }
                }
                //в DB не добавлены нужные таблицы
                catch (JobPersistenceException exc)
                {
                    throw;
                }
            }
            // Ставим всё на паузу, если мы не из кластерного решения: для GUI ждём ручного запуска, для службы ждём вызова OnStart()
            if (!cluster)
            {
                await scheduler.PauseAll();
            }

            if (Environment.UserInteractive || Debugger.IsAttached)
            {
                var viewModel = new MainViewModel();
                viewModel.SetJobs(jobs,cluster);

                var window = new MainWindow { DataContext = viewModel };
                window.Show();
                window.Closed += async (sender, args) =>
                {
                    if (!cluster)
                    {
                        foreach (var job in viewModel.Jobs)
                        {
                            job.StopCommand.Execute(null);
                        }
                    }
                    await scheduler.Shutdown();
                    Stopped?.Invoke();
                };
                if (!cluster)
                {
                    await scheduler.Start();
                }
                Started?.Invoke();
            }
            else
            {

                var jobService = new JobService(jobs,cluster);
                jobService.Started += async () =>
                {
                    await scheduler.Start();
                    Started?.Invoke();
                };
                jobService.Stopped += async () =>
                {
                    await scheduler.Shutdown();
                    Stopped?.Invoke();
                };
                ServiceBase.Run(new ServiceBase[] { jobService });
            }
        }

        public void InitDB(NameValueCollection properties)
        {
            this.propertiesDB = properties;
        }
    }
}