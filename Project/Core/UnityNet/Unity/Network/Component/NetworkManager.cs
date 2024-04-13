using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityNet
{
    public class NetworkManager : MonoBehaviour
    {
        [Header("Network")]
        [SerializeField] private int port = 7777;

        [Header("Prefab")]
        [SerializeField] private NetworkPrefabs prefabs;

        private UnitySession session;

        public ulong ClientId { get; private set; }
        public bool IsConnected { get; private set; }
        public static NetworkManager Instance { get; private set; }

        private void Awake()
        {

            Instance = this;
            session = new UnitySession();

        }

        private void Update()
        {

            if (IsConnected)
            {

                List<IPacket> list = PacketQueue.Instance.PopAll();
                foreach (IPacket packet in list)
                    UnityPacketManager.Instance.HandlePacket(session, packet);

            }

        }
        public void Connected()
        {

            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = iphost.AddressList[1];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port); // IP주소, 포트번호 입력

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return session; });

            IsConnected = true;

        }

        public NetObject SpawnNetObject(string prefabName, Vector3 pos, Quaternion rot, bool synchronization = true)
        {

            var prefab = prefabs.prefabs.Find(x => x.name == prefabName);

            if(prefab == null)
            {

                Debug.LogError($"{prefabName}이라는 이름의 NetPrefab이 존재하지 않습니다!");
                return null;

            }

            if (synchronization) 
            {

                NetPrefabSpawneingPacket packet = new NetPrefabSpawneingPacket(pos, rot, prefabName);
                session.Send(packet.Write());

            }

            return Instantiate(prefab, pos, rot);

        }

        private void OnDestroy()
        {

            session.Disconnect();

        }

    }

}
