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
        
        public static void Message(State state)
        {
            switch ((Message) state.buffer[0])
            {
                case Network.Message.Connection:
                    Connection(state);
                    break;

                case Network.Message.Card:
                    Card(state);
                    break;

                case Network.Message.Status:
                    Status(state);
                    break;

                case Network.Message.Question:
                    Question(state);
                    break;

                case Network.Message.Answer:
                    Answer(state);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Connection(State state)
        {
            switch ((Connection) state.buffer[5])
            {
                case Network.Connection.Connect:
                    Debug.Log("Opponent connected");
                    break;

                case Network.Connection.Disconnect:
                    Debug.Log("Opponent disconnected");
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        Object.FindObjectOfType<GameManager>().OpponentGaveUp();
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }


            //client.opId = senderId;
        }

        private static void Card(State state)
        {
            var cardId = state.buffer[5];
            switch ((Card) state.buffer[6])
            {
                case Network.Card.Guess:
                    Debug.Log($"{cardId} was guessed");
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        Object.FindObjectOfType<GameManager>().OpponentGuess(cardId);
                    });
                    break;
                case Network.Card.Up:
                    Debug.Log($"{cardId} was raised");
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        Object.FindObjectOfType<DeckOpponent>().Flip(cardId, true);
                    });
                    break;
                case Network.Card.Down:
                    Debug.Log($"{cardId} was lowered");
                    TasksDispatcher.Instance.Schedule(delegate
                    {
                        Object.FindObjectOfType<DeckOpponent>().Flip(cardId, false);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Status(State state)
        {
            var status = state.buffer[5];

            TasksDispatcher.Instance.Schedule(delegate
            {
                Object.FindObjectOfType<GameManager>().SetMatchStatus((Status) status);
            });
        }

        /*private static void TimeUp(State state) //TODO: remover
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var time = state.buffer[5];

            Debug.Log($"{time}s passed");
            //TODO
        }*/

        private static void Question(State state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var questionId = BitConverter.ToInt32(state.buffer, 5);
            var questionText = Encoding.Default.GetString(state.buffer.Skip(9).Take(100).ToArray());

            Debug.Log($"Opponent asked {questionText}");

            TasksDispatcher.Instance.Schedule(delegate
            {
                Object.FindObjectOfType<GameManager>().RequireAnswer();
                Object.FindObjectOfType<ChatManager>().ShowMessage(questionId, "Opponent", questionText);
            });
        }

        private static void Answer(State state)
        {
            var senderId = BitConverter.ToInt32(state.buffer, 1);
            var questionId = BitConverter.ToInt32(state.buffer, 5);
            var answer = (Answer) state.buffer[9];

            TasksDispatcher.Instance.Schedule(delegate
            {
                var agreementText = answer switch
                {
                    Network.Answer.Confirm => //CONFIRMED
                        "confirmed",
                    Network.Answer.Deny => //DENIED
                        "denied",
                    _ => "left uncler"
                };

                Debug.Log($"Opponent {agreementText}");
                
                Object.FindObjectOfType<ChatManager>().ReactToMessage(questionId, answer);
                Object.FindObjectOfType<GameManager>().SetTurn(answer == Network.Answer.Unclear);

                if (answer == Network.Answer.Unclear)
                    Object.FindObjectOfType<GameManager>().Unclear();
            });
        }
    }
}