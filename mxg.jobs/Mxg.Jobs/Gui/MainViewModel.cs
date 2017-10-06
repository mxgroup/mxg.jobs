using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Quartz;
using Quartz.Impl.Matchers;

namespace Mxg.Jobs.Gui
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<JobPresentationEntity> _jobs;
        private IScheduler _scheduler;

        //public void SetJobs(IEnumerable<QuartzJob> jobs)
        //{
        //    Jobs = new ObservableCollection<JobPresentationEntity>(jobs.Select(x => new JobPresentationEntity(x)));
        //}

        public ObservableCollection<JobPresentationEntity> Jobs
        {
            get => _jobs;
            set
            {
                if (Equals(value, _jobs)) return;
                _jobs = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetJobs(Dictionary<QuartzJob4, IScheduler> jobDictionary)
        {
            Jobs = new ObservableCollection<JobPresentationEntity>(jobDictionary
                .Select(job => new JobPresentationEntity(job)));
        }
    }
}
