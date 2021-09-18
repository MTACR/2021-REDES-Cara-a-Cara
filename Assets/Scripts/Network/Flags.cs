namespace Network
{
    public enum Message : byte
    {
        Connection = 0,
        Card = 1,
        Status = 2,
        TimeUp = 3,
        Question = 4,
        Answer = 5
    }

    public enum Connection : byte
    {
        /*Request = 0,
        Positive = 1,
        Negative = 2,*/
        Connect = 4,
        Disconnect = 3
    }

    public enum Card : byte
    {
        /*Choose = 0,*/
        Guess = 1,
        Up = 2,
        Down = 3
    }

    public enum Status : byte
    {
        Start = 0,
        Win = 1,
        Lose = 2,
        Tie = 3,
        End = 4
    }

    public enum Answer : byte
    {
        Confirm = 0,
        Deny = 1,
        Unclear = 2
    }
}