using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface INetSerializeable
    {

        void Serialize(ref ArraySegment<byte> buffer, ref ushort count);
        void Deserialize(ref ArraySegment<byte> buffer, ref ushort count);

    }

}
