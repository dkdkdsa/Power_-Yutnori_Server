using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Core
{
    public abstract class PacketSession : Session 
    {
        public static readonly int HeaderSize = 2;

        public sealed override int OnRecv(ArraySegment<byte> buffer)    
        {
            int processLen = 0;

            while(true)
            {

                if (buffer.Count < HeaderSize)   
                    break;

                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));  

                processLen += dataSize;

                buffer = new ArraySegment<byte>(
                    buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {

        private Socket _socket;
        private int _disconnected = 0;
        private RecvBuffer _recvBuffer = new RecvBuffer(1024);
        private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        private List<ArraySegment<byte>> _pendinglist = new List<ArraySegment<byte>>();
        private object handle = new object();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnSend(int numOfBytes);
        public abstract int OnRecv(ArraySegment<byte> buffer);


        public void Start(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);


            RegisterRecv();
        }


        private void RegisterRecv()
        {
            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {   

            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {

                try
                {

                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }


                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }
        
                    if (_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed! {e}");
                }

            }
            else
            {
                // Disconnect
            }
        }

        public void Send(ArraySegment<byte> sendBuff) 
        {
            lock (handle)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendinglist.Count == 0)
                    RegisterSend();
            }

        }

        private void RegisterSend()
        {

            while(_sendQueue.Count > 0)
            {

                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendinglist.Add(buff);

            }
            _sendArgs.BufferList = _pendinglist;

            bool pending = _socket.SendAsync(_sendArgs);
            if(pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }

        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(handle)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendinglist.Clear();           

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0) 
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"OnReceiveCompleted Failed! {ex}");
                    }

                }
                else
                {
                    Disconnect();
                }
            }

        }



        public void Disconnect()
        {
            if(Interlocked.Exchange(ref _disconnected, 1) == 1)
            {
                return;
            }
            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

    }
}
