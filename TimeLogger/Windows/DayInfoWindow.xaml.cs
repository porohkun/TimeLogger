using System;
using System.Collections.Generic;
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

        public DayInfoWindow()
        {
            InitializeComponent();
        }

        public void ShowDay(DateTime day)
        {
            Title = day.ToString("dd.MM.yyyy");
            
            var tasks = new Dictionary<string, TimeSpan>();
            TimeSpan total = new TimeSpan();

            var nextDay = day.AddDays(1).Date;
            foreach (var period in TaskPeriod.GetForDay(day))
            {
                var periodLength = (period.End < nextDay ? period.End : nextDay) - (period.Start > day ? period.Start : day);
                if (tasks.ContainsKey(period.Task.ID))
                    tasks[period.Task.ID] += periodLength;
                else
                    tasks.Add(period.Task.ID, periodLength);
                total += periodLength;
            }

            var taskList = new List<TaskData>();
            foreach (var pair in tasks)
                taskList.Add(new TaskData() { Task = pair.Key, Length = pair.Value.ToJira() });

            grid.ItemsSource = taskList;

            dayText.Text = string.Format("Total: {0}", total.ToJira());
        }
    }
}
