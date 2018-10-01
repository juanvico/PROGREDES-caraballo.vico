using System;
using System.Net.Sockets;

namespace Client
{
    public class Transmitter
    {
        public static void Receive(Socket socket)
        {
            try
            {
                while (true)
                {
                    var pos = 0;
                    var lengthInBytes = new byte[4];
                    var i = 0;
                    while (i < 4)
                    {
                        i += socket.Receive(lengthInBytes, i, 4 - i, SocketFlags.None);
                    }
                    int length = BitConverter.ToInt32(lengthInBytes, 0);
                    var msgBytes = new byte[length];
                    while (pos < length)
                    {
                        var recieved = socket.Receive(msgBytes, pos, length - pos, SocketFlags.None);
                        if (recieved == 0) throw new SocketException();
                        pos += recieved;
                    }
                    Console.WriteLine(System.Text.Encoding.ASCII.GetString(msgBytes));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Send(Socket client, string message = "")
        {
            try
            {
                var msg = message;
                SendBitsLength(client, msg);

                var byteMsg = System.Text.Encoding.ASCII.GetBytes(msg);
                var length = byteMsg.Length;
                var pos = 0;

                while (pos < length)
                {
                    var sent = client.Send(byteMsg, pos, length - pos, SocketFlags.None);
                    if (sent == 0) throw new SocketException();
                    pos += sent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendBitsLength(Socket client, string msg)
        {
            var messaageLengthInInt = msg.Length;
            var messageLengthInBit = BitConverter.GetBytes(messaageLengthInInt);
            var i = 0;
            while (i < 4)
            {
                i += client.Send(messageLengthInBit, i, 4 - i, SocketFlags.None);
            }
        }

        public static void SendImage(Socket client, byte[] msg)
        {
            SendImageBitsLength(client, msg);

            var pos = 0;
            while (pos < msg.Length)
            {
                var sent = client.Send(msg, pos, msg.Length - pos, SocketFlags.None);
                if (sent == 0) throw new SocketException();
                pos += sent;
            }
        }

        private static void SendImageBitsLength(Socket client, byte[] msg)
        {
            var length = msg.Length;
            var dataLength = BitConverter.GetBytes(length);
            var i = 0;
            while (i < 4)
            {
                i += client.Send(dataLength, i, 4 - i, SocketFlags.None);
            }
        }
    }
}
