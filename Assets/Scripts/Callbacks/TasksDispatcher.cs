using System.Collections.Generic;

namespace Callbacks
{
    public class TasksDispatcher
    {
        private static TasksDispatcher instance;
        private readonly Queue<Task> queue;
        private static readonly object locker = new object();

        private TasksDispatcher()
        {
            queue = new Queue<Task>();
        }

        public static TasksDispatcher Instance
        {
            get
            {
                lock (locker)
                    return instance ??= new TasksDispatcher();
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
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }

            return null;
        }
        
        public delegate void Task();

    }
}