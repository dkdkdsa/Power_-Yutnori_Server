using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ServerPacketHandler
    {
        public static void PrefabHandle(PacketSession session, IPacket packet)
        {

            var clientSession = session as ClientSession;
            var cp = packet as NetPrefabSpawneingPacket;

            //Program.Room.BroadCast(cp.Write(), clientSession.SessionId);

            Console.WriteLine($"Send : {clientSession.SessionId}");

        }

    }
}
