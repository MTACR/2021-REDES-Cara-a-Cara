using System;
using UnityEngine;

namespace Cards
{
    public class DeckOpponent : MonoBehaviour
    {
        public GameObject prefab;
        private Card[] cards;
        
        void Start()
        {
            cards = new Card[28];
            int j = 0;
            for (int i = 0; i < 28; i++)
            {
                Card card = Instantiate(prefab, transform).GetComponent<Card>();
                card.Setup(i);
                card.transform.position += new Vector3((i % 7f) * 3f, j * 4f, 0f);
                cards[i] = card;
                
                if ((i + 1) % 7 == 0)
                    j++;
            }
        }

        public void Flip(int i)
        {
            cards[i].Flip();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Alpha0))
                Flip(0);
        }
    }
}
