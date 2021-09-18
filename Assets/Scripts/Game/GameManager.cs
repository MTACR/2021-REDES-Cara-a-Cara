using Cards;
using Network;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public GameObject scrollView;
        [SerializeField] public GameObject message;
        [SerializeField] public GameObject myCard;
        [SerializeField] public GameObject errorOvrl;
        [SerializeField] public TextMeshProUGUI errorText;
        [SerializeField] public TextMeshProUGUI turnText;
        private Client client;
        public bool myTurn { get; private set; }
        private Deck deck;
        public bool canClick;

        private void Awake()
        {
            deck = FindObjectOfType<Deck>();
            client = Client.Instance;
            client.SetListeners(() =>
            {
                
            }, s =>
            {
                errorText.text = s;
                errorOvrl.SetActive(true);
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
            Card model = deck.ChoosenCard();
            Card card = myCard.GetComponent<Card>();
            card.Setup(model.model, deck.chosenCard);
        }

        public void RandomCard()
        {
            Card model = deck.RandomCard();
            Card card = myCard.GetComponent<Card>();
            card.Setup(model.model, deck.chosenCard);
        }

        public void Guess()
        {
            if (!myTurn) return;
            
            canClick = false;
        }

        public void ShowGiveUp()
        {
            canClick = false;
        }

        public void DoGiveUp()
        {
            
        }

        public void StartMatch() {
            //TODO
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

        public void EndMatch(Status status) {
            //TODO
            switch ((Status) status) {
                case Status.Win: //WIN
                    Debug.Log($"Match was won");
                    //TODO
                    break;
                case Status.Lose: //LOSE          
                    Debug.Log($"Match was lost");
                    //TODO
                    break;
                case Status.Tie: //TIE
                    Debug.Log($"Match was tied");
                    //TODO
                    break;
                case Status.End: //END
                    Debug.Log($"Match was ended");
                    //TODO
                    break;
                default:
                    break;
            }
        }

        public void ReturnHome() {
            //TODO: loading para o Home
        }
    }
}
