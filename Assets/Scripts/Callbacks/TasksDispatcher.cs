using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class TasksDispatcher
    {
        public delegate void Task();

        private static TasksDispatcher instance;
        private static readonly object locker = new object();
        private readonly Queue<Task> queue;

        private TasksDispatcher()
        {
            queue = new Queue<Task>();
            
            if (Object.FindObjectOfType<TaskManager>() == null)
                new GameObject().AddComponent<TaskManager>().name = "Tasks";
        }

        public static TasksDispatcher Instance
        {
            get
            {
                lock (locker)
                {
                    return instance ??= new TasksDispatcher();
                }
            }
        }

        public void Schedule(Task task)
        {
            lock (locker)
            {
                queue.Enqueue(task);
            }
        }

        public Task Dequeue()
        {
            lock (locker)
            {
                if (queue.Count > 0) return queue.Dequeue();
            }

            return null;
        }
    }
}