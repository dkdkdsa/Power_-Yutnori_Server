using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            makeFunc.Add((ushort)PacketType.MethodLinkParamPacket, MakePacket<MethodLinkPacketParam>);
            makeFunc.Add((ushort)PacketType.TransformLinkPacket, MakePacket<TransformLinkPacket>);

            handler.Add((ushort)PacketType.NetPrefabSpawneing, NetPrefabHandle);
            handler.Add((ushort)PacketType.GameEnterPacket, GameEnterHandle);
            handler.Add((ushort)PacketType.MethodLinkPacket, LinkMethodHandle);
            handler.Add((ushort)PacketType.MethodLinkParamPacket, LinkMethodParamHandle);
            handler.Add((ushort)PacketType.TransformLinkPacket, LinkTransformHandle);

        }

        private void NetPrefabHandle(PacketSession session, IPacket packet)
        {

            var prefabPacket = packet as NetPrefabSpawneingPacket;
            NetworkManager.Instance.SyncNetObject(prefabPacket.prefabName, prefabPacket.position, prefabPacket.rotation, prefabPacket.hash, prefabPacket.ownerId);

        }

        private void GameEnterHandle(PacketSession session, IPacket packet)
        {

            var prefabPacket = packet as GameEnterPacket;

            NetworkManager.Instance.SetClientId(prefabPacket.clientId);

            foreach(var item in prefabPacket.datas)
            {

                NetworkManager.Instance.SyncNetObject(item.prefabName, item.position, item.rotaitoin, item.hash, item.ownerId);

            }

        }

        private void LinkMethodHandle(PacketSession session, IPacket packet)
        {

            var p = packet as MethodLinkPacket;

            NetworkManager.Instance.LinkMethodInvoke(p.methodName, p.componentName, p.objectHash);

        }

        private void LinkMethodParamHandle(PacketSession session, IPacket packet)
        {

            var p = packet as MethodLinkPacketParam;

            LocalNetworkEvent.MethodLinkEvent?.Invoke(p);
           

        }

        public static void LinkTransformHandle(PacketSession session, IPacket packet)
        {

            var p = packet as TransformLinkPacket;

            var trm = NetworkManager.Instance.FindNetObject(p.objectHash).GetComponent<NetTransform>();
            trm.Sync(p.position);

        }

    }

}
