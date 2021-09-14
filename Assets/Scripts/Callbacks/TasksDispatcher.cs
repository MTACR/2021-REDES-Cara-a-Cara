using System.Collections.Generic;

namespace Callbacks
{
    public class TasksDispatcher
    {
        private static TasksDispatcher instance;
        private readonly Queue<Task> queue;
        private readonly object locker;

        private TasksDispatcher()
        {
            queue = new Queue<Task>();
            locker = new object();
        }

        public static TasksDispatcher Instance => instance ??= new TasksDispatcher();

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