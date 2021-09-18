using System;
using System.Collections.Generic;
using Game;
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
        private Dictionary<int, Message> messages;
        private Message lastMessage;
        private GameManager manager;

        private void Awake()
        {
            client = Client.Instance;
            messages = new Dictionary<int, Message>();
            manager = FindObjectOfType<GameManager>();
        }

        public void ShowMessage(int id, string sender, string text)
        {
            Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            message.Setup(id, sender, text);
            messages[id] = message;
            lastMessage = message;
        }
        
        private void ShowMessage(string sender, string text)
        {
            Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            int id = message.Setup(sender, text);
            messages[id] = message;
            lastMessage = message;
        }

        public void SendMessage()
        {
            if (!manager.myTurn) return;
            
            string message = msg.text.Trim();

            if (message.Length <= 0) return;
            
            if (!message.EndsWith("?"))
                message += "?";

            msg.Select();
            msg.text = "";
            ShowMessage("Me", message);
            client.Send(SenderParser.ParseQuestion(lastMessage.id, message));
        }

        public void ReactToMessage(int id, Answer answer)
        {
            if (!manager.myTurn) return;
            
            messages[id].React(answer);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }
        
    }
}
