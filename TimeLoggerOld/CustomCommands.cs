using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TimeLoggerOld
{
    static class CustomCommands
    {
        public static RoutedCommand Archive = new RoutedCommand();
        public static RoutedCommand UnArchive = new RoutedCommand();
        public static RoutedCommand ShowPeriods = new RoutedCommand();
        public static RoutedCommand Edit = new RoutedCommand();
    }
}
