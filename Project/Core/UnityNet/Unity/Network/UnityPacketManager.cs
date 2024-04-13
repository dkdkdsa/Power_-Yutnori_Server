using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityNet
{
    public class UnityPacketManager : PacketManager
    {

        public static UnityPacketManager Instance { get; } = new UnityPacketManager();

        public override void Register()
        {

            makeFunc.Add((ushort)PacketType.NetPrefabSpawneing, MakePacket<NetPrefabSpawneingPacket>);
            handler.Add((ushort)PacketType.NetPrefabSpawneing, NetPrefabHandle);

        }

        private void NetPrefabHandle(PacketSession session, IPacket packet)
        {

            var prefabPacket = packet as NetPrefabSpawneingPacket;
            NetworkManager.Instance.SpawnNetObject(prefabPacket.prefabName, prefabPacket.position, prefabPacket.rotation, false);

        }

    }

}
