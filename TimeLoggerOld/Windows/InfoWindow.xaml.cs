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

namespace TimeLoggerOld
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        DateTime _weekStart = DateTime.Now.Date;

        public InfoWindow()
        {
            InitializeComponent();
            
            while (_weekStart.DayOfWeek != DayOfWeek.Monday)
                _weekStart = _weekStart.AddDays(-1);

            ShowWeek(_weekStart);
        }

        void ShowWeek(DateTime firstDay)
        {
            var lastDay = firstDay.AddDays(6);
            if (firstDay.Year == lastDay.Year)
                weekText.Text = string.Format("{0} - {1}",firstDay.ToString("dd.MM"), lastDay.ToString("dd.MM yyyy"));
            else
                weekText.Text = string.Format("{0} - {1}", firstDay.ToString("dd.MM yyyy"), lastDay.ToString("dd.MM yyyy"));
            day1.SetDay(firstDay);
            day2.SetDay(firstDay.AddDays(1));
            day3.SetDay(firstDay.AddDays(2));
            day4.SetDay(firstDay.AddDays(3));
            day5.SetDay(firstDay.AddDays(4));
            day6.SetDay(firstDay.AddDays(5));
            day7.SetDay(firstDay.AddDays(6));
        }

        private void PrewWeek_Click(object sender, RoutedEventArgs e)
        {
            _weekStart = _weekStart.AddDays(-7);
            ShowWeek(_weekStart);
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            _weekStart = _weekStart.AddDays(7);
            ShowWeek(_weekStart);
        }
    }
}
