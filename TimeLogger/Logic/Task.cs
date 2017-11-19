using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimiJson;

namespace TimeLogger
{
    public class Task
    {
        private static Dictionary<string, Task> Tasks = new Dictionary<string, Task>();

        public string ID { get; private set; }
        public string Name { get;  set; }
        public string IDName { get { return string.Format("[{0}] {1}", ID, Name); } }
        public bool Archived { get; set; }
        public bool UnArchived { get { return !Archived; } }
        public string Duration { get { return TaskPeriod.TaskDuration(this).ToJira(); } }
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

        private Task(string id, string name = "")
        {
            ID = id;
            Name = name;
        }

        private Task(JsonValue task)
        {
            ID = task["id"];
            Name = task["name"];
            if (task.Object.ContainsKey("archived"))
                Archived = task["archived"];
        }

        public JsonValue ToJson()
        {
            return new JsonValue(new JsonObject(
                new JOPair("id", ID),
                new JOPair("name", Name),
                new JOPair("archived", Archived)
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
