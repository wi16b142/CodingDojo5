using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CD5_Server.Model
{
    class Server
    {
        Socket serverSocket;
        List<ClientHandler> clients = new List<ClientHandler>();
        Thread acceptingThread;
        Action<string> GUIUpdate;

        public Server(string ip, int port, Action<string> guiBroadcast)
        {
            GUIUpdate = guiBroadcast;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(5);

        }

        public void StartAccepting()
        {
            acceptingThread = new Thread(Accept);
            acceptingThread.IsBackground = true;
            acceptingThread.Start();

        }

        private void Accept()
        {
            while (acceptingThread.IsAlive)
            {
                try
                {
                    clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(BroadCastMessage)));
                } catch (Exception e)
                {
                    //exit
                }
            }
        }

        public void StopAccepting()
        {
            serverSocket.Close();
            acceptingThread.Abort();
            foreach(var client in clients)
            {
                client.Close();
            }
            clients.Clear();
        }

        public void BroadCastMessage(string msg, Socket senderSocket)
        {
            GUIUpdate(msg);

            foreach(var client in clients)
            {
                if(client.ClientSocket != senderSocket)
                {
                    client.Send(msg);
                }
            }
        }

        public void KickClient(string name)
        {
            foreach(var client in clients)
            {
                if (client.Name.Equals(name))
                {
                    client.Close();
                    clients.Remove(client);
                    break; //exit after deletion
                }
            }
        }

    }
}
