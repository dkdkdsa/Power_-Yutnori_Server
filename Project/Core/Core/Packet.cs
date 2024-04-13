using System;
using System.Collections.Generic;
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

            return segment;

        }

    }

    #region Packet

    public class NetPrefabSpawneingPacket : IPacket
    {
        public ushort Protocol => (ushort)PacketType.NetPrefabSpawneing;

        public NetPrefabSpawneingPacket(Vector3 position, Quaternion rotation, string prefabName)
        {

            this.position = position;
            this.rotation = rotation;
            this.prefabName = prefabName;

        }

        public NetPrefabSpawneingPacket() { }   

        public Vector3 position;
        public Quaternion rotation;
        public string prefabName;

        public void Read(ArraySegment<byte> segment)
        {

            ushort count = 0;
            count += sizeof(ushort);

            Serializer.Deserialize(ref position, ref segment, ref count);
            Serializer.Deserialize(ref rotation, ref segment, ref count);
            Serializer.Deserialize(ref prefabName, ref segment, ref count);

        }

        public ArraySegment<byte> Write()
        {

            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            Protocol.Serialize(ref segment, ref count);
            position.Serialize(ref segment, ref count);
            rotation.Serialize(ref segment, ref count);
            prefabName.Serialize(ref segment, ref count);

            return segment;

        }

    }

    #endregion

}
