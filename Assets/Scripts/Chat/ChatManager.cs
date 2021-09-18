using System.Collections.Generic;
using Game;
using Network;
using TMPro;
using UnityEngine;

namespace Chat
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] public GameObject container;
        [SerializeField] public GameObject prefab;
        [SerializeField] public TMP_InputField msg;
        private Client client;
        private Message lastMessage;
        private GameManager manager;
        private Dictionary<int, Message> messages;

        private void Awake()
        {
            client = Client.Instance;
            messages = new Dictionary<int, Message>();
            manager = FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        public void ShowMessage(int id, string sender, string text)
        {
            var message = Instantiate(prefab, container.transform).GetComponent<Message>();
            message.Setup(id, sender, text);
            messages[id] = message;
            lastMessage = message;
        }

        private void ShowMessage(string sender, string text)
        {
            var message = Instantiate(prefab, container.transform).GetComponent<Message>();
            var id = message.Setup(sender, text);
            messages[id] = message;
            lastMessage = message;
        }

        public void SendMessage()
        {
            if (!manager.myTurn) return;

            var message = msg.text.Trim();

            if (message.Length <= 0) return;

            if (!message.EndsWith("?"))
                message += "?";

            msg.Select();
            msg.text = "";
            ShowMessage("Me", message);
            client.Send(SenderParser.Question(lastMessage.id, message));
            manager.SetTurn(client.opId);
        }

        public void ReactToMessage(int id, Answer answer)
        {
            messages[id].React(answer);
        }
    }
}