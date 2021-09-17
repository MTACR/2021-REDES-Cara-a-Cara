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
        private Client client;
        private Message lastMessage;

        private void Start()
        {
            client = Client.Instance;
        }

        public void ShowMessage(int id, string sender, string text)
        {
            Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            message.Setup(id, sender, text);

            lastMessage = message;
        }
        
        private void ShowMessage(string sender, string text)
        {
            Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            message.Setup(sender, text);

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
            ShowMessage("Me", message);
            client.Send(SenderParser.ParseQuestion(client.id, lastMessage.id, message));
        }

        public void ReactToMessage(Answer answer)
        {
            lastMessage.React(answer);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }
        
    }
}
