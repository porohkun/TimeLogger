using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class SelectTaskWindowDummy
    {
        public bool ShowArchive { get; set; }
        public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();
        public ObservableCollection<Label> Tags { get; set; } = new ObservableCollection<Label>();

        public SelectTaskWindowDummy()
        {
            Tasks.Add(new Task("ttt1", "task_01", "aaa", "bbb"));
            Tasks.Add(new Task("ttt2", "task_02", "aaa", "ccc"));
            Tasks.Add(new Task("ttt3", "task_03", "aaa", "ddd"));
            Tasks.Add(new Task("ttt4", "task_04", "bbb", "ccc"));
            Tasks.Add(new Task("ttt5", "task_05", "bbb", "ddd"));

            Tags.Add(new Label("aaa"));
            Tags.Add(new Label("bbb"));
            Tags.Add(new Label("ccc"));
            Tags.Add(new Label("ddd"));
        }
    }

    /// <summary>
    /// Interaction logic for SelectTaskWindow.xaml
    /// </summary>
    public partial class SelectTaskWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private bool _showArchive;
        private bool _editTask = true;
        private ObservableCollection<Task> _tasks;
        private CollectionView _tasksView;
        private CollectionView _tagsView;
        private Task _selectedTask;
        private string _selectedTaskName;

        public bool ShowArchive
        {
            get => _showArchive;
            set
            {
                if (_showArchive != value)
                {
                    _showArchive = value;
                    EditTask = false;
                    NotifyPropertyChanged(nameof(ShowArchive));
                    Label.UnselectAll();
                    RefreshView();
                }
            }
        }
        public bool EditTask
        {
            get => _editTask;
            set
            {
                if (_editTask != value)
                {
                    _editTask = value;
                    NotifyPropertyChanged(nameof(EditTask));
                }
            }
        }
        public ObservableCollection<Task> Tasks
        {
            get
            {
                if (_tasks == null)
                {
                    _tasks = new ObservableCollection<Task>(Task.GetAllTasks());
                    SortTasks();
                }
                return _tasks;
            }
        }

        private void SortTasks()
        {
            _tasks.Sort((Comparison<Task>)((t1, t2) =>
            {
                if (t1.Periods.Count == 0 && t2.Periods.Count == 0)
                    return t1.Name.CompareTo(t2.Name);
                else if (t1.Periods.Count == 0)
                    return -1;
                else if (t2.Periods.Count == 0)
                    return 1;
                else
                    return -t1.Periods.Last().End.CompareTo(t2.Periods.Last().End);
            }));
        }

        public ObservableCollection<Label> Tags => Label.Labels;
        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask != value)
                {
                    _selectedTask = value;
                    EditTask = false;
                    NotifyPropertyChanged(nameof(SelectedTask));
                }
            }
        }
        public string SelectedTaskName
        {
            get => _selectedTaskName;
            set
            {
                if (_selectedTaskName != value)
                {
                    _selectedTaskName = value;
                    NotifyPropertyChanged(nameof(SelectedTaskName));
                }
            }
        }
        public ObservableCollection<Label> SelectedTaskTags { get; set; } = new ObservableCollection<Label>();


        public SelectTaskWindow()
        {
            InitializeComponent();
            DataContext = this;

            _tasksView = (CollectionView)CollectionViewSource.GetDefaultView(Tasks);
            _tasksView.Filter = TaskFilter;
            _tagsView = (CollectionView)CollectionViewSource.GetDefaultView(Tags);
            _tagsView.Filter = TagFilter;
            RefreshView();
        }

        private void RefreshView()
        {
            _tasksView.Refresh();
            SortTasks();
            _tagsView.Refresh();
        }

        private bool TaskFilter(object item)
        {
            if (!(item is Task task)) return false;
            if (ShowArchive != task.Archived)
                return false;
            foreach (var label in Label.Labels.Where(l => l.Selected))
                if (!task.Tags.Contains(label))
                    return false;
            return true;
        }

        private bool TagFilter(object item)
        {
            if (!(item is Label label)) return false;
            if (ShowArchive)
                return label.ArchivedCount > 0;
            else
                return label.Count > label.ArchivedCount;
        }

        private void taskName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            EditTask = false;
        }

        void ListBox_PreviewRightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CommandBinding_Archive(object sender, ExecutedRoutedEventArgs e)
        {
            (e.Parameter as Task).Archived = true;
            RefreshView();
        }

        private void CommandBinding_UnArchive(object sender, ExecutedRoutedEventArgs e)
        {
            (e.Parameter as Task).Archived = false;
            RefreshView();
        }

        private void CommandBinding_ShowPeriods(object sender, ExecutedRoutedEventArgs e)
        {
            var task = e.Parameter as Task;
            var win = new PeriodsWindow(task);
            win.Owner = this;
            win.ShowDialog();
        }

        private void CommandBinding_Edit(object sender, ExecutedRoutedEventArgs e)
        {
            EditTask = true;
            SelectedTaskName = SelectedTask.Name;
            SelectedTaskTags.Clear();
            SelectedTaskTags.AddRange(SelectedTask.Tags);
            TaskNameTextBox.Focus();
            TaskNameTextBox.SelectAll();
        }

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Label.UnselectAll();
            MainWindow.SaveAll();
        }

        private void TagsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Label label in e.AddedItems)
                label.Selected = true;
            foreach (Label label in e.RemovedItems)
                label.Selected = false;
            EditTask = false;
            RefreshView();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedTask = ((ListViewItem)sender).Content as Task;
            DialogResult = true;
            Close();
        }

        private void ApplyTaskEdit_Click(object sender, RoutedEventArgs e)
        {
            EditTask = false;
            SelectedTask.Name = SelectedTaskName;
            SelectedTask.SetTags(SelectedTaskTags);
            RefreshView();
        }

        private void CancelTaskEdit_Click(object sender, RoutedEventArgs e)
        {
            EditTask = false;
        }

        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            var task = Task.GetById(Guid.NewGuid().ToString());
            Tasks.Add(task);
            _tasksView.Refresh();
            SelectedTask = task;
            CommandBinding_Edit(null, null);
        }
    }
}
