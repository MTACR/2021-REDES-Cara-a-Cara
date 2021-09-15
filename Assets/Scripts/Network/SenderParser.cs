using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenderParser {
    //<op_type: Integer, sender_name: String, message: String
    public byte[] ParseConnection(ConnectionType opType, string senderName, string message) {
        int length = 1 + 1 + 20 + 100;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.ConnectionOp;
        messageByte[1] = (byte) opType;
        messageByte = OffsetStringtoByte(messageByte, senderName, 2);
        messageByte = OffsetStringtoByte(messageByte, message, 22);

        return messageByte;
    }

    //<character_id: Integer, OpCode: Integer>
    public byte[] ParseCardOp(byte characterId, CardOpType cardOpType) {
        int length = 1 + 1 + 1;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.CardOp;
        messageByte[1] =        characterId;
        messageByte[2] = (byte) cardOpType;

        return messageByte;
    }

    public byte[] ParseStatus(Status status) {
        int length = 1 + 1;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.Status;
        messageByte[1] = (byte) status;
        return messageByte;
    }

    public byte[] ParseStatus(byte secondsPassed) {
        int length = 1 + 1;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.TimeUp;
        messageByte[1] =        secondsPassed;
        return messageByte;
    }

    public byte[] ParseQuestion(string senderName, string message) {
        int length = 1 + senderName.Length + message.Length;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.Question;
        messageByte = OffsetStringtoByte(messageByte, senderName, 1);
        messageByte = OffsetStringtoByte(messageByte, message, 21);
        return messageByte;
    }

    public byte[] ParseAnswer(string senderName, Answer answer, string response) {
        int length = 1 + senderName.Length + 1 + response.Length;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.Answer;
        messageByte = OffsetStringtoByte(messageByte, senderName, 1);
        messageByte[21] = (byte) answer;
        messageByte = OffsetStringtoByte(messageByte, response, 22);
        return messageByte;
    }

    private byte[] OffsetStringtoByte(byte[] messageByte, string str, int offset) {
        for (int i = 0; i < str.Length; ++i) {
            messageByte[offset + i] = (byte)str[i];
        }

        return messageByte;
    }
}
