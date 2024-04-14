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
            handler.Add((ushort)PacketType.NetPrefabSpawneing, PrefabSpawnHandle);
            //makeFunc.Add((ushort)PacketType.None, MakePacket<DebugPacket>);
            //handler.Add((ushort)PacketType.None, PacketHandler.DebugHandler);

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
                prefabName = prefabPacket.prefabName

            };

            Program.Room.AddObjectData(data);

        }

    }
}
