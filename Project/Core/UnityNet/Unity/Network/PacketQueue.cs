using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace UnityNet
{
    public class PacketQueue
    {
        public static PacketQueue Instance { get; } = new PacketQueue();

        private Queue<IPacket> packetQueue = new Queue<IPacket>();
        private object handle = new object();

        public void Push(IPacket packet)
        {
            lock (handle)
            {
                packetQueue.Enqueue(packet);
            }
        }

        public IPacket Pop()
        {
            lock (handle)
            {
                if (packetQueue.Count == 0)
                    return null;

                return packetQueue.Dequeue();
            }
        }

        public List<IPacket> PopAll()
        {
            List<IPacket> list = new List<IPacket>();

            lock (handle)
            {
                while (packetQueue.Count > 0)
                    list.Add(packetQueue.Dequeue());
            }

            return list;
        }
    }
}
