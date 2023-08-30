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

namespace TimeLoggerOld
{
    /// <summary>
    /// Interaction logic for DayEditWindow.xaml
    /// </summary>
    public partial class DayEditWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }

        private DateTime _date;
        private ObservableCollection<TaskPeriod> _periods;
        public ObservableCollection<TaskPeriod> Periods
        {
            get
            {
                if (_periods == null)
                {
                    _periods = new ObservableCollection<TaskPeriod>(TaskPeriod.GetForDay(_date));
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

        //public class TaskData
        //{
        //    public string Task { get; set; }
        //    public string Length { get; set; }
        //}

        public DayEditWindow()
        {
            InitializeComponent();
            DataContext = this;
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

        public void ShowDay(DateTime day)
        {
            _date = day;
            Title = day.ToString("dd.MM.yyyy");

            //var tasks = new Dictionary<string, TimeSpan>();
            //TimeSpan total = new TimeSpan();

            //var nextDay = day.AddDays(1).Date;
            //foreach (var period in TaskPeriod.GetForDay(day))
            //{
            //    var periodLength = (period.End < nextDay ? period.End : nextDay) - (period.Start > day ? period.Start : day);
            //    if (tasks.ContainsKey(period.Task.ID))
            //        tasks[period.Task.ID] += periodLength;
            //    else
            //        tasks.Add(period.Task.ID, periodLength);
            //    total += periodLength;
            //}

            //var taskList = new List<TaskData>();
            //foreach (var pair in tasks)
            //    taskList.Add(new TaskData() { Task = pair.Key, Length = pair.Value.ToJira() });

            //grid.ItemsSource = taskList;

            //dayText.Text = string.Format("Total: {0}", total.ToJira());
        }
    }
}
