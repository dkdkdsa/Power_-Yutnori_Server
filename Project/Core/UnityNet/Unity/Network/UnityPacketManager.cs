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
            makeFunc.Add((ushort)PacketType.MethodLinkPacket, MakePacket<MethodLinkPacket>);

            handler.Add((ushort)PacketType.NetPrefabSpawneing, NetPrefabHandle);
            handler.Add((ushort)PacketType.GameEnterPacket, GameEnterHandle);
            handler.Add((ushort)PacketType.MethodLinkPacket, LinkMethodHandle);

        }

        private void NetPrefabHandle(PacketSession session, IPacket packet)
        {

            var prefabPacket = packet as NetPrefabSpawneingPacket;
            NetworkManager.Instance.SyncNetObject(prefabPacket.prefabName, prefabPacket.position, prefabPacket.rotation, prefabPacket.hash);

        }

        private void GameEnterHandle(PacketSession session, IPacket packet)
        {

            var prefabPacket = packet as GameEnterPacket;

            NetworkManager.Instance.SetClientId(prefabPacket.clientId);

            foreach(var item in prefabPacket.datas)
            {

                NetworkManager.Instance.SyncNetObject(item.prefabName, item.position, item.rotaitoin, item.hash);

            }

        }

        private void LinkMethodHandle(PacketSession session, IPacket packet)
        {

            var p = packet as MethodLinkPacket;

            NetworkManager.Instance.LinkMethodInvoke(p.methodName, p.componentName, p.objectHash);

        }

    }

}
