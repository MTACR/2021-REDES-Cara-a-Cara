using Callbacks;
using Cards;
using Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;

public class Parser {
    public void ParseMessage(StateObject currentState, StateObject futureState) {
        byte messageType = futureState.buffer[0]; 
        switch((MessageType) messageType) {
            case MessageType.ConnectionOp: //CONNECTION_OP
                ConnectionOpParse(currentState, futureState);
                break;
            case MessageType.CardOp: //CARD_OP
                CardOpParse(currentState, futureState);
                break;
            case MessageType.Status: //MATCH_STATUS
                StatusOpParse(currentState, futureState);
                break;
            case MessageType.TimeUp: //TIME_UP
                TimeUpParse(currentState, futureState);
                break;
            case MessageType.Question: //QUESTION
                QuestionParse(currentState, futureState);
                break;
            case MessageType.Answer: //QUESTION_ANSR
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
        switch ((ConnectionType) opType) {
            case ConnectionType.Request: //REQUEST
                Debug.Log($"{sender_name} sent a request: {sentText}");
                //TODO
                break;
            case ConnectionType.Positive: //POSITIVE
                Debug.Log($"{sender_name} accepted your request: {sentText}");
                //TODO
                break;
            case ConnectionType.Negative: //NEGATIVE
                Debug.Log($"{sender_name} declined your request: {sentText}");
                //TODO
                break;
            case ConnectionType.Disconnect: //DISCONNECT
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
        switch ((CardOpType) opCode) {
            case CardOpType.Choose: //CHOOSE
                Debug.Log($"{characterId} was chosen");
                //TODO
                break;
            case CardOpType.Guess: //GUESS
                Debug.Log($"{characterId} was guessed");
                TasksDispatcher.Instance.Schedule(delegate {
                    if (UnityEngine.Object.FindObjectOfType<Deck>().IsChosen(characterId)) {
                        byte[] messase = SenderParser.ParseStatus(Status.Win);
                        UnityEngine.Object.FindObjectOfType<Client>().Send(messase);
                        UnityEngine.Object.FindObjectOfType<GameManager>().EndMatch(Status.Lose);
                    }
                });
                //TODO?
                break;
            case CardOpType.Up: //UP
                Debug.Log($"{characterId} was raised");
                TasksDispatcher.Instance.Schedule(delegate {
                    UnityEngine.Object.FindObjectOfType<DeckOpponent>().Flip(characterId, true);
                });
                //TODO?
                break;
            case CardOpType.Down: //DOWN
                Debug.Log($"{characterId} was lowered");
                TasksDispatcher.Instance.Schedule(delegate {
                    UnityEngine.Object.FindObjectOfType<DeckOpponent>().Flip(characterId, false);
                });
                //TODO?
                break;
            default:
                break;
        }
    }

    public void StatusOpParse(StateObject currentState, StateObject futureState) {
        byte status = futureState.buffer[1];
        switch ((Status) status) {
            case Status.Start: //START
                Debug.Log($"Match was started");
                TasksDispatcher.Instance.Schedule(delegate {
                    UnityEngine.Object.FindObjectOfType<GameManager>().StartMatch();
                });
                //TODO?
                break;
            default:
                TasksDispatcher.Instance.Schedule(delegate {
                    UnityEngine.Object.FindObjectOfType<GameManager>().EndMatch((Status) status);
                });
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
        switch ((Answer) agreement) {
            case Answer.Confirm: //CONFIRMED
                agreementText = "confirmed";
                break;
            case Answer.Deny: //DENIED
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
