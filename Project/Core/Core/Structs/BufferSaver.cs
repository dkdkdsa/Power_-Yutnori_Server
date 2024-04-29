using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 매계변수 동기화에 사용되는 버퍼 저장기
    /// </summary>
    public struct BufferSaver : INetSerializeable
    {

        public ushort startCount;
        public byte[] buffer;

        public void Deserialize(ref ArraySegment<byte> buffer, ref ushort count)
        {

            this.buffer = new byte[128];
            //받은 배열에서 복사
            Array.Copy(buffer.Array, buffer.Offset + count, this.buffer, 0, 128);
            count += 128;

        }

        public void Serialize(ref ArraySegment<byte> buffer, ref ushort count)
        {

            //저장 되어있는 배열을 복사
            Array.Copy(this.buffer, 0, buffer.Array, buffer.Offset + count, 128);
            count += 128;

        }

        public void Saving<T>(T obj) where T : INetSerializeable
        {

            this.buffer = new byte[128];
            var seg = new ArraySegment<byte>(buffer);

            //저장 배열에 복사
            obj.Serialize(ref seg, ref startCount);

        }

        public INetSerializeable Casting(Type type)
        {

            var obj = Activator.CreateInstance(type) as INetSerializeable;
            var seg = new ArraySegment<byte>(buffer);

            ushort cnt = 0;
            obj.Deserialize(ref seg, ref cnt);

            return obj;

        }

    }

}
