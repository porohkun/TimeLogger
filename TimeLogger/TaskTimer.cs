using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimiJson;

namespace TimeLogger
{
    public class TaskTimer
    {
        private static TaskTimer _instance;
        public static event Action OnTick;

        public static void Begin()
        {
            _instance = new TaskTimer();
        }

        System.Timers.Timer _timer;
        public TaskTimer()
        {
            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }
        
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {


            OnTick?.Invoke();
        }
    }
}
