using System;

namespace Network
{
    public static class SenderParser
    {
        public static byte[] Connection(Connection op /*, string senderName, string message*/)
        {
            const int length = 1 + 4 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Connection;
            bytes = OffsetIntToByte(bytes, Client.Instance.myId, 1);
            bytes[5] = (byte) op;
            /*messageByte = OffsetStringtoByte(messageByte, senderName, 6);
            messageByte = OffsetStringtoByte(messageByte, message, 26);*/
            return bytes;
        }

        public static byte[] Card(int id, Card card)
        {
            const int length = 1 + 4 + 1 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Card;
            bytes = OffsetIntToByte(bytes, Client.Instance.myId, 1);
            bytes[5] = (byte) id;
            bytes[6] = (byte) card;
            return bytes;
        }

        public static byte[] Status(Status status)
        {
            //TODO: usar para informar vitoria ou remover
            const int length = 1 + 4 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Status;
            bytes = OffsetIntToByte(bytes, Client.Instance.myId, 1);
            bytes[5] = (byte) status;
            return bytes;
        }

        /*public static byte[] ParseTimeUp(byte secondsPassed) { //TODO: provavelmente remover
        int length = 1 + 4 + 1;
        byte[] messageByte = new byte[length];
        messageByte[0] = (byte) MessageType.TimeUp;
        messageByte = OffsetIntToByte(messageByte, Client.Instance.myId, 1);
        messageByte[5] =        secondsPassed;
        return messageByte;
    }*/

        public static byte[] Question(int id, string message)
        {
            const int length = 1 + 4 + 4 + 100;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Question;
            bytes = OffsetIntToByte(bytes, Client.Instance.myId, 1);
            bytes = OffsetIntToByte(bytes, id, 5);
            bytes = OffsetStringtoByte(bytes, message, 9);
            return bytes;
        }

        public static byte[] Answer(int id, Answer answer /*, string response*/)
        {
            const int length = 1 + 4 + 4 + 1;
            var bytes = new byte[length];
            bytes[0] = (byte) Message.Answer;
            bytes = OffsetIntToByte(bytes, Client.Instance.myId, 1);
            bytes = OffsetIntToByte(bytes, id, 5);
            bytes[9] = (byte) answer;
            /*messageByte = OffsetStringtoByte(messageByte, response, 10);*/
            return bytes;
        }

        private static byte[] OffsetStringtoByte(byte[] bytes, string str, int offset)
        {
            for (var i = 0; i < str.Length; ++i)
                bytes[offset + i] = (byte) str[i];

            return bytes;
        }

        private static byte[] OffsetIntToByte(byte[] bytes, int id, int offset)
        {
            var byteId = BitConverter.GetBytes(id);
            for (var i = 0; i < 4; ++i)
                bytes[offset + i] = byteId[i];

            return bytes;
        }
    }
}