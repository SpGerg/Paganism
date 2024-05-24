using System.Collections.Generic;
using System.Linq;
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
            CurrentTasks.Remove(task);
        }

        public static bool TryGet(int id, out Task task)
        {
            foreach (var task1 in CurrentTasks)
            {
                if (task1.Id == id)
                {
                    task = task1;
                    return true;
                }
            }

            task = null;
            return false;
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
            foreach (var task in CurrentTasks)
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
