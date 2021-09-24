namespace Network
{
    public enum Message : byte
    {
        Connection = 0,
        Card = 1,
        Status = 2,
        Question = 3,
        Answer = 4
    }

    public enum Connection : byte
    {
        Connect = 0,
        Disconnect = 1,
        Ping = 2,
        Pong = 3
    }

    public enum Card : byte
    {
        Guess = 0,
        Up = 1,
        Down = 2
    }

    public enum Status : byte
    {
        Start = 0,
        Win = 1,
        Lose = 2,
        Rematch = 3,
    }

    public enum Answer : byte
    {
        Confirm = 0,
        Deny = 1,
        Unclear = 2
    }
}