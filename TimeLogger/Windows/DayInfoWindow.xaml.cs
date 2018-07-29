using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DayInfoWindow.xaml
    /// </summary>
    public partial class DayInfoWindow : Window
    {
        public class TaskData
        {
            public string Task { get; set; }
            public string Length { get; set; }
        }

        public ObservableCollection<TaskData> Periods { get; } = new ObservableCollection<TaskData>();

        public DayInfoWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ShowDay(DateTime day)
        {
            Title = day.ToString("dd.MM.yyyy");
            
            var tasks = new Dictionary<Task, TimeSpan>();
            TimeSpan total = new TimeSpan();

            var nextDay = day.AddDays(1).Date;
            foreach (var period in TaskPeriod.GetForDay(day))
            {
                var periodLength = (period.End < nextDay ? period.End : nextDay) - (period.Start > day ? period.Start : day);
                if (tasks.ContainsKey(period.Task))
                    tasks[period.Task] += periodLength;
                else
                    tasks.Add(period.Task, periodLength);
                total += periodLength;
            }

            Periods.Clear();
            foreach (var pair in tasks)
                Periods.Add(new TaskData() { Task = pair.Key.Name, Length = pair.Value.ToJira() });

            dayText.Text = string.Format("Total: {0}", total.ToJira());
        }
    }
}
