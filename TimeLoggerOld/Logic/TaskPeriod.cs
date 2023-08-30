using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimiJson;
using System.ComponentModel;

namespace TimeLoggerOld
{
    public class TaskPeriod : INotifyPropertyChanged
    {
        private static List<TaskPeriod> Periods = new List<TaskPeriod>();

        private DateTime _start;
        private DateTime _end;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }

        public string StartString { get { return Start.ToString("yyyy-MM-dd HH:mm"); } }
        public string EndString { get { return End.ToString("yyyy-MM-dd HH:mm"); } }
        public string DurationString { get { return Duration.ToJira(); } }

        public Task Task { get; private set; }
        public bool Started { get; private set; }
        public bool Ended { get; private set; }
        public DateTime Start
        {
            get { return Started ? _start : DateTime.Now; }
            set { _start = value; Started = true; OnPropertyChanged("Start"); OnPropertyChanged("StartString"); OnPropertyChanged("Duration"); OnPropertyChanged("DurationString"); }
        }
        public DateTime End
        {
            get { return Ended ? _end : DateTime.Now; }
            set { _end = value; Ended = true; OnPropertyChanged("End"); OnPropertyChanged("EndString"); OnPropertyChanged("Duration"); OnPropertyChanged("DurationString"); }
        }

        public TimeSpan Duration { get { return Started ? (End - Start) : TimeSpan.Zero; } }

        public static List<TaskPeriod> TaskPeriods(TimeLoggerOld.Task task)
        {
            return new List<TaskPeriod>(from p in Periods where p.Task == task select p);
        }

        public static TimeSpan TaskDuration(Task task)
        {
            if (task == null)
                return new TimeSpan(0);
            return new TimeSpan(TaskPeriods(task).Sum(a => { return a.Duration.Ticks; }));
        }

        public static TaskPeriod GetNew(Task task)
        {
            return new TaskPeriod(task);
        }

        public static IEnumerable<TaskPeriod> GetToday()
        {
            return GetForDay(DateTime.Now.Date);
        }

        public static IEnumerable<TaskPeriod> GetForDay(DateTime day)
        {
            var nextDay = day.AddDays(1);
            foreach (TaskPeriod period in Periods)
            {
                if ((period.Start > day.Date && period.Start < nextDay.Date) || (period.End > day.Date && period.End < nextDay.Date))
                    yield return period;
            }
        }

        private TaskPeriod(Task task)
        {
            Task = task;
        }

        private TaskPeriod(JsonValue period)
        {
            Task = Task.GetById(period["task"]);
            Start = new DateTime(period["start"]);
            End = new DateTime(period["end"]);
        }

        public void Begin()
        {
            if (!Started)
            {
                Start = DateTime.Now;
                Periods.Add(this);
            }
        }

        public void Stop()
        {
            if (Started && !Ended)
                End = DateTime.Now;
        }

        public JsonValue ToJson()
        {
            return new JsonValue(new JsonObject(
                new JOPair("task", Task.ID),
                new JOPair("start", Start.Ticks),
                new JOPair("end", End.Ticks)
                ));
        }

        public static void Load(JsonArray json)
        {
            foreach (JsonValue jperiod in json)
            {
                TaskPeriod period = new TaskPeriod(jperiod);
                Periods.Add(period);
            }
        }

        public static JsonArray Save()
        {
            JsonArray result = new JsonArray();
            foreach (TaskPeriod period in Periods.OrderBy((p) => p.Start))
            {
                result.Add(period.ToJson());
            }
            return result;
        }


    }
}
