using System;
using System.Collections;
using TMPro;
using UnityEditor.Animations;
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

        private void Start()
        {
            animator = GetComponent<Animator>();
            cooldown = Time.time;
            isVisible = true;
        }

        public void Setup(Card card)
        {
            this.name = card.name;
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

            if (!hit.collider.gameObject.name.Equals(this.name) || !(cooldown < Time.time)) return;
            
            cooldown = Time.time + 0.7f;
            Flip();
        }

        public void Flip()
        {
            animator.Play(isVisible ? "card_down" : "card_up");
            isVisible = !isVisible;
        }

    }
}
