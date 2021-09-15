using System.Collections;
using System.Collections.Generic;
using Cards;
using Network;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float interval = 30;
    private float timePassed = 0;
    public string player_name = "player";
    private Client client;
    private bool myTurn = true;
    public GameObject scrollView;
    public GameObject message;

    // Start is called before the first frame update
    void Start()
    {
        client = GetComponent<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > interval && client.isReady && client.isHost) {
            timePassed = 0;
            byte[] message = SenderParser.ParseTimeUp((byte) interval);
            client.Send(message);
        }


        if (Input.GetKey(KeyCode.E))
            Instantiate(message, scrollView.transform);


    }

    public void ShowCards()
    {
        FindObjectOfType<Deck>().FlipAll();
    }

    public void EndMatch(Status status) {
        //TODO
    }
}
