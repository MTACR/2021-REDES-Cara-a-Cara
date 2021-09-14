using UnityEngine;

namespace Cards
{
    public class Deck : MonoBehaviour
    {
        public Card[] objs;
        public GameObject prefab;
        private Controller[] cards;

        private void Start()
        {
            cards = new Controller[objs.Length];
            int i = 0;
            int j = 0;
            foreach (var c in objs)
            {
                Controller card = Instantiate(prefab).GetComponent<Controller>();
                card.Setup(c);
                card.transform.position = new Vector3((i % 7f) * 10f - 30f, j * 10f, 0f);
                cards[i++] = card;

                if (i % 7 == 0)
                    j++;
            }
        }
        
    }
}