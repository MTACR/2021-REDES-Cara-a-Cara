using System;
using UnityEngine;

namespace Network
{
    public class Manager : MonoBehaviour
    {
        private bool isHost;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (isHost)
            {
                FindObjectOfType<Host>();
            }
        }

    }
}
