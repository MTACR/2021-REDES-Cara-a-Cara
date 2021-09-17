using System;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

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

        public void Setup(bool isMine, string sender, string message)
        {
            this.isMine = isMine;
            
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

            this.sender.text = isMine ? "Me" : sender;
            this.message.text = message;
        }
        
        private void OnAnswer(Answer answer)
        {
            React(answer);
            pannel.SetActive(false);
            onEnter.enabled = false;
            onExit.enabled = false;
            
            if (!isMine)
                Client.Instance.Send(SenderParser.ParseAnswer("EU", answer, "resposta?"));
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
            pannel.SetActive(true);
        }
    
        public void MouseExit()
        {
            pannel.SetActive(false);
        }

        public void Yes()
        {
            OnAnswer(Answer.Confirm);
        }

        public void No()
        {
            OnAnswer(Answer.Deny);
        }

        public void Reject()
        {
            OnAnswer(Answer.Unclear);
        }

    }
}
