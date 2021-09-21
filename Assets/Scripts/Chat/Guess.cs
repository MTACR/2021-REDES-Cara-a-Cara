using TMPro;
using UnityEngine;

namespace Chat
{
    public class Guess : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void SetMessage(string name)
        {
            text.text = "Guessed card " + name;
        }
    
    }
}
