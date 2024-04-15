using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public struct NetObjectData : INetSerializeable
    {

        public int hash;
        public Vector3 position;
        public Quaternion rotaitoin;
        public string prefabName;

        public void Serialize(ref ArraySegment<byte> buffer, ref ushort count)
        {

            hash.Serialize(ref buffer, ref count);
            position.Serialize(ref buffer, ref count);
            rotaitoin.Serialize(ref buffer, ref count);
            prefabName.Serialize(ref buffer, ref count);

        }

        public void Deserialize(ref ArraySegment<byte> buffer, ref ushort count)
        {

            Serializer.Deserialize(ref hash, ref buffer, ref count);
            Serializer.Deserialize(ref position, ref buffer, ref count);
            Serializer.Deserialize(ref rotaitoin, ref buffer,ref count);
            Serializer.Deserialize(ref prefabName, ref buffer, ref count);

        }

    }

}
