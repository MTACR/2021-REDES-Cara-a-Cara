using System;
using System.Linq;
using System.Text;
using Callbacks;
using Cards;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Network
{
    public class Parser
    {
        public void ParseMessage(StateObject currentState, StateObject futureState)
        {
            var messageType = futureState.buffer[0];
            switch ((MessageType) messageType)
            {
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ConnectionOpParse(StateObject currentState, StateObject futureState)
        {
            var opType = futureState.buffer[1];
            var sender = futureState.buffer.Skip(2).Take(20).ToArray();
            var sender_name = Encoding.Default.GetString(sender);
            var sentMessage = futureState.buffer.Skip(22).Take(100).ToArray();
            var sentText = Encoding.Default.GetString(sentMessage);
            switch ((ConnectionType) opType)
            {
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CardOpParse(StateObject currentState, StateObject futureState)
        {
            var characterId = futureState.buffer[1];
            var opCode = futureState.buffer[2];
            switch ((CardOpType) opCode)
            {
                case CardOpType.Choose: //CHOOSE
                    Debug.Log($"{characterId} was chosen");
                    //TODO
                    break;
                case CardOpType.Guess: //GUESS
                    Debug.Log($"{characterId} was guessed");
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        if (Object.FindObjectOfType<Deck>().IsChosen(characterId))
                        {
                            var messase = SenderParser.ParseStatus(Status.Win);
                            Object.FindObjectOfType<Client>().Send(messase);
                            Object.FindObjectOfType<GameManager>().EndMatch(Status.Lose);
                        }
                    });
                    //TODO?
                    break;
                case CardOpType.Up: //UP
                    Debug.Log($"{characterId} was raised");
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        Object.FindObjectOfType<DeckOpponent>().Flip(characterId, true);
                    });
                    //TODO?
                    break;
                case CardOpType.Down: //DOWN
                    Debug.Log($"{characterId} was lowered");
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        Object.FindObjectOfType<DeckOpponent>().Flip(characterId, false);
                    });
                    //TODO?
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void StatusOpParse(StateObject currentState, StateObject futureState)
        {
            var status = futureState.buffer[1];
            switch ((Status) status)
            {
                case Status.Start: //START
                    Debug.Log("Match was started");
                    TasksDispatcher.Instance.Schedule(delegate { Object.FindObjectOfType<GameManager>().StartMatch(); });
                    //TODO?
                    break;
                case Status.Win:
                    break;
                case Status.Lose:
                    break;
                case Status.Tie:
                    break;
                case Status.End:
                    break;
                default:
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        Object.FindObjectOfType<GameManager>().EndMatch((Status) status);
                    });
                    break;
            }
        }

        private static void TimeUpParse(StateObject currentState, StateObject futureState)
        {
            var time = futureState.buffer[1];

            Debug.Log($"{time}s passed");
            //TODO
        }

        private static void QuestionParse(StateObject currentState, StateObject futureState)
        {
            var sender = futureState.buffer.Skip(1).Take(20).ToArray();
            var sender_name = Encoding.Default.GetString(sender);
            var questionMessage = futureState.buffer.Skip(21).Take(100).ToArray();
            var questionText = Encoding.Default.GetString(questionMessage);

            Debug.Log($"{sender_name} asked {questionText}");
            //TODO
        }

        private static void QuestionAnswerParse(StateObject currentState, StateObject futureState)
        {
            var sender = futureState.buffer.Skip(1).Take(20).ToArray();
            var sender_name = Encoding.Default.GetString(sender);
            var agreement = futureState.buffer[22];
            var answerMessage = futureState.buffer.Skip(22).Take(100).ToArray();
            var answernText = Encoding.Default.GetString(answerMessage);

            string agreementText = (Answer) agreement switch
            {
                Answer.Confirm => //CONFIRMED
                    "confirmed",
                Answer.Deny => //DENIED
                    "denied",
                _ => "left uncler"
            };

            Debug.Log($"{sender_name} {agreementText}: {answernText}");
            //TODO
        }
    }
}