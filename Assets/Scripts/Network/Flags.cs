using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum MessageType : byte {
    ConnectionOp = 0,
    CardOp = 1,
    Status = 2,
    TimeUp = 3,
    Question = 4, 
    Answer = 5
}

enum ConnectionType : byte {
    Request = 0,
    Positive = 1,
    Negative = 2,
    Disconnect = 3
}

enum CardOpType : byte {
    Choose = 0,
    Guess = 1,
    Up = 2,
    Down = 3
}

enum Status : byte {
    Start = 0,
    Win = 1,
    Lose = 2,
    Tie = 3,
    End = 4
}

enum Answer : byte {
    Confirm = 0,
    Deny = 1,
    Unclear = 2
}


