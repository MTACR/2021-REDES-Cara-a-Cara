namespace Network
{
    public enum Message : byte
    {
        Connection = 0,
        Card = 1,
        Status = 2,
        Question = 4,
        Answer = 5
    }

    public enum Connection : byte
    {
        Connect = 4,
        Disconnect = 3
    }

    public enum Card : byte
    {
        Guess = 1,
        Up = 2,
        Down = 3
    }

    public enum Status : byte
    {
        Win = 1,
        Lose = 2,
        Rematch = 5,
    }

    public enum Answer : byte
    {
        Confirm = 0,
        Deny = 1,
        Unclear = 2
    }
}