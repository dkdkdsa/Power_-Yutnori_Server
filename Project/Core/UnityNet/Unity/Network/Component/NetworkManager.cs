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
        public int CurrentTurn { get; private set; }
        public static NetworkManager Instance { get; private set; }

        public event Action<int> OnTurnChangeEvent;
        public event Action OnNetworkConnected;

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
        public void Connect()
        {

            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = iphost.AddressList[1];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port); // IP주소, 포트번호 입력

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return session; });

            IsConnected = true;

        }

        public void TurnChanged(int currentTurn)
        {

            Debug.Log(currentTurn);
            OnTurnChangeEvent(currentTurn);
            CurrentTurn = currentTurn;

        }

        public NetObject SpawnNetObject(string prefabName, Vector3 pos, Quaternion rot, int ownerClientId = -1)
        {

            var prefab = prefabs.prefabs.Find(x => x.name == prefabName);

            if(prefab == null)
            {

                Debug.LogError($"{prefabName}이라는 이름의 NetPrefab이 존재하지 않습니다!");
                return null;

            }

            int hash = Guid.NewGuid().GetHashCode();


            NetPrefabSpawneingPacket packet = new NetPrefabSpawneingPacket(hash, pos, rot, prefabName, ownerClientId);
            session.Send(packet.Write());

            var obj = Instantiate(prefab, pos, rot);
            obj.Spawn(hash, ownerClientId);

            netObjectContainer.Add(hash, obj);

            return obj;

        }

        public void SyncNetObject(string prefabName, Vector3 pos, Quaternion rot, int hash, int ownerClientId = -1)
        {

            var prefab = prefabs.prefabs.Find(x => x.name == prefabName);

            if (prefab == null)
            {

                Debug.LogError($"{prefabName}이라는 이름의 NetPrefab이 존재하지 않습니다!");

            }

            var obj = Instantiate(prefab, pos, rot);
            obj.Spawn(hash, ownerClientId);

            netObjectContainer.Add(hash, obj);

        }

        public void SetClientId(int clientId)
        {

            ClientId = clientId;
            OnNetworkConnected?.Invoke();

        }

        public NetObject FindNetObject(int hash)
        {

            return netObjectContainer[hash];

        }

        public void LinkMethod(Action method, int senderHash, string callCompoName, bool immediatelyCall = false)
        {

            var netobj = netObjectContainer[senderHash];

            if (netobj.IsHaveOwner == true && netobj.OwnerCliendId != ClientId)
            {

                Debug.LogWarning($"이 클라이언트는 {netobj.name}의 Owner가 아닙니다 Owner만 매서드를 동기화 할 수 있습니다");
                return;

            }

            if (immediatelyCall)
            {

                method.Invoke();

            }

            var packet = new MethodLinkPacket(senderHash, method.Method.Name, callCompoName, immediatelyCall);
            session.Send(packet.Write());

        }

        public void LinkMethod<T>(Action<T> method, int senderHash, T param, string callCompoName, bool immediatelyCall = false) where T : INetSerializeable
        {

            var netobj = netObjectContainer[senderHash];

            if (netobj.IsHaveOwner == true && netobj.OwnerCliendId != ClientId)
            {

                Debug.LogWarning($"이 클라이언트는 {netobj.name}의 Owner가 아닙니다 Owner만 매서드를 동기화 할 수 있습니다");
                return;

            }

            if (immediatelyCall)
            {

                method.Invoke(param);

            }

            var packet = new MethodLinkPacketParam(senderHash, method.Method.Name, callCompoName, param, immediatelyCall);
            session.Send(packet.Write());

        }

        public void LinkMethodInvoke(string method, string componentName, int hash) 
        {
            
            if(netObjectContainer.TryGetValue(hash, out var obj))
            {

                var compo = obj.GetComponent(componentName) as NetBehavior;

                if(compo == null)
                {

                    Debug.LogWarning($"컴포넌트가 누락되었습니다 이름 : {componentName}, 해시 : {hash}, 매서드 : {method}");
                    return;

                }

                compo.Invoke(method, 0);

            }
            else
            {

                Debug.LogWarning($"해시값이 누락되었습니다 값 : {hash}");

            }

        }

        public void LinkMethodInvoke(string method, string componentName, int hash, INetSerializeable param)
        {

            if (netObjectContainer.TryGetValue(hash, out var obj))
            {

                var compo = obj.GetComponent(componentName) as NetBehavior;

                if (compo == null)
                {

                    Debug.LogWarning($"컴포넌트가 누락되었습니다 이름 : {componentName}, 해시 : {hash}, 매서드 : {method}");

                }

                var t = compo.GetType();

                var info = t.GetMethod(method, new[] { param.GetType() });

                info.Invoke(compo, new[]{ param });

            }
            else
            {

                Debug.LogWarning($"해시값이 누락되었습니다 값 : {hash}");

            }

        }

        public void SendPacket(IPacket packet)
        {

            if (!IsConnected) return;

            session.Send(packet.Write());

        }

        private void OnDestroy()
        {

            session.Disconnect();

        }

    }

}
