using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Quartz;

namespace Mxg.Jobs.Gui
{
    public class JobPresentationEntity : INotifyPropertyChanged
    {
        private const string StoppedStatusText = "Stopped";
        private const string StartedStatusText = "Started";
        private string _name;
        private string _status;

        public JobPresentationEntity(KeyValuePair<QuartzJob4, IScheduler> job)
        {
            Name = job.Key.GetType().Name;

            ExecuteOnceCommand = new RelayCommand(param => job.Key.Execute());
            StartCommand = new RelayCommand(param =>
            {
                job.Value.Start();
                Status = StartedStatusText;
            });
            StopCommand = new RelayCommand(param =>
            {
                job.Value.Shutdown();
                Status = StoppedStatusText;
            });
            
            Status = StoppedStatusText;
        }

        public ICommand ExecuteOnceCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
