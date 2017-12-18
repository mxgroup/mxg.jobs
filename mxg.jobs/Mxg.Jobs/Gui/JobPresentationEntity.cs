using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Mxg.Jobs.Gui
{
    public class JobPresentationEntity : INotifyPropertyChanged
    {
        private const string StoppedStatusText = "Stopped";
        private const string StartedStatusText = "Started";
        private string _name;
        private string _status;

        public JobPresentationEntity(SingleCallCronJob job,bool cluster)
        {
            Name = job.GetType().Name;

            ExecuteOnceCommand = new RelayCommand(param => job.Execute());
            StartCommand = new RelayCommand(param =>
            {
                job.Start();
                Status = StartedStatusText;
            });
            StopCommand = new RelayCommand(param =>
            {
                job.Stop(cluster);
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
