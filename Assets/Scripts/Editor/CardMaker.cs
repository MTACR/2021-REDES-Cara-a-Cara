using System;
using Cards;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    /*[CustomEditor(typeof(Card))]
    public class CardMaker : UnityEditor.Editor
    {
        public Card card;

        private void OnEnable()
        {
            card = (Card) target;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("Card Maker");
            
            if (GUILayout.Button("Make new"))
                Make();
        }

        private void Make()
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath("Assets/Pictures");

            foreach (var asset in assets)
            {
                card.name = asset.name;
                card.texture = asset.GetComponent<Texture>();
            }
        }
        
    }*/
}
