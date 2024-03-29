﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Transmitter
    {
        public static string Receive(Socket client)
        {
            while (true)
            {
                var pos = 0;
                var lengthInBytes = new byte[4];
                var i = 0;
                while (i < 4)
                {
                    i += client.Receive(lengthInBytes, i, 4 - i, SocketFlags.None);
                }
                int length = BitConverter.ToInt32(lengthInBytes, 0);
                var msgBytes = new byte[length];
                while (pos < length)
                {
                    var recieved = client.Receive(msgBytes, pos, length - pos, SocketFlags.None);
                    if (recieved == 0) throw new SocketException();
                    pos += recieved;
                }
                
                string cmd = System.Text.Encoding.ASCII.GetString(msgBytes);
                return cmd;
            }
        }

        public static void Send(Socket client, string message = "")
        {
            var msg = message;
            SendBitsLength(client, msg);

            var byteMsg = System.Text.Encoding.ASCII.GetBytes(msg); //lo convierte a un array de bytes
            var length = byteMsg.Length;
            var pos = 0;

            while (pos < length)
            {
                var sent = client.Send(byteMsg, pos, length - pos, SocketFlags.None);
                if (sent == 0) throw new SocketException();
                pos += sent;
            }
        }

        static void SendBitsLength(Socket client, string msg)
        {
            var messaageLengthInInt = msg.Length;
            var messageLengthInBit = BitConverter.GetBytes(messaageLengthInInt);
            var i = 0;
            while (i < 4)
            {
                i += client.Send(messageLengthInBit, i, 4 - i, SocketFlags.None);
            }
        }
    }
}
