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
        private Animator animator;
        private bool isVisible;
        private float cooldown;
        private int ID { get; set; }
        private Client client;
        private GameManager manager;
        
        private void Start()
        {
            manager = FindObjectOfType<GameManager>();
            animator = GetComponent<Animator>();
            cooldown = Time.time;
            client = Client.Instance;
        }

        public void Setup(CardModel card, int id)
        {
            this.model = card;
            this.name = card.name;
            this.ID = id;
            picture.GetComponent<Renderer>().material.mainTexture = card.texture;
            text.text = card.name;
            transform.GetChild(0).name = card.name;
        }
        
        public void Setup(int id)
        {
            this.ID = id;
        }

        void Update()
        {
            if (!Input.GetMouseButton(0) || !manager.canClick) return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (!Physics.Raycast(ray, out hit)) return;

            Card c = hit.collider.gameObject.transform.parent.GetComponent<Card>();

            if (!c) return;
            if (!c.name.Equals(this.name) || c.ID != ID || !(cooldown < Time.time)) return;
            
            cooldown = Time.time + 0.7f;
            Flip();
            client.Send(SenderParser.ParseCardOp(Client.Instance.id, ID, isVisible ? CardOpType.Up : CardOpType.Down));
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
