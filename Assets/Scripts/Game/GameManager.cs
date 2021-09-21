using System;
using Callbacks;
using Cards;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Card = Cards.Card;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public GameObject scrollView;
        [SerializeField] public GameObject message;
        [SerializeField] public GameObject myCard;
        [SerializeField] public GameObject errorOvrl;
        [SerializeField] public GameObject resultOvrl;
        [SerializeField] public TextMeshProUGUI errorText;
        [SerializeField] public TextMeshProUGUI turnText;
        [SerializeField] public TextMeshProUGUI resultText;
        public bool canClick;
        private bool isGuessing;
        private Client client;
        private Deck deck;
        public bool myTurn { get; private set; }

        private void Awake()
        {
            deck = FindObjectOfType<Deck>();
            client = Client.Instance;
            client.SetListeners(() => { }, s =>
            {
                canClick = false;
                errorText.text = s;
                errorOvrl.SetActive(true);
                resultOvrl.SetActive(false);
            });

            myTurn = !client.isHost;
            SetTurn(myTurn ? Client.Instance.myId : Client.Instance.opId);
        }

        public void ShowCards()
        {
            canClick = true;
            FindObjectOfType<Deck>().FlipAll();
        }

        public void SetCard()
        {
            var model = deck.ChoosenCard();
            var card = myCard.GetComponent<Card>();
            card.Setup(model.model, deck.chosenCard);
        }

        public void RandomCard()
        {
            var model = deck.RandomCard();
            var card = myCard.GetComponent<Card>();
            card.Setup(model.model, deck.chosenCard);
        }

        public void Guess()
        {
            if (!myTurn) return;

            isGuessing = !isGuessing;
            deck.SelectionMode(isGuessing);
            
            Debug.Log("Guess mode = " + isGuessing);
            
            turnText.text = isGuessing ? "Guess the opponent's card" : "Your turn";
        }

        public void ShowGiveUp()
        {
            canClick = false;
        }

        public void DoGiveUp()
        {
            canClick = true;
            errorOvrl.SetActive(false);
            resultOvrl.SetActive(false);
            SceneManager.LoadScene("Home");
            client.Send(SenderParser.Connection(Connection.Disconnect));
            client.Dispose();
        }

        public void SetTurn(int id)
        {
            if (id == client.myId)
            {
                myTurn = true;
                turnText.text = "Your turn";
            }
            else
            {
                myTurn = false;
                turnText.text = "Opponent's turn";
            }
        }

        public void RequireAnswer()
        {
            turnText.text = "Answer the opponent's question";
        }

        public void Unclear()
        {
            turnText.text = "Unclear message. Try a yes/no question";
        }

        public void SetMatchStatus(Status status)
        {
            canClick = false;
            errorOvrl.SetActive(false);
            
            switch (status)
            {
                case Status.Win:
                    Debug.Log("Match was won");
                    resultText.text = "You win";
                    resultOvrl.SetActive(true);
                    break;
                
                case Status.Lose:      
                    Debug.Log("Match was lost");
                    resultText.text = "You lose";
                    resultOvrl.SetActive(true);
                    break;
                
                case Status.Rematch:
                    Debug.Log("Starting new match");
                    StartCoroutine(Utils.ILoadScene("Game"));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public void Rematch()
        {
            resultText.text = "You want to rematch";
            client.Send(SenderParser.Status(Status.Rematch));
        }

        public void OpponentGaveUp()
        {
            errorText.text = "Your opponent gave up";
            errorOvrl.SetActive(true);
            resultOvrl.SetActive(false);
        }

        public void ErrorOk()
        {
            SceneManager.LoadScene("Home");
            client.Dispose();
        }

    }
}