﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core
{

    public enum PacketType
    {

        None,
        NetPrefabSpawneing,
        GameEnterPacket,
        ClientJoinPacket,
        MethodLinkPacket


    }

    public interface IPacket
    {
        ushort Protocol { get; }
        void Read(ArraySegment<byte> segment);
        ArraySegment<byte> Write();
    }

    public class DebugPacket : IPacket
    {
        public ushort Protocol => (ushort)PacketType.None;

        public string name;
        public Vector3 position = new Vector3(10, 10, 10);

        public void Read(ArraySegment<byte> segment)
        {

            ushort count = 0;
            count += sizeof(ushort);

            Serializer.Deserialize(ref position, ref segment, ref count);
            Serializer.Deserialize(ref name, ref segment, ref count);

        }

        public ArraySegment<byte> Write()
        {

            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            Protocol.Serialize(ref segment, ref count);
            position.Serialize(ref segment, ref count);
            name.Serialize(ref segment, ref count);

            return SendBufferHelper.Close(count);

        }

    }

    #region Packet

    public class NetPrefabSpawneingPacket : IPacket
    {
        public ushort Protocol => (ushort)PacketType.NetPrefabSpawneing;

        public NetPrefabSpawneingPacket(int hash, Vector3 position, Quaternion rotation, string prefabName)
        {

            this.hash = hash;
            this.position = position;
            this.rotation = rotation;
            this.prefabName = prefabName;

        }

        public NetPrefabSpawneingPacket() { }

        public int hash;
        public Vector3 position;
        public Quaternion rotation;
        public string prefabName;

        public void Read(ArraySegment<byte> segment)
        {

            ushort count = sizeof(ushort);
            count += sizeof(ushort);

            Serializer.Deserialize(ref hash, ref segment, ref count);
            Serializer.Deserialize(ref position, ref segment, ref count);
            Serializer.Deserialize(ref rotation, ref segment, ref count);
            Serializer.Deserialize(ref prefabName, ref segment, ref count);

        }

        public ArraySegment<byte> Write()
        {

            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = sizeof(ushort);

            Protocol.Serialize(ref segment, ref count);
            hash.Serialize(ref segment, ref count);
            position.Serialize(ref segment, ref count);
            rotation.Serialize(ref segment, ref count);
            prefabName.Serialize(ref segment, ref count);
            count.Serialize(ref segment);

            return SendBufferHelper.Close(count);

        }

    }

    public class GameEnterPacket : IPacket
    {
        public ushort Protocol => (ushort)PacketType.GameEnterPacket;

        public List<NetObjectData> datas = new List<NetObjectData>();
        public int clientId;

        public GameEnterPacket(List<NetObjectData> datas, int clientId)
        {
            this.datas = datas;
            this.clientId = clientId;
        }

        public GameEnterPacket() { }

        public void Read(ArraySegment<byte> segment)
        {

            ushort count = sizeof(ushort);
            count += sizeof(ushort);

            Serializer.Deserialize(datas, ref segment, ref count);
            Serializer.Deserialize(ref clientId, ref segment, ref count);

        }

        public ArraySegment<byte> Write()
        {

            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = sizeof(ushort);

            Protocol.Serialize(ref segment, ref count);
            datas.Serialize(ref segment, ref count);
            clientId.Serialize(ref segment, ref count);

            Serializer.Serialize(count, ref segment);

            return SendBufferHelper.Close(count);

        }

    }

    public class ClientJoinPacket : IPacket
    {
        public ushort Protocol => (ushort)PacketType.ClientJoinPacket;

        public int clientId;

        public void Read(ArraySegment<byte> segment)
        {

            ushort count = sizeof(ushort);
            count += sizeof(ushort);

            Serializer.Deserialize(ref clientId, ref segment, ref count);

        }

        public ArraySegment<byte> Write()
        {

            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = sizeof(ushort);

            Protocol.Serialize(ref segment, ref count);
            clientId.Serialize(ref segment, ref count);

            Serializer.Serialize(count, ref segment);

            return SendBufferHelper.Close(count);

        }

    }

    public class MethodLinkPacket : IPacket
    {
        public ushort Protocol => (ushort)PacketType.MethodLinkPacket;

        public int objectHash;
        public string methodName;
        public string componentName;

        public MethodLinkPacket(int objectHash, string methodName, string componentName)
        {
            this.objectHash = objectHash;
            this.methodName = methodName;
            this.componentName = componentName;
        }

        public MethodLinkPacket() { }

        public void Read(ArraySegment<byte> segment)
        {

            ushort count = sizeof(ushort);
            count += sizeof(ushort);

            Serializer.Deserialize(ref objectHash, ref segment, ref count);
            Serializer.Deserialize(ref methodName, ref segment, ref count);
            Serializer.Deserialize(ref componentName, ref segment, ref count);

        }

        public ArraySegment<byte> Write()
        {

            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = sizeof(ushort);

            Protocol.Serialize(ref segment, ref count);
            objectHash.Serialize(ref segment, ref count);
            methodName.Serialize(ref segment, ref count);
            componentName.Serialize(ref segment, ref count);

            count.Serialize(ref segment);

            return SendBufferHelper.Close(count);

        }

    }

    #endregion

}
