using Network;
using TMPro;
using UnityEngine;

namespace Cards
{
    public class Card : MonoBehaviour
    {
        public MeshRenderer picture;
        public TextMeshPro text;
        private Animator animator;
        private bool isVisible;
        private float cooldown;
        private int id;
        private int ID => id;

        private void Start()
        {
            animator = GetComponent<Animator>();
            cooldown = Time.time;
            isVisible = true;
        }

        public void Setup(CardModel card, int id)
        {
            this.name = card.name;
            this.id = id;
            picture.GetComponent<Renderer>().material.mainTexture = card.texture;
            text.text = card.name;
            transform.GetChild(0).name = card.name;
        }
        
        public void Setup(int id)
        {
            this.id = id;
        }

        void Update()
        {
            if (!Input.GetMouseButton(0)) return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (!Physics.Raycast(ray, out hit)) return;

            Card c = hit.collider.gameObject.transform.parent.GetComponent<Card>();

            if (!c) return;
            if (!c.name.Equals(this.name) || c.ID != id || !(cooldown < Time.time)) return;
            
            cooldown = Time.time + 0.7f;
            Flip();
            FindObjectOfType<Client>().Send(id+ " " + isVisible);
        }

        private void Flip()
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
