using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable/Card", order = 1)]
public class Card : ScriptableObject
{
    public string name;
    public Texture texture;
}
