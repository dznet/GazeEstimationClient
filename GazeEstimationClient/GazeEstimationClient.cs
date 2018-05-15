using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GazeEstimationClient
{
    class GazeEstimationClient
    {

        static string _port = "5055";
        static string _address = "127.0.0.1";

        private Socket _socket;
        private IPEndPoint _ipEndPoint;

        public Queue<string> ResponseString;

        private void SendData(byte[] data)
        {
            try
            {

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(_ipEndPoint);
                string request = $"POST {_address}:{_port}/HTTP/1.1/\r\n{data}";

                {
                    ResponseString.Enqueue($"Request: {request}");
                }

                byte[] sendBytes = Encoding.ASCII.GetBytes(request);
                _socket.Send(sendBytes);
            }
            finally
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
            }
        }

        private void RecievedData()
        {
            try
            {

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(_ipEndPoint);

                byte[] receiveBytes = new byte[256];
                int countBytes = 0;
                StringBuilder builder = new StringBuilder();
                do
                {
                    countBytes = _socket.Receive(receiveBytes, receiveBytes.Length, 0);
                    builder.Append(Encoding.ASCII.GetString(receiveBytes, 0, countBytes));
                }
                while (_socket.Available > 0);
                {
                    ResponseString.Enqueue($"Response: {builder.ToString()}");
                }
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            finally
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
            }
        }
    }
}
