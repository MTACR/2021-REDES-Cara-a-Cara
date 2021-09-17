using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenderParser {
    //<op_type: Integer, sender_name: String, message: String
    public static byte[] ParseConnection(int senderId, ConnectionType opType, string senderName, string message) {
        int length = 1 + 4 + 1 + 20 + 100;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.ConnectionOp;
        messageByte = OffsetIntToByte(messageByte, senderId, 1);
        messageByte[5] = (byte) opType;
        messageByte = OffsetStringtoByte(messageByte, senderName, 6);
        messageByte = OffsetStringtoByte(messageByte, message, 26);

        return messageByte;
    }

    //<character_id: Integer, OpCode: Integer>
    public static byte[] ParseCardOp(int senderId, int characterId, CardOpType cardOpType) {
        int length = 1 + 4 + 1 + 1;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.CardOp;
        messageByte = OffsetIntToByte(messageByte, senderId, 1);
        messageByte[5] = (byte) characterId;
        messageByte[6] = (byte) cardOpType;

        return messageByte;
    }

    public static byte[] ParseStatus(int senderId, Status status) {
        int length = 1 + 4 + 1;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.Status;
        messageByte[5] = (byte) status;
        return messageByte;
    }

    public static byte[] ParseTimeUp(int senderId, byte secondsPassed) {
        int length = 1 + 4 + 1;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.TimeUp;
        messageByte = OffsetIntToByte(messageByte, senderId, 1);
        messageByte[5] =        secondsPassed;
        return messageByte;
    }

    public static byte[] ParseQuestion(int senderId, int questionId, string message) {
        int length = 1 + 4 + 4 + 100;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.Question;
        messageByte = OffsetIntToByte(messageByte, senderId, 1);
        messageByte = OffsetIntToByte(messageByte, questionId, 5);
        messageByte = OffsetStringtoByte(messageByte, message, 9);
        return messageByte;
    }

    public static byte[] ParseAnswer(int senderId, int questionId, Answer answer, string response) {
        int length = 1 + 4 + 4 + 1 + 100;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.Answer;
        messageByte = OffsetIntToByte(messageByte, senderId, 1);
        messageByte = OffsetIntToByte(messageByte, questionId, 5);
        messageByte[9] = (byte) answer;
        messageByte = OffsetStringtoByte(messageByte, response, 10);
        return messageByte;
    }

    private static byte[] OffsetStringtoByte(byte[] messageByte, string str, int offset) {
        for (int i = 0; i < str.Length; ++i) {
            messageByte[offset + i] = (byte)str[i];
        }

        return messageByte;
    }

    private static byte[] OffsetIntToByte(byte[] messageByte, int id, int offset) {
        var byteId = System.BitConverter.GetBytes(id);
        for (int i = 0; i < 4; ++i) {
            messageByte[offset + i] = byteId[i];
        }

        return messageByte;
    }
}
