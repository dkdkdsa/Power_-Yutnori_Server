using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class SendBufferHelper 
    {

        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(
            () => { return null; });
        public static int chunkSize { get; set; } = 4096; // 사이즈 변경 희망시

        public static ArraySegment<byte> Open(int reserveSize) 
        {

            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(chunkSize);
            if (CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(chunkSize);

            return CurrentBuffer.Value.Open(reserveSize);

        }
        public static ArraySegment<byte> Close(int usedSize) 
        {

            return CurrentBuffer.Value.Close(usedSize);

        }

    }



    public class SendBuffer 
    {

        private byte[] buffer; 
        private int usedSize = 0;
        public int FreeSize { get { return buffer.Length - usedSize; } }

        public SendBuffer(int chunkSize)
        {

            buffer = new byte[chunkSize];

        }
        public ArraySegment<byte> Open(int reserveSize)
        {

            if (reserveSize > FreeSize)
                return default;

            return new ArraySegment<byte>(buffer, usedSize, reserveSize);

        }

        public ArraySegment<byte> Close(int usedSize)
        {

            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, this.usedSize, usedSize);
            this.usedSize += usedSize;
            return segment;

        }

    }

}
