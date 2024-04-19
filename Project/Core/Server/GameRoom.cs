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

                var enterPacket = new GameEnterPacket(datas, session.SessionId);
                session.Send(enterPacket.Write());

            }

        }

        public void Leave(ClientSession session)
        {
            lock (handler)
            {

                sessions.Remove(session);

            }
        }

        public void BroadCast(ArraySegment<byte> segment, int clientId = -1)
        {
            ArraySegment<byte> packet = segment;

            lock (handler) // 
            {
                foreach (ClientSession s in sessions)
                {

                    if(clientId != -1)
                    {

                        if (s.SessionId == clientId) continue;

                    }

                    s.Send(segment);

                }
            }
        }

        public void AddObjectData(NetObjectData data)
        {

            datas.Add(data);

        }

        public void RemoveObjectData(int objectHash)
        {

            var x = datas.Find(x => x.hash == objectHash);

            datas.Remove(x);

        }

    }
}
