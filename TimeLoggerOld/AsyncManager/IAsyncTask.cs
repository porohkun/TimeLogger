using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLoggerOld
{
    public interface IAsyncTask
    {
        string StartStatus { get; }
        string FinalStatus { get; }
        string FailureStatus { get; }

        IEnumerator<TaskStatus> DoWork();
        void ExceptionCatch(Exception e);
        void TaskCompleted();
    }
}
