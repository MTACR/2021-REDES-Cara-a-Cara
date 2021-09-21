using Game;
using Network;
using TMPro;
using UnityEngine;

namespace Cards
{
    public class Card : MonoBehaviour
    {
        [SerializeField] public MeshRenderer picture;
        [SerializeField] public TextMeshPro text;
        [SerializeField] public CardModel model;
        private int id;
        private Animator animator;
        private Client client;
        private float cooldown;
        private bool isVisible;
        public bool isGuessing;
        private GameManager manager;

        private void Start()
        {
            manager = FindObjectOfType<GameManager>();
            animator = GetComponent<Animator>();
            cooldown = Time.time;
            client = Client.Instance;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || !manager.canClick) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit)) return;

            var c = hit.collider.gameObject.transform.parent.GetComponent<Card>();

            if (!c) return;
            if (!c.name.Equals(name) || c.id != id) return;
            
            if (isGuessing && isVisible)
            {
                Debug.Log("Guess card " + id);
                client.Send(SenderParser.Card(id, Network.Card.Guess));
                FindObjectOfType<Deck>().SelectionMode(false);
            }
            else
            {
                if (!(cooldown < Time.time)) return;
                
                cooldown = Time.time + 0.7f;
                Flip();
                client.Send(SenderParser.Card(id, isVisible ? Network.Card.Up : Network.Card.Down));
            }
        }

        public void Setup(CardModel card, int id)
        {
            model = card;
            name = card.name;
            this.id = id;
            picture.GetComponent<Renderer>().material.mainTexture = card.texture;
            text.text = card.name;
            transform.GetChild(0).name = card.name;
        }

        public void Setup(int id)
        {
            this.id = id;
        }

        public void Flip()
        {
            animator.Play(isVisible ? "card_down" : "card_up");
            isVisible = !isVisible;
        }

        public void Flip(bool isVisible)
        {
            this.isVisible = isVisible;
            animator.Play(isVisible ? "card_down" : "card_up");
        }

    }
}