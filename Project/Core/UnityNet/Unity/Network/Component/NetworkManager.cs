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

        private Dictionary<int, NetObject> netObjectContainer = new Dictionary<int, NetObject>();
        private UnitySession session;

        public int ClientId { get; private set; }
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

        public NetObject SpawnNetObject(string prefabName, Vector3 pos, Quaternion rot)
        {

            var prefab = prefabs.prefabs.Find(x => x.name == prefabName);

            if(prefab == null)
            {

                Debug.LogError($"{prefabName}이라는 이름의 NetPrefab이 존재하지 않습니다!");
                return null;

            }

            int hash = Guid.NewGuid().GetHashCode();


            NetPrefabSpawneingPacket packet = new NetPrefabSpawneingPacket(hash, pos, rot, prefabName);
            session.Send(packet.Write());

            var obj = Instantiate(prefab, pos, rot);
            obj.Spawn(hash);

            netObjectContainer.Add(hash, obj);

            return obj;

        }

        public void SyncNetObject(string prefabName, Vector3 pos, Quaternion rot, int hash)
        {

            var prefab = prefabs.prefabs.Find(x => x.name == prefabName);

            if (prefab == null)
            {

                Debug.LogError($"{prefabName}이라는 이름의 NetPrefab이 존재하지 않습니다!");

            }

            var obj = Instantiate(prefab, pos, rot);
            obj.Spawn(hash);

            netObjectContainer.Add(hash, obj);

        }

        public void SetClientId(int clientId)
        {

            if (clientId != 0) return;

            ClientId = clientId;

        }

        public void LinkMethod(Action mehod, int senderHash)
        {

            var packet = new MethodLinkPacket(senderHash, mehod.Method.Name, mehod.Target.GetType().Name);
            session.Send(packet.Write());

        }

        public void LinkMethodInvoke(string method, string componentName, int hash) 
        {
            
            if(netObjectContainer.TryGetValue(hash, out var obj))
            {

                var compo = obj.GetComponent(componentName) as NetBehavior;

                if(compo == null)
                {

                    Debug.LogWarning($"컴포넌트가 누락되었습니다 이름 : {componentName}");

                }

                compo.Invoke(method, 0);

            }
            else
            {

                Debug.LogWarning($"해시값이 누락되었습니다 값 : {hash}");

            }

        }

        private void OnDestroy()
        {

            session.Disconnect();

        }

    }

}
