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

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for SelectTaskWindow.xaml
    /// </summary>
    public partial class SelectTaskWindow : Window
    {
        private bool _showArchive;
        public bool ShowArchive
        {
            get { return _showArchive; }
            set
            {
                _showArchive = value;
                RefreshView();
            }
        }

        public string TaskId
        {
            get { return taskName.Text; }
            set { taskName.Text = value; }
        }

        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks
        {
            get
            {
                if (_tasks == null)
                {
                    _tasks = new ObservableCollection<Task>(Task.GetAllTasks());
                    _tasks.Sort((t1, t2) => { return t1.ID.CompareTo(t2.ID); });
                }
                return _tasks;
            }
        }

        public SelectTaskWindow()
        {
            InitializeComponent();
            DataContext = this;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Tasks);
            view.Filter = TaskFilter;
        }

        private void RefreshView()
        {
            CollectionViewSource.GetDefaultView(Tasks).Refresh();
        }

        private bool TaskFilter(object item)
        {
            var task = item as Task;
            return (!task.Archived || _showArchive) && (task.ID.ToLower().Contains(TaskId.ToLower()) || task.Name.ToLower().Contains(TaskId.ToLower()));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                TaskId = (listBox.SelectedItem as Task).ID.ToString();
                DialogResult = true;
                Close();
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedItem != null)
                taskName.Text = (listBox.SelectedItem as Task).ID;
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
            win.Show();
        }

        private void CommandBinding_Edit(object sender, ExecutedRoutedEventArgs e)
        {
            var task = e.Parameter as Task;
            var win = new SetNameWindow();
            win.CurrentTask = task;
            win.Show();
        }

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainWindow.SaveAll();
        }
    }
}
