using Cards;
using Network;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public float interval = 30;
        private float timePassed = 0;
        public string player_name = "player";
        private Client client;
        private bool myTurn = true;
        private Deck deck;
        public GameObject scrollView;
        public GameObject message;
        public GameObject myCard;

        void Start()
        {
            client = Client.Instance;
            deck = FindObjectOfType<Deck>();
        }

        void Update()
        {
            timePassed += Time.deltaTime;
            
            if (timePassed > interval && client.isReady && client.isHost) {
                timePassed = 0;
                byte[] message = SenderParser.ParseTimeUp((byte) interval);
                client.Send(message);
            }
        }

        public void ShowCards()
        {
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

        public void StartMatch() {
            //TODO
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
    }
}
