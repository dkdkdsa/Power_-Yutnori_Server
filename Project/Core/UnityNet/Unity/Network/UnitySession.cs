using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityNet
{
    public class UnitySession : PacketSession
    {

        private void HandlePacketRecv(PacketSession session, IPacket packet)
        {

            PacketQueue.Instance.Push(packet);

        }

        public override void OnConnected(EndPoint endPoint)
        {

            //이벤트 발행

        }

        public override void OnDisconnected(EndPoint endPoint)
        {

            //이벤트 발행

        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {

            UnityPacketManager.Instance.OnRecvPacket(this, buffer, HandlePacketRecv);

        }

        public override void OnSend(int numOfBytes)
        {

            Debug.Log(numOfBytes);

        }

    }

}
