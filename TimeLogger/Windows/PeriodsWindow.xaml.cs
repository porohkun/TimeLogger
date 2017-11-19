using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for PeriodsWindow.xaml
    /// </summary>
    public partial class PeriodsWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }

        private Task _task;
        private ObservableCollection<TaskPeriod> _periods;
        public ObservableCollection<TaskPeriod> Periods
        {
            get
            {
                if (_periods == null)
                {
                    _periods = new ObservableCollection<TaskPeriod>(TaskPeriod.TaskPeriods(_task));
                    _periods.Sort((p1, p2) => { return p1.Start.CompareTo(p2.Start); });
                }
                return _periods;
            }
        }

        private TaskPeriod _selected;
        public TaskPeriod Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged("Selected"); }
        }

        public DateTime Test = DateTime.Now;

        public PeriodsWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public PeriodsWindow(Task task) : this()
        {
            _task = task;
            Title = _task.IDName;
        }

        void ListBox_PreviewRightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainWindow.SaveAll();
        }
    }
}
