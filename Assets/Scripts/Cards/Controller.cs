using System.Collections;
using System.Collections.Generic;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Controller : MonoBehaviour
{
    public Card card;
    public MeshRenderer picture;
    public TextMeshPro name;
    
    void Start()
    {
        picture.GetComponent<Renderer>().material.mainTexture = card.texture;
        name.text = card.name;
    }

    void Update()
    {
        
    }
    
}
