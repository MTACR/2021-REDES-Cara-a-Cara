using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "Card", menuName = "Scriptable/Card", order = 1)]
    public class Card : ScriptableObject
    {
        public Texture texture;
    }
}
