using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

        public int StonePosition { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected bytes: {endPoint}");

            // 채팅 게임룸 생성
            Program.Room.Enter(this);
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected bytes: {endPoint}");
            SessionManager.instance.Remove(this);
            if (Room != null)
            {
                Room.Leave(this);
                Room = null;
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ServerPacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)             // 샌드 이벤트 발생시 동작
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }

    }

}
