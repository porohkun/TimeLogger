using System;
using System.Collections.Generic;
using System.Data;
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
using MimiJson;
using System.Net;
using System.IO;

namespace TimeLoggerOld
{
    /// <summary>
    /// Interaction logic for JiraWindow.xaml
    /// </summary>
    public partial class JiraWindow : Window
    {
        public JiraWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var data = new Dictionary<string, Dictionary<Task, TimeSpan>>();

            foreach (var task in Task.GetAllTasks(false))
            {
                var req = WebRequest.Create("https://jira...." + task.ID);
                req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("xxx:xxx")));
                var res = req.GetResponse();
                string text;
                using (var reader = new StreamReader(res.GetResponseStream()))
                    text = reader.ReadToEnd();
                var json = JsonValue.Parse(text);
                var worklogs = json["fields"]["worklog"]["worklogs"];

                foreach (var log in worklogs)
                {
                    var started = Jira.ParseDate(log["started"]).Date.ToString("yyyy-MM-dd");
                    if (!data.ContainsKey(started)) data.Add(started, new Dictionary<Task, TimeSpan>());

                    if (!data[started].ContainsKey(task)) data[started].Add(task, new TimeSpan());
                    var daydata = data[started];

                    daydata[task] += new TimeSpan(0, 0, log["timeSpentSeconds"]);
                }
            }

            var table = new DataTable();
            var tasks = Task.GetAllTasks(false).ToArray();
            table.Columns.Add("date");
            table.Columns.Add("total");
            foreach (var task in tasks)
                table.Columns.Add(task.ID);

            foreach (var day in data)
            {
                var row = table.Rows.Add();
                var daysum = new TimeSpan();
                var array = new object[tasks.Length + 2];
                array[0] = day.Key;
                for (int i = 0; i < tasks.Length; i++)
                {
                    var task = tasks[i];
                    if (data[day.Key].ContainsKey(task))
                    {
                        var time = data[day.Key][task];
                        daysum += time;
                        array[i + 2] = time.ToJira();
                    }
                    else
                        array[i + 2] = "";
                }
                array[1] = daysum;
                row.ItemArray = array;
            }

            grid.DataContext = table.DefaultView;
        }
    }
}
