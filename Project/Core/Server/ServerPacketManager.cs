using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ServerPacketManager : PacketManager
    {

        public static ServerPacketManager Instance { get; } = new();

        public override void Register()
        {

            makeFunc.Add((ushort)PacketType.NetPrefabSpawneing, MakePacket<NetPrefabSpawneingPacket>);
            makeFunc.Add((ushort)PacketType.MethodLinkPacket, MakePacket<MethodLinkPacket>);
            makeFunc.Add((ushort)PacketType.MethodLinkParamPacket, MakePacket<MethodLinkPacketParam>);
            makeFunc.Add((ushort)PacketType.TransformLinkPacket, MakePacket<TransformLinkPacket>);
            makeFunc.Add((ushort)PacketType.DespawnObjectPacket, MakePacket<DespawnObjectPacket>);
            makeFunc.Add((ushort)PacketType.TurnChangePacket, MakePacket<TurnChangePacket>);

            handler.Add((ushort)PacketType.NetPrefabSpawneing, PrefabSpawnHandle);
            handler.Add((ushort)PacketType.MethodLinkPacket, MethodLinkHandle);
            handler.Add((ushort)PacketType.MethodLinkParamPacket, MethodLinkParamHandle);
            handler.Add((ushort)PacketType.TransformLinkPacket, BroadCastNotSendClient);
            handler.Add((ushort)PacketType.DespawnObjectPacket, DespawnObjectHandle);
            handler.Add((ushort)PacketType.TurnChangePacket, TurnChangePacket);

        }

        private void DespawnObjectHandle(PacketSession session, IPacket packet)
        {

            var p = packet as DespawnObjectPacket;
            Program.Room.RemoveObjectData(p.objectHash);

            BroadCastNotSendClient(session, p);

        }

        public static void PrefabSpawnHandle(PacketSession session, IPacket packet)
        {

            var clientSession = session as ClientSession;
            var prefabPacket = packet as NetPrefabSpawneingPacket;

            Program.Room.BroadCast(prefabPacket.Write(), clientSession.SessionId);

            var data = new NetObjectData
            {

                hash = prefabPacket.hash,
                position = prefabPacket.position,
                rotaitoin = prefabPacket.rotation,
                prefabName = prefabPacket.prefabName,
                ownerId = prefabPacket.ownerId

            };

            Program.Room.AddObjectData(data);

        }

        public static void MethodLinkHandle(PacketSession session, IPacket packet)
        {

            var p = packet as MethodLinkPacket;
            int clientId = p.immediatelyCalled ? (session as ClientSession).SessionId : -1;

            Program.Room.BroadCast(packet.Write(), clientId);

        }

        public static void MethodLinkParamHandle(PacketSession session, IPacket packet)
        {

            var p = packet as MethodLinkPacketParam;
            int clientId = p.immediatelyCalled ? (session as ClientSession).SessionId : -1;

            Program.Room.BroadCast(packet.Write(), clientId);

        }

        /// <summary>
        /// 보낸 클라를 제외하고 브로드캐스트
        /// </summary>
        public static void BroadCastNotSendClient(PacketSession session, IPacket packet)
        {

            Program.Room.BroadCast(packet.Write(), (session as ClientSession).SessionId);

        }

        public static void TurnChangePacket(PacketSession session, IPacket packet)
        {

            Program.Turn = Program.Turn == 0 ? 1 : 0;

            var pa = new TurnChangeBroadCastPacket();
            pa.turn = Program.Turn;

            Program.Room.BroadCast(pa.Write());

        }

    }
}
