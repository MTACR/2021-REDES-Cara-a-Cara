using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float interval = 30;
    private float timePassed = 0;
    public string player_name = "player";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > interval) {
            timePassed = 0;
            byte[] message = SenderParser.ParseTimeUp((byte) interval);
            
        }
    }

    public void ShowCards()
    {
        FindObjectOfType<Deck>().FlipAll();
    }
    
}
