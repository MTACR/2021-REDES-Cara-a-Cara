using Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Parser {
    public void ParseMessage(StateObject currentState, StateObject futureState) {
        byte messageType = futureState.buffer[0]; 
        switch(messageType) {
            case 0: //CONNECTION_OP
                ConnectionOpParse(currentState, futureState);
                break;
            case 1: //CARD_OP
                break;
            case 2: //MATCH_STATUS
                break;
            case 3: //TIME_UP
                break;
            case 4: //QUESTION
                break;
            case 5: //QUESTION_ANSR
                break;
            default:
                break;
        }
    }

    public void ConnectionOpParse(StateObject currentState, StateObject futureState) {
        byte opType = futureState.buffer[1];
        byte[] sender = futureState.buffer.Skip(2).Take(20).ToArray();
        string sender_name = Encoding.Default.GetString(sender);
        byte[] sentMessage = futureState.buffer.Skip(22).Take(100).ToArray();
        string sentText = Encoding.Default.GetString(sentMessage);
        switch (opType) {
            case 0: //REQUEST
                System.Console.WriteLine($"{sender_name} sent a request: {sentText}");
                //TODO
                break;
            case 1: //POSITIVE
                System.Console.WriteLine($"{sender_name} accepted your request: {sentText}");
                //TODO
                break;
            case 2: //NEGATIVE
                System.Console.WriteLine($"{sender_name} declined your request: {sentText}");
                //TODO
                break;
            case 3: //DISCONNECT
                System.Console.WriteLine($"{sender_name} disconnected: {sentText}");
                //TODO
                break;
            default:
                break;
        }
    }

    public void CardOpParse(StateObject currentState, StateObject futureState) {
        byte characterId = futureState.buffer[1];
        byte opCode = futureState.buffer[2];
        switch (opCode) {
            case 0: //CHOOSE
                System.Console.WriteLine($"{characterId} was chosen");
                //TODO
                break;
            case 1: //GUESS
                System.Console.WriteLine($"{characterId} was guessed");
                //TODO
                break;
            case 2: //UP
                System.Console.WriteLine($"{characterId} was raised");
                //TODO
                break;
            case 3: //DOWN
                System.Console.WriteLine($"{characterId} was lowered");
                //TODO
                break;
            default:
                break;
        }
    }
}
