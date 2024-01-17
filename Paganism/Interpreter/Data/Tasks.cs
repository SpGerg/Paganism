using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data
{
    public static class Tasks
    {
        public static List<Task> CurrentTasks { get; } = new List<Task>();

        public static void Add(Task task)
        {
            CurrentTasks.Add(task);
        }

        public static void Remove(Task task)
        {
            _ = CurrentTasks.Remove(task);
        }

        public static Task Get(int id)
        {
            return CurrentTasks.Find(task => task.Id == id);
        }

        public static int Count()
        {
            return CurrentTasks.Count;
        }

        public static void Clear()
        {
            foreach (Task task in CurrentTasks)
            {
                if (task.Status is TaskStatus.Running)
                {
                    continue;
                }

                task.Dispose();
            }

            CurrentTasks.Clear();
        }
    }
}
