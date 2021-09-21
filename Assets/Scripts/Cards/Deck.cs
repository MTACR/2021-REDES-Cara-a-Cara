using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class Deck : MonoBehaviour
    {
        [SerializeField] public CardModel[] models;
        [SerializeField] public GameObject prefab;
        private Card[] cards;
        public int chosenCard { private set; get; }

        private void Start()
        {
            cards = new Card[models.Length];
            var i = 0;
            var j = 0;
            
            foreach (var c in models)
            {
                var card = Instantiate(prefab, transform).GetComponent<Card>();
                card.Setup(c, i);
                card.transform.position += new Vector3(i % 7f * 11.5f, j * 15f, 0f);
                cards[i++] = card;

                if (i % 7 == 0)
                    j++;
            }

            chosenCard = Random.Range(0, cards.Length);
            Debug.Log("Your card is " + chosenCard);
        }

        public void FlipAll()
        {
            foreach (var card in cards) card.Flip();
        }

        public Card ChoosenCard()
        {
            return cards[chosenCard];
        }

        public Card RandomCard()
        {
            return cards[Random.Range(1, cards.Length)];
        }

        public void SelectionMode(bool isGuessing)
        {
            foreach (var card in cards) card.SetGuessing(isGuessing);
        }

        public Card GetCard(int id)
        {
            return cards[id];
        }
        
    }
}