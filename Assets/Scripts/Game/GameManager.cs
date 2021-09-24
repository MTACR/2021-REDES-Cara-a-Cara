using System;
using Callbacks;
using Cards;
using Chat;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Card = Cards.Card;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public GameObject myCard;
        [SerializeField] public GameObject errorOvrl;
        [SerializeField] public GameObject resultOvrl;
        [SerializeField] public TextMeshProUGUI errorText;
        [SerializeField] public TextMeshProUGUI turnText;
        [SerializeField] public TextMeshProUGUI resultText;
        public bool canClick;
        private bool isGuessing;
        private bool myRematch;
        private bool opRematch;
        public bool matchRunning { get; private set; }
        private Deck deck;
        private ChatManager chat;
        public bool myTurn { get; private set; }

        private void Awake()
        {
            if (FindObjectOfType<TaskManager>() == null)
                new GameObject().AddComponent<TaskManager>().name = "Tasks";
            
            deck = FindObjectOfType<Deck>();
            chat = FindObjectOfType<ChatManager>();
            
            Client.Instance.SetListeners(() => { }, s =>
            {
                canClick = false;
                errorText.text = s;
                errorOvrl.SetActive(true);
                resultOvrl.SetActive(false);
            });

            myTurn = !Client.Instance.isHost;
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
            Client.Instance.Send(SenderParser.Connection(Connection.Disconnect));
            Client.Instance.Dispose();
        }

        public void SetMyTurn(bool myTurn)
        {
            this.myTurn = myTurn;
            isGuessing = false;
            turnText.text = myTurn ? "Your turn" : "Opponent's turn";
        }

        public void OpponentGuess(int id)
        {
            SetMyTurn(true);
            
            if (deck.chosenCard == id)
            {
                Client.Instance.Send(SenderParser.Status(Status.Win));
                SetMatchStatus(Status.Lose);
            }
            else
            {
                Client.Instance.Send(SenderParser.Status(Status.Lose));
                SetMatchStatus(Status.Win);
            }
            
            chat.ShowGuess("Opponent", deck.GetCard(id).name);
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
                case Status.Start:
                    Debug.Log("Opponent is ready to play");
                    matchRunning = true;
                    canClick = true;
                    SetMyTurn(myTurn);
                    break;
                
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
                    opRematch = true;
                    resultText.text = "Your opponent wants to rematch";
                    DoRematch();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void DoRematch()
        {
            if (!myRematch || !opRematch) return;
            
            Debug.Log("Starting new match");
            StartCoroutine(Utils.ILoadScene("Game"));
        }

        public void Rematch()
        {
            myRematch = true;
            resultText.text = "You want to rematch";
            Client.Instance.Send(SenderParser.Status(Status.Rematch));
            DoRematch();
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
            Client.Instance?.Dispose();
        }

        public void EnableClick()
        {
            canClick = true;
        }

        public void StartMatch()
        {
            Debug.Log("I'm ready to play");
            Client.Instance.Send(SenderParser.Status(Status.Start));
        }
        
        private void OnApplicationQuit()
        {
            Client.Instance?.Dispose();
            Debug.Log("Application ending after " + Time.time + " seconds");
        }

    }
}