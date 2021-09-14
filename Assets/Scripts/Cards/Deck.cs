using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cards
{
    public class Deck : MonoBehaviour
    {
        public Card[] objs;
        public GameObject prefab;
        private Controller[] cards;

        private void Start()
        {
            Shuffle(objs);
            cards = new Controller[objs.Length];
            int i = 0;
            int j = 0;
            foreach (var c in objs)
            {
                Controller card = Instantiate(prefab).GetComponent<Controller>();
                card.Setup(c, i);
                card.transform.position = new Vector3((i % 7f) * 7.5f - 30f, j * 10f, 0f);
                cards[i++] = card;

                if (i % 7 == 0)
                    j++;
            }
        }
        
        private static void Shuffle(IList<Card> array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                Card tmp = array[i];
                int r = Random.Range(i, array.Count);
                array[i] = array[r];
                array[r] = tmp;
            }
        }
        
    }
}