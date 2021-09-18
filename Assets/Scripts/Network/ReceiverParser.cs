using System;
using System.Linq;
using System.Text;
using Callbacks;
using Cards;
using Chat;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Network
{
    public static class ReceiverParser
    {
        public static void ParseMessage(StateObject state)
        {
            var messageType = state.buffer[0];
            
            switch ((MessageType) messageType)
            {
                case MessageType.ConnectionOp: //CONNECTION_OP
                    ConnectionOpParse(state);
                    break;
                case MessageType.CardOp: //CARD_OP
                    CardOpParse(state);
                    break;
                case MessageType.Status: //MATCH_STATUS
                    StatusOpParse(state);
                    break;
                case MessageType.TimeUp: //TIME_UP
                    TimeUpParse(state);
                    break;
                case MessageType.Question: //QUESTION
                    QuestionParse(state);
                    break;
                case MessageType.Answer: //QUESTION_ANSR
                    QuestionAnswerParse(state);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ConnectionOpParse(StateObject state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var opType = state.buffer[5];
            var sender = state.buffer.Skip(6).Take(20).ToArray();
            var sender_name = Encoding.Default.GetString(sender);
            var sentMessage = state.buffer.Skip(26).Take(100).ToArray();
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


            Client.Instance.opId = senderId;
        }

        private static void CardOpParse(StateObject state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var characterId = state.buffer[5];
            var opCode = state.buffer[6];
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
                            Client.Instance.Send(messase);
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

        private static void StatusOpParse(StateObject state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var status = state.buffer[5];
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

        private static void TimeUpParse(StateObject state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var time = state.buffer[5];

            Debug.Log($"{time}s passed");
            //TODO
        }

        private static void QuestionParse(StateObject state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var questionId = BitConverter.ToInt32(state.buffer, 5);
            var questionMessage = state.buffer.Skip(9).Take(100).ToArray();
            var questionText = Encoding.Default.GetString(questionMessage);

            Debug.Log($"{"PH"} asked {questionText}");
            
            TasksDispatcher.Instance.Schedule(delegate
            {
                Object.FindObjectOfType<GameManager>().RequireAnswer();
                Object.FindObjectOfType<ChatManager>().ShowMessage(questionId, "???????????", questionText);
            });
        }

        private static void QuestionAnswerParse(StateObject state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var questionId = BitConverter.ToInt32(state.buffer, 5);
            var agreement = state.buffer[9];
            var answerMessage = state.buffer.Skip(10).Take(100).ToArray();
            var answernText = Encoding.Default.GetString(answerMessage);
            var answer = (Answer) agreement;
                
            string agreementText = (Answer) agreement switch
            {
                Answer.Confirm => //CONFIRMED
                    "confirmed",
                Answer.Deny => //DENIED
                    "denied",
                _ => "left uncler"
            };
            
            TasksDispatcher.Instance.Schedule(delegate
            {
                Object.FindObjectOfType<ChatManager>().ReactToMessage(questionId, answer);
            });

            Debug.Log($"{"PH"} {agreementText}: {answernText}");
            //TODO
        }
    }
}