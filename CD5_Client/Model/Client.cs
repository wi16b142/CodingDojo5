using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CD5_Client.Model
{
    class Client
    {
        private TcpClient client;
        private Socket clientSocket;
        private byte[] buffer = new byte[512];
        Action<string> Incoming;
        Action DisconnectAcknowledge;
        private const string endMsg ="@quit";


        public Client(string ip, int port, Action<string> IncomingNewMsg, Action DisconnectRequest)
        {
            try
            {
                DisconnectAcknowledge = DisconnectRequest;
                Incoming = IncomingNewMsg;
                this.client = new TcpClient();
                this.client.Connect(IPAddress.Parse(ip), port);
                clientSocket = client.Client;
                StartReceiving();
            }
            catch (Exception e)
            {
                IncomingNewMsg("Server not ready!");
                DisconnectAcknowledge();
            }

        }

        public void StartReceiving()
        {
            Task.Factory.StartNew(Receive);
        }

        private void Receive()
        {
            string message = "";
            while (!message.Contains(endMsg))
            {
                int length = clientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);
                Incoming(message);
            }
            Close();
        }

        public void Send(string msg)
        {
            if(clientSocket != null) clientSocket.Send(Encoding.UTF8.GetBytes(msg));
        }

        public void Close()
        {
            clientSocket.Close();
            DisconnectAcknowledge();
        }
    }
}
