using TMPro;
using UnityEngine;

namespace Cards
{
    public class Controller : MonoBehaviour
    {
        public Card card;
        public MeshRenderer picture;
        public TextMeshPro text;
    
        void Start()
        {
            picture.GetComponent<Renderer>().material.mainTexture = card.texture;
            text.text = card.text;
        }

        void Update()
        {
        
        }
    
    }
}
