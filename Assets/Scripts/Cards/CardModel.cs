using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "Card", menuName = "Scriptable/Card", order = 1)]
    public class CardModel : ScriptableObject
    {
        public Texture texture;
    }
}
