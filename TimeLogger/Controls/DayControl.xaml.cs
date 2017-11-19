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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for DayControl.xaml
    /// </summary>
    public partial class DayControl : UserControl
    {
        DateTime _day;

        public DayControl()
        {
            InitializeComponent();
        }

        public void SetDay(DateTime day)
        {
            _day = day;
            var nextDay = day.AddDays(1).Date;
            dayText.Text = day.ToString("dd.MM");
            grid.Children.Clear();
            grid.RowDefinitions.Clear();

            try
            {
                var last = day.Date;//.AddHours(8);
                int row = 0;
                foreach (var period in TaskPeriod.GetForDay(day))
                {
                    var start = period.Start > last ? period.Start : last;
                    var end = period.End < nextDay ? period.End : nextDay;

                    if (start > last)
                    {
                        var length = start - last;
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(length.TotalMinutes, GridUnitType.Star) });
                        row++;
                    }
                    var periodLength = end - start;
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(periodLength.TotalMinutes, GridUnitType.Star) });
                    var tb = new TextBlock
                    {
                        Text = period.Task.ID,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Background = Brushes.Lavender
                    };
                    var rect = new Rectangle { Stroke = new SolidColorBrush(Color.FromRgb(112, 112, 112)) };
                    grid.Children.Add(tb);
                    grid.Children.Add(rect);
                    Grid.SetRow(tb, row);
                    Grid.SetRow(rect, row);

                    last = end;
                    row++;
                }

                if (last < nextDay)
                {
                    var length = nextDay - last;
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(length.TotalMinutes, GridUnitType.Star) });
                }
            }
            catch
            {

            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new DayInfoWindow();
            win.ShowDay(_day);
            win.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new DayEditWindow();
            win.ShowDay(_day);
            win.ShowDialog();
        }
    }
}
