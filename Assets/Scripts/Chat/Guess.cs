using TMPro;
using UnityEngine;

namespace Chat
{
    public class Guess : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI card;
        [SerializeField] private TextMeshProUGUI sender;

        public void SetMessage(string sender, string card)
        {
            this.sender.text = sender;
            this.card.text = "Guessed card " + card;
        }
    
    }
}
