using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimeLogger
{
    public class Label : INotifyPropertyChanged, IComparable, IComparable<Label>
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        #region IComparable, IComparable<Label> Members

        public int CompareTo(object obj)
        {
            return CompareTo((Label)obj);
        }

        public int CompareTo(Label other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion

        public static ObservableCollection<Label> Labels { get; } = new ObservableCollection<Label>();

        private string _name;
        private bool _selected;
        private readonly List<Task> _tasks;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selected"));
            }
        }
        public int Count => _tasks.Count;
        public int NotArchivedCount => _tasks.Count(t => !t.Archived);
        public int ArchivedCount => _tasks.Count(t => t.Archived);

        public Label(string name)
        {
            _tasks = new List<Task>();
            _name = name;
        }

        private Label()
        {
            _tasks = new List<Task>();
        }

        internal void AddTask(Task task)
        {
            if (_tasks.Contains(task))
                return;
            _tasks.Add(task);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        internal void RemoveTask(Task task)
        {
            if (!_tasks.Contains(task))
                return;
            _tasks.Remove(task);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        internal static Label GetLabelByName(string tag)
        {
            var label = Labels.FirstOrDefault(l => l.Name == tag);
            if (label != null)
                return label;
            label = new Label() { Name = tag };
            Labels.Add(label);
            Labels.Sort((Comparison<Label>)((l1, l2) => { return l1.Name.CompareTo(l2.Name); }));
            return label;
        }

        internal static void UnselectAll()
        {
            foreach (var label in Labels)
            {
                label.Selected = false;
            }
        }
    }
}
