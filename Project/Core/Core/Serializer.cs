using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public static class Serializer
    {

        public static void Serialize(this Vector3 vector, ref ArraySegment<byte> buffer, ref ushort count)
        {

            Array.Copy(BitConverter.GetBytes(vector.x), 0, buffer.Array, buffer.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(vector.y), 0, buffer.Array, buffer.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(vector.z), 0, buffer.Array, buffer.Offset + count, sizeof(float));
            count += sizeof(float);

        }

        public static void Serialize(this Quaternion quaternion, ref ArraySegment<byte> buffer, ref ushort count)
        {

            Array.Copy(BitConverter.GetBytes(quaternion.x), 0, buffer.Array, buffer.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(quaternion.y), 0, buffer.Array, buffer.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(quaternion.z), 0, buffer.Array, buffer.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(quaternion.w), 0, buffer.Array, buffer.Offset + count, sizeof(float));
            count += sizeof(float);

        }

        public static void Serialize(this ushort value, ref ArraySegment<byte> buffer, ref ushort count)
        {

            Array.Copy(BitConverter.GetBytes(value), 0, buffer.Array, buffer.Offset + count, sizeof(ushort));
            count += sizeof(ushort);


        }

        public static void Serialize(this ushort value, ref ArraySegment<byte> buffer)
        {

            Array.Copy(BitConverter.GetBytes(value), 0, buffer.Array, buffer.Offset, sizeof(ushort));

        }

        public static void Serialize(this string value, ref ArraySegment<byte> buffer, ref ushort count)
        {

            var array = Encoding.UTF8.GetBytes(value);

            ((ushort)array.Length).Serialize(ref buffer, ref count);

            Array.Copy(array, 0, buffer.Array, buffer.Offset + count, array.Length);
            count += (ushort)array.Length;


        }

        public static void Deserialize(ref Vector3 vector, ref ArraySegment<byte> buffer, ref ushort count)
        {

            vector.x =  BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(float);
            vector.y = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(float);
            vector.z = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(float);

        }

        public static void Deserialize(ref Quaternion quaternion, ref ArraySegment<byte> buffer, ref ushort count)
        {

            quaternion.x = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(float);
            quaternion.y = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(float);
            quaternion.z = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(float);
            quaternion.w = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(float);

        }

        public static void Deserialize(ref ushort value, ref ArraySegment<byte> buffer, ref ushort count)
        {
            
            value = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);

        }

        public static void Deserialize(ref string value, ref ArraySegment<byte> buffer, ref ushort count)
        {

            ushort lenght = 0;
            Deserialize(ref lenght, ref buffer, ref count);

            value = Encoding.UTF8.GetString(buffer.Array, count, lenght);
            count += lenght;

        }

    }

}
