using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class PacketHandler
    {

        public static void DebugHandler(PacketSession session, IPacket packet)
        {

            DebugPacket movePacket = packet as DebugPacket;

            Console.WriteLine($"{movePacket.name}");
            Console.WriteLine(movePacket.position);


            //GameRoom room = clientSession.Room;
            //room.Leave(clientSession);
        }

    }
}
