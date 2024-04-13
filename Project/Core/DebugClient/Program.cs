using Core;
using System.Net;

namespace DebugClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // DNS
            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = iphost.AddressList[1];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // IP주소, 포트번호 입력

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.instance.Generate(); }, 1);

            while (true)
            {

                try
                {
                    //모든 세션들이 서버로 채팅 패킷 전송
                    SessionManager.instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(100);
            }
        }
    }

    public class ServerSession : PacketSession // 게임컨텐츠 영역
    {                           // 데이터의 송수신 구현보다, 송수신시의 동작 작성
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected bytes: {endPoint}");

        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected bytes: {endPoint}");
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)  // 리시브 이벤트 발생시 동작
        {
            //PacketManager.Instance.OnRecvPacket(this, buffer);  // 패킷매니저 시작(해석기 가동)
        }
        public override void OnSend(int numOfBytes)             // 샌드 이벤트 발생시 동작
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

    public class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager instance { get { return _session; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in _sessions)
                {
                    DebugPacket packet = new DebugPacket();
                    packet.name = "ASDF";
                    ArraySegment<byte> segment = packet.Write();

                    session.Send(segment);
                }
            }
        }
    }

}
