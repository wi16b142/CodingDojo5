using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CD5_Server.Model
{
    class ClientHandler
    {
        private Action<string, Socket> action;
        private byte[] buffer = new byte[512];
        private Thread receiveMessageThread;
        private const string endMsg = "@quit";

        public string Name { get; private set; }

        public Socket ClientSocket { get; private set; }

        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            this.ClientSocket = socket;
            this.action = action;
            this.receiveMessageThread = new Thread(Receive);
            this.receiveMessageThread.Start();
        }

        private void Receive()
        {
            string msg = "";
            while(!msg.Equals(endMsg))
            {
                int length = ClientSocket.Receive(buffer);
                msg = Encoding.UTF8.GetString(buffer,0,length);

                if(Name == null && msg.Contains(":"))
                {
                    Name = msg.Split(':')[0];
                } else
                {
                    Name = "Anonymus";
                }

                action(msg, ClientSocket);
            }
            Close();
        }

        public void Close()
        {
            Send(endMsg);
            this.ClientSocket.Close(1);
            receiveMessageThread.Abort();
        }

        public void Send(string msg)
        {
            ClientSocket.Send(Encoding.UTF8.GetBytes(msg));
        }
    }
}
