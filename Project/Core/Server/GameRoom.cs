using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> sessions = new List<ClientSession>();
        object handler = new object();


        public void Enter(ClientSession session)
        {
            lock (handler)
            {   // 신규 유저 추가
                sessions.Add(session);
                session.Room = this;

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

                    //if (s.SessionId == clientId) continue;

                    s.Send(segment);    // 리스트에 들어있는 모든 클라에 전송
                }
            }
        }

    }
}
