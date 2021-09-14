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
                CardOpParse(currentState, futureState);
                break;
            case 2: //MATCH_STATUS
                StatusOpParse(currentState, futureState);
                break;
            case 3: //TIME_UP
                TimeUpParse(currentState, futureState);
                break;
            case 4: //QUESTION
                QuestionParse(currentState, futureState);
                break;
            case 5: //QUESTION_ANSR
                QuestionAnswerParse(currentState, futureState);
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
                Debug.Log($"{sender_name} sent a request: {sentText}");
                //TODO
                break;
            case 1: //POSITIVE
                Debug.Log($"{sender_name} accepted your request: {sentText}");
                //TODO
                break;
            case 2: //NEGATIVE
                Debug.Log($"{sender_name} declined your request: {sentText}");
                //TODO
                break;
            case 3: //DISCONNECT
                Debug.Log($"{sender_name} disconnected: {sentText}");
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
                Debug.Log($"{characterId} was chosen");
                //TODO
                break;
            case 1: //GUESS
                Debug.Log($"{characterId} was guessed");
                //TODO
                break;
            case 2: //UP
                Debug.Log($"{characterId} was raised");
                //TODO
                break;
            case 3: //DOWN
                Debug.Log($"{characterId} was lowered");
                //TODO
                break;
            default:
                break;
        }
    }

    public void StatusOpParse(StateObject currentState, StateObject futureState) {
        byte status = futureState.buffer[1];
        switch (status) {
            case 0: //START
                Debug.Log($"Match was started");
                //TODO
                break;
            case 1: //WIN
                Debug.Log($"Match was won");
                //TODO
                break;
            case 2: //UP
                Debug.Log($"Match was lost");
                //TODO
                break;
            case 3: //TIE
                Debug.Log($"Match was tied");
                //TODO
                break;
            case 5: //END
                Debug.Log($"Match was ended");
                //TODO
                break;
            default:
                break;
        }
    }

    public void TimeUpParse(StateObject currentState, StateObject futureState) {
        byte time = futureState.buffer[1];

        Debug.Log($"{time}s passed");
        //TODO
    }

    public void QuestionParse(StateObject currentState, StateObject futureState) {
        byte[] sender = futureState.buffer.Skip(1).Take(20).ToArray();
        string sender_name = Encoding.Default.GetString(sender);
        byte[] questionMessage = futureState.buffer.Skip(21).Take(100).ToArray();
        string questionText = Encoding.Default.GetString(questionMessage);

        Debug.Log($"{sender_name} asked {questionText}");
        //TODO
    }

    public void QuestionAnswerParse(StateObject currentState, StateObject futureState) {
        byte[] sender = futureState.buffer.Skip(1).Take(20).ToArray();
        string sender_name = Encoding.Default.GetString(sender);
        byte agreement = futureState.buffer[22];
        byte[] answerMessage = futureState.buffer.Skip(22).Take(100).ToArray();
        string answernText = Encoding.Default.GetString(answerMessage);

        string agreementText;
        switch (agreement) {
            case 0: //CONFIRMED
                agreementText = "confirmed";
                break;
            case 1: //DENIED
                agreementText = "denied";
                break;
            default://NEITHER
                agreementText = "left uncler";
                break;
        }

        Debug.Log($"{sender_name} {agreementText}: {answernText}");
        //TODO
    }
}
