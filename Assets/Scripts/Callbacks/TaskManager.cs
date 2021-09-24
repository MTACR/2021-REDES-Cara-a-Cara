using UnityEngine;

namespace Callbacks
{
    public class TaskManager : MonoBehaviour
    {
        private TasksDispatcher dispatcher;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            dispatcher = TasksDispatcher.Instance;
        }

        private void Update()
        {
            var task = dispatcher.Dequeue();

            var timeout = Time.realtimeSinceStartup + 0.04f;

            while (task != null)
            {
                task();

                if (Time.realtimeSinceStartup > timeout)
                    break;

                task = dispatcher.Dequeue();
            }
        }
    }
}