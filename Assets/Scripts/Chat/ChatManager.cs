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
        [SerializeField] public GameObject messagePrefab;
        [SerializeField] public GameObject guessPrefab;
        [SerializeField] public TMP_InputField msg;
        [SerializeField] public ScrollRect scroll;
        private Message lastMessage;
        private GameManager manager;

        private void Awake()
        {
            manager = FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        public void ShowOpMessage(string sender, string text)
        {
            var message = Instantiate(messagePrefab, container.transform).GetComponent<Message>();
            message.Setup(false, sender, text);
            lastMessage = message;
            
            Canvas.ForceUpdateCanvases();
            scroll.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
        }

        private void ShowMyMessage(string sender, string text)
        {
            var message = Instantiate(messagePrefab, container.transform).GetComponent<Message>();
            message.Setup(true, sender, text);
            lastMessage = message;
            
            Canvas.ForceUpdateCanvases();
            scroll.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
        }

        public void ShowGuess(string sender, string card)
        {
            Instantiate(guessPrefab, container.transform).GetComponent<Guess>().SetMessage(sender, card);
            
            Canvas.ForceUpdateCanvases();
            scroll.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
        }

        public void SendMessage()
        {
            if (!manager.myTurn || !manager.canClick || !manager.matchRunning || manager.isGuessing) return;

            var message = msg.text.Trim();

            if (message.Length <= 0) return;

            if (!message.EndsWith("?"))
                message += "?";

            msg.Select();
            msg.text = "";
            ShowMyMessage("Me", message);
            Client.Instance.Send(SenderParser.Question(message));
            manager.SetMyTurn(false);
        }

        public void ReactToMessage(Answer answer)
        {
            lastMessage.React(answer);
        }
    }
}