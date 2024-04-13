using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Core
{
    public class Listener
    {
        private Socket listenSocket;
        private Func<Session> sessionFactory; 

        public void init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {

            listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sessionFactory += sessionFactory;

            listenSocket.Bind(endPoint);

            listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); 

            RegisterAccept(args);       

        }

        private void RegisterAccept(SocketAsyncEventArgs args)
        {

            args.AcceptSocket = null;

            bool pending = listenSocket.AcceptAsync(args);
            if (pending == false)
            {

                OnAcceptCompleted(null, args);

            }

        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {

            if (args.SocketError == SocketError.Success)
            {

                Session session = sessionFactory.Invoke();
                session.ToString();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);

            }
            else
            {

                Console.WriteLine(args.SocketError.ToString());

            }

            RegisterAccept(args);

        }
    }
}
