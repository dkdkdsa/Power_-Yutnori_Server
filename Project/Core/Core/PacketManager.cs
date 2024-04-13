using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class PacketManager
    {

        protected PacketManager()
        {
            Register();
        }

        protected Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> makeFunc
            = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
        protected Dictionary<ushort, Action<PacketSession, IPacket>> handler
            = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        public abstract void Register();

        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer
            , Action<PacketSession, IPacket> onRecvCallback = null)  //  Action 콜백 : 입력되는 액션에 따라 Invoke
        {
            ushort count = 0;

            ushort size = 0;
            Serializer.Deserialize(ref size, ref buffer, ref count);

            ushort id = 0;
            Serializer.Deserialize(ref id, ref buffer, ref count);
            //count += 2;    // id값을 가지고 switch 대신 딕셔너리에서 값을 찾고 등록된 핸들러에서 해당 작업 Invoke

            Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
            if (makeFunc.TryGetValue(id, out func))
            {
                IPacket packet = func.Invoke(session, buffer);
                if (onRecvCallback != null) // 액션이 실행되면 Invoke 
                    onRecvCallback.Invoke(session, packet);
                else
                    HandlePacket(session, packet);  // 디폴트 실행
            }

        }
        // 패킷 생성 부분
        protected T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
        {
            T packet = new T();     
            packet.Read(buffer);    
            return packet;
        }

        public void HandlePacket(PacketSession session, IPacket packet)
        {
            Action<PacketSession, IPacket> action = null;
            if (handler.TryGetValue(packet.Protocol, out action))
            {
                action.Invoke(session, packet);
            }
        }
    }
}
