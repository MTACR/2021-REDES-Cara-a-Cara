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
            Client.Instance.Pong();
            
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

                case Network.Connection.Ping:
                    Client.Instance.Send(SenderParser.Connection(Network.Connection.Pong));
                    break;

                case Network.Connection.Pong:
                    Debug.Log("Pong");
                    Client.Instance.Pong();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            Status status = (Status) state.buffer[5];

            if (status == Network.Status.Start)
            {
                Debug.Log("Opponent is ready to play");
                Client.Instance.opReady = true;
            }

            TasksDispatcher.Instance.Schedule(delegate
            {
                try
                {
                    Object.FindObjectOfType<GameManager>().SetMatchStatus(status);
                }
                catch (NullReferenceException e)
                {
                    Debug.Log("I'm not ready yet");
                }
            });
        }

        private static void Question(State state)
        {
            var question = Encoding.Default.GetString(state.buffer.Skip(9).Take(100).ToArray());

            Debug.Log($"Opponent asked {question}");

            TasksDispatcher.Instance.Schedule(delegate
            {
                Object.FindObjectOfType<GameManager>().RequireAnswer();
                Object.FindObjectOfType<ChatManager>().ShowOpMessage("Opponent", question);
            });
        }

        private static void Answer(State state)
        {
            var answer = (Answer) state.buffer[9];

            TasksDispatcher.Instance.Schedule(delegate
            {
                Object.FindObjectOfType<ChatManager>().ReactToMessage(answer);
                Object.FindObjectOfType<GameManager>().SetMyTurn(answer == Network.Answer.Unclear);

                if (answer == Network.Answer.Unclear)
                    Object.FindObjectOfType<GameManager>().Unclear();
            });
        }
    }
}