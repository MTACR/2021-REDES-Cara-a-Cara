using System;

namespace Network
{
    public static class SenderParser
    {
        public static byte[] Connection(Connection op)
        {
            const int length = 1 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Connection;
            bytes[1] = (byte) op;
            return bytes;
        }

        public static byte[] Card(int id, Card card)
        {
            const int length = 1 + 1 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Card;
            bytes[1] = (byte) id;
            bytes[2] = (byte) card;
            return bytes;
        }

        public static byte[] Status(Status status)
        {
            const int length = 1 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Status;
            bytes[1] = (byte) status;
            return bytes;
        }

        public static byte[] Question(string message)
        {
            const int length = 1 + 100;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Question;
            bytes = OffsetStringtoByte(bytes, message, 1);
            return bytes;
        }

        public static byte[] Answer(Answer answer)
        {
            const int length = 1 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Answer;
            bytes[1] = (byte) answer;
            return bytes;
        }

        private static byte[] OffsetStringtoByte(byte[] bytes, string str, int offset)
        {
            for (var i = 0; i < str.Length; ++i)
                bytes[offset + i] = (byte) str[i];

            return bytes;
        }
    }
}