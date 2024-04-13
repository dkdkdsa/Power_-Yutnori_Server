﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Connector
    {

        private Func<Session> sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int Dummycount = 1) 
        {

            for(int i = 0; i< Dummycount; i++)
            {

                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.sessionFactory = sessionFactory;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);

            }

        }

        private void RegisterConnect(SocketAsyncEventArgs args) 
        {

            var socket = args.UserToken as Socket;

            if (socket == null)
            {

                return;

            }

            bool pending = socket.ConnectAsync(args);

            if (pending == false)
            {

                OnConnectCompleted(null, args);

            }

        }

        private void OnConnectCompleted(object state, SocketAsyncEventArgs args) 
        { 

            if(args.SocketError == SocketError.Success)
            {

                Session session = sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);

            }
            else
            {

                Console.WriteLine("OnConnectCompleted Fail: {args.SocketError}");

            }

        }

    }

}