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
            handler.Add((ushort)PacketType.NetPrefabSpawneing, ServerPacketHandler.PrefabHandle);
            //makeFunc.Add((ushort)PacketType.None, MakePacket<DebugPacket>);
            //handler.Add((ushort)PacketType.None, PacketHandler.DebugHandler);

        }

        

    }
}
