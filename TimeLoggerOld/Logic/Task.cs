using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MimiJson;

namespace TimeLoggerOld
{
    public class Task : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        private static Dictionary<string, Task> Tasks = new Dictionary<string, Task>();

        private string _id;
        private string _name;
        private bool _archived;
        private List<Label> _tags = new List<Label>();
        
        public string ID
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(ID));
                }
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }
        public bool Archived
        {
            get => _archived;
            set
            {
                if (_archived != value)
                {
                    _archived = value;
                    NotifyPropertyChanged(nameof(Archived));
                }
            }
        }
        public string Duration => TaskPeriod.TaskDuration(this).ToJira();
        public IEnumerable<Label> Tags => _tags;

        public string TagsString
        {
            get
            {
                var sb = new StringBuilder();
                for (var i = 0; i < _tags.Count; i++)
                {
                    sb.Append(_tags[i].Name);
                    if (i < _tags.Count - 1)
                        sb.Append(", ");
                }
                return sb.ToString();
            }
        }

        public List<TaskPeriod> Periods
        {
            get
            {
                var result = TaskPeriod.TaskPeriods(this);
                result.Sort((a1, a2) => { return a1.Start.CompareTo(a2.Start); });
                return result;
            }
        }

        public static Task GetById(string id)
        {
            if (Tasks.ContainsKey(id))
                return Tasks[id];
            Task result = new Task(id);
            Tasks.Add(id, result);
            return result;
        }

        public static List<Task> GetAllTasks(bool withArchived = true)
        {
            List<Task> tasks = new List<Task>();
            tasks.AddRange(Tasks.Values.Where((t) => !t.Archived || withArchived));
            return tasks;
        }

        private Task(string id, string name = "New Task")
        {
            ID = id;
            Name = name;
        }

        public bool AddTag(Label tag, bool silent = false)
        {
            if (_tags.Contains(tag)) return false;
            _tags.Add(tag);
            _tags.Sort();
            tag.AddTask(this);
            if (!silent)
            {
                NotifyPropertyChanged("Tags");
                NotifyPropertyChanged("TagsString");
            }
            return true;
        }

        public bool RemoveTag(Label tag, bool silent = false)
        {
            if (!_tags.Contains(tag)) return false;
            _tags.Remove(tag);
            tag.RemoveTask(this);
            if (!silent)
            {
                NotifyPropertyChanged("Tags");
                NotifyPropertyChanged("TagsString");
            }
            return true;
        }

        public void SetTags(IEnumerable<Label> tags, bool silent = false)
        {
            while (_tags.Count > 0)
                RemoveTag(_tags[0], true);
            foreach (var tag in tags)
                AddTag(tag, true);
            if (!silent)
            {
                NotifyPropertyChanged("Tags");
                NotifyPropertyChanged("TagsString");
            }
        }

        public Task(string id, string name, params string[] tags)
        {
            ID = id;
            Name = name;
            _tags = new List<Label>(tags.Select(t => new Label(t)));
        }

        private Task(JsonValue task)
        {
            ID = task["id"];
            Name = task["name"];
            if (task.Object.ContainsKey("archived"))
                Archived = task["archived"];
            if (task.Object.ContainsKey("tags"))
                SetTags(task["tags"].Array.Select(t => Label.GetLabelByName(t)));
        }

        public JsonValue ToJson()
        {
            return new JsonValue(new JsonObject(
                new JOPair("id", ID),
                new JOPair("name", Name),
                new JOPair("archived", Archived),
                new JOPair("tags", new JsonArray(_tags.Select(t => t.Name).DynamicCast<JsonValue>()))
                ));
        }

        public static void LoadAll()
        {
            Tasks = new Dictionary<string, Task>();

            JsonValue json = JsonValue.ParseFile(Settings.TasksPath, Encoding.UTF8);

            if (json.Type == JsonValueType.Object && json.Object.ContainsKey("tasks"))
            {
                foreach (JsonValue jtask in json["tasks"].Array)
                {
                    Task task = new Task(jtask);
                    Tasks.Add(task.ID, task);
                }
                if (json.Object.ContainsKey("periods"))
                    TaskPeriod.Load(json["periods"]);
            }
        }

        public static void SaveAll()
        {
            JsonValue result = new JsonValue(new JsonObject(
                new JOPair("tasks", new JsonArray())));
            foreach (Task task in Tasks.Values)
            {
                result["tasks"].Array.Add(task.ToJson());
            }
            result.Object.Add("periods", TaskPeriod.Save());
            System.IO.File.WriteAllText(Settings.TasksPath, result.ToString(), Encoding.UTF8);
        }

    }
}
