using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace CircularWaitingControl
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double _value;
        private bool _start;

        public MainViewModel()
        {
            StartCommand = new RelayCommand(OnStart);
            Value = 1000;
        }

        public double Value
        {
            get { return _value; }
            private set
            {
                if (_value.Equals(value)) return;
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public bool Start
        {
            get { return _start; }
            private set
            {
                if (_start.Equals(value)) return;
                _start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        public ICommand StartCommand { get; }

        private void OnStart(object o)
        {
            Value = 0.0;

            var timer = new DispatcherTimer();
            timer.Tick += (s, ea) =>
            {
                Value += 1.0;
                if (Value >= 1000.0)
                    //Value = 0.0;
                timer.Stop();
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();

            Start = !Start;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
