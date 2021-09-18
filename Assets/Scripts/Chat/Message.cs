using System;
using Game;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chat
{
    public class Message : MonoBehaviour
    {
        [SerializeField] public GameObject pannel;
        [SerializeField] public Image image;
        [SerializeField] public Sprite yes;
        [SerializeField] public Sprite no;
        [SerializeField] public Sprite reject;
        [SerializeField] public EventTrigger onEnter;
        [SerializeField] public EventTrigger onExit;
        [SerializeField] public RectTransform container;
        [SerializeField] public TextMeshProUGUI sender;
        [SerializeField] public TextMeshProUGUI message;
        [SerializeField] public bool isMine;
        private GameManager manager;
        public int id { get; private set; }

        private void Start()
        {
            manager = FindObjectOfType<GameManager>();
        }

        public void Setup(int id, string sender, string message)
        {
            this.id = id;
            Setup(false, sender, message, id);
        }

        public int Setup(string sender, string message)
        {
            id = GetHashCode();
            Setup(true, sender, message, id);
            return id;
        }

        private void Setup(bool isMine, string sender, string message, int id)
        {
            this.isMine = isMine;
            this.id = id;

            if (isMine)
            {
                container.offsetMin = new Vector2(50, container.offsetMin.y);
                container.offsetMax = new Vector2(0, container.offsetMax.y);
            }
            else
            {
                container.offsetMin = new Vector2(0, container.offsetMin.y);
                container.offsetMax = new Vector2(-50, container.offsetMax.y);
            }

            onEnter.enabled = !isMine;
            onExit.enabled = !isMine;

            this.sender.text = sender;
            this.message.text = message;
        }

        private void OnAnswer(Answer answer)
        {
            React(answer);
            pannel.SetActive(false);
            onEnter.enabled = false;
            onExit.enabled = false;

            if (isMine) return;

            var client = Client.Instance;
            client.Send(SenderParser.Answer(id, answer));
            manager.SetTurn(answer == Answer.Unclear ? client.opId : client.myId);
        }

        public void React(Answer answer)
        {
            image.sprite = answer switch
            {
                Answer.Confirm => yes,
                Answer.Deny => no,
                Answer.Unclear => reject,
                _ => throw new ArgumentOutOfRangeException(nameof(answer), answer, null)
            };
        }

        public void MouseEnter()
        {
            if (manager.canClick)
                pannel.SetActive(true);
        }

        public void MouseExit()
        {
            if (manager.canClick)
                pannel.SetActive(false);
        }

        public void Yes()
        {
            if (manager.canClick)
                OnAnswer(Answer.Confirm);
        }

        public void No()
        {
            if (manager.canClick)
                OnAnswer(Answer.Deny);
        }

        public void Reject()
        {
            if (manager.canClick)
                OnAnswer(Answer.Unclear);
        }
    }
}