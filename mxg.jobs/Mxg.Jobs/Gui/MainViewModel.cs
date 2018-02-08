using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mxg.Jobs.Gui
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<JobPresentationEntity> _jobs;
        private bool _cluster;
        public string Cluster => _cluster?"Запущено в кластере":"Не кластер";
        
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

        
        public void SetJobs(List<SingleCallCronJob> jobDictionary,bool cluster)
        {
            _cluster = cluster;
            Jobs = new ObservableCollection<JobPresentationEntity>(jobDictionary
                .Select(job => new JobPresentationEntity(job,cluster)));
        }
    }
}
