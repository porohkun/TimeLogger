using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TimeLoggerOld
{
    public static class AsyncManager
    {
        public static event EventHandler StatusChanged;
        private static string _status;
        public static string Status
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    StatusChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler ProgressChanged;
        private static double _progress;
        public static double Progress
        {
            get => _progress * 100;
            set
            {
                if (Math.Abs(value - _progress) > 0.00001)
                {
                    _progress = value;
                    ProgressChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        private static BackgroundWorker _bw;
        private static Queue<IAsyncTask> _tasks;
        private static bool _busy;

        static AsyncManager()
        {
            Status = "";
            _tasks = new Queue<IAsyncTask>();
            _bw = new BackgroundWorker() { WorkerReportsProgress = true };
            _bw.DoWork += _bw_DoWork;
            _bw.ProgressChanged += _bw_ProgressChanged;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
        }

        public static void Push(IAsyncTask task)
        {
            _tasks.Enqueue(task);
            if (!_busy)
                Start();
        }

        public static void Start()
        {
            try
            {
                var task = _tasks.Dequeue();
                _busy = true;
                SetStatus(task.StartStatus);
                SetProgress(0);
                _bw.RunWorkerAsync(task);
            }
            catch (Exception e)
            {

            }
        }

        private static void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var task = (IAsyncTask)e.Argument;
            try
            {
                var routine = task.DoWork();
                do
                {
                    var status = routine.Current;
                    if (status != null)
                        _bw.ReportProgress(0, status);
                } while (routine.MoveNext());
                e.Result = task;
                _bw.ReportProgress(0, new TaskStatus(0f, task.FinalStatus));
            }
            catch (Exception ex)
            {
                task.ExceptionCatch(ex);
                _bw.ReportProgress(0, new TaskStatus(0f, task.FailureStatus));
            }
        }

        private static void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var status = (TaskStatus)e.UserState;
            SetStatus(status.Status);
            SetProgress(status.Progress);
        }

        private static void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var task = (IAsyncTask)e.Result;
                task.TaskCompleted();
            }
            _busy = false;
            Start();
        }

        private static void SetStatus(string status)
        {
            Status = status;
            //if (StatusChanged != null)
            //    StatusChanged(status);
        }

        private static void SetProgress(double progress)
        {
            Progress = progress;
            //if (ProgressChanged != null)
            //    ProgressChanged(progress);
        }
    }
}
