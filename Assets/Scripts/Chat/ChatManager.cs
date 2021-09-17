using System;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chat
{
    public class ChatManager : MonoBehaviour
    {
        
        [SerializeField] public GameObject container;
        [SerializeField] public GameObject prefab;
        [SerializeField] public TMP_InputField msg;

        private Message lastMessage;

        public void ShowMessage(bool isMine, string sender, string text)
        {
            Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            message.Setup(isMine, sender, text);

            lastMessage = message;
        }

        public void SendMessage()
        {
            string message = msg.text.Trim();

            if (message.Length <= 0) return;
            
            if (!message.EndsWith("?"))
                message += "?";

            msg.Select();
            msg.text = "";
            ShowMessage(true, "Me", message);
            Client.Instance.Send(SenderParser.ParseQuestion("William", message));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }
    }
}
