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
            makeFunc.Add((ushort)PacketType.GameEnterPacket, MakePacket<GameEnterPacket>);

            handler.Add((ushort)PacketType.NetPrefabSpawneing, NetPrefabHandle);
            handler.Add((ushort)PacketType.GameEnterPacket, GameEnterHandle);

        }

        private void NetPrefabHandle(PacketSession session, IPacket packet)
        {

            var prefabPacket = packet as NetPrefabSpawneingPacket;
            NetworkManager.Instance.SyncNetObject(prefabPacket.prefabName, prefabPacket.position, prefabPacket.rotation, prefabPacket.hash);

        }

        private void GameEnterHandle(PacketSession session, IPacket packet)
        {

            var prefabPacket = packet as GameEnterPacket;

            foreach(var item in prefabPacket.datas)
            {

                NetworkManager.Instance.SyncNetObject(item.prefabName, item.position, item.rotaitoin, item.hash);

            }

        }

    }

}
