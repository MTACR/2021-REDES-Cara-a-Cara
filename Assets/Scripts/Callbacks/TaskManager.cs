using UnityEngine;

namespace Callbacks
{
    public class TaskManager : MonoBehaviour
    {
        private TasksDispatcher dispatcher;

        private void Start()
        {
            dispatcher = TasksDispatcher.Instance;
        }


        private void Update()
        {
            TasksDispatcher.Task current = dispatcher.Dequeue();
            
            float timeout = Time.realtimeSinceStartup + 0.04f;
            
            while(current != null)
            {
                current();
                
                if (Time.realtimeSinceStartup > timeout)
                    break;

                current = dispatcher.Dequeue();
            }
        }
        
    }
}