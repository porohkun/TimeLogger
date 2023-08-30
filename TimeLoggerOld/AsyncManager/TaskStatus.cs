using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLoggerOld
{
    public class TaskStatus
    {
        public double Progress { get; private set; }
        public string Status { get; private set; }

        public TaskStatus(double progress, string status)
        {
            Progress = progress;
            Status = status;
        }
    }
}
