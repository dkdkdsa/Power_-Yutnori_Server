using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom
    {

        private List<NetObjectData> datas = new List<NetObjectData>();
        private List<ClientSession> sessions = new List<ClientSession>();
        private object handler = new object();


        public void Enter(ClientSession session)
        {
            lock (handler)
            { 

                sessions.Add(session);
                session.Room = this;

                if(datas.Count > 0 )
                {

                    var enterPacket = new GameEnterPacket(datas);
                    session.Send(enterPacket.Write());

                }

            }

        }

        public void Leave(ClientSession session)
        {
            lock (handler)
            {
                // 플레이어 제거하고
                sessions.Remove(session);


            }
        }

        public void BroadCast(ArraySegment<byte> segment, int clientId)
        {
            ArraySegment<byte> packet = segment;

            lock (handler) // 
            {
                foreach (ClientSession s in sessions)
                {

                    if (s.SessionId == clientId) continue;

                    s.Send(segment);    // 리스트에 들어있는 모든 클라에 전송
                }
            }
        }

        public void AddObjectData(NetObjectData data)
        {

            datas.Add(data);

        }

    }
}
