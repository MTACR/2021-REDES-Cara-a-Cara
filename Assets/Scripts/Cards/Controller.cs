using Network;
using TMPro;
using UnityEngine;

namespace Cards
{
    public class Controller : MonoBehaviour
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

        public void Setup(Card card, int id)
        {
            this.name = card.name;
            this.id = id;
            picture.GetComponent<Renderer>().material.mainTexture = card.texture;
            text.text = card.name;
            transform.GetChild(0).name = card.name;
        }

        void Update()
        {
            if (!Input.GetMouseButton(0)) return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (!Physics.Raycast(ray, out hit)) return;

            Controller c = hit.collider.gameObject.transform.parent.GetComponent<Controller>();

            if (!c) return;
            if (!c.name.Equals(this.name) || c.ID != id || !(cooldown < Time.time)) return;
            
            cooldown = Time.time + 0.7f;
            Flip();
            FindObjectOfType<Client>().Send(id+ " " + isVisible + " <EOF>");
        }

        public void Flip()
        {
            animator.Play(isVisible ? "card_down" : "card_up");
            isVisible = !isVisible;
        }

    }
}
