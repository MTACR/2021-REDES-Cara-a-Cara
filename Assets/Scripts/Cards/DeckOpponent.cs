using UnityEngine;

namespace Cards
{
    public class DeckOpponent : MonoBehaviour
    {
        [SerializeField] public GameObject prefab;
        [SerializeField] private Card[] cards;

        private void Start()
        {
            cards = new Card[28];
            var j = 0;
            for (var i = 0; i < 28; i++)
            {
                var card = Instantiate(prefab, transform).GetComponent<Card>();
                card.Setup(i);
                card.transform.position += new Vector3(i % 7f * 3f, j * 4f, 0f);
                cards[i] = card;

                if ((i + 1) % 7 == 0)
                    j++;
            }
        }

        public void Flip(int i, bool isVisible)
        {
            cards[i].Flip(isVisible);
        }
    }
}