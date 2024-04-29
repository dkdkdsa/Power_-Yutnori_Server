using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityNet
{

    [RequireComponent(typeof(NetObject))]
    public abstract class NetBehavior : MonoBehaviour
    {

        public NetObject NetObject { get; private set; }

        protected virtual void Start()
        {

            NetObject = GetComponent<NetObject>();

        }

        public void LinkMethod(Action method, bool immediatelyCall = false)
        {

            Debug.Log(GetType().Name);
            NetworkManager.Instance.LinkMethod(method, NetObject.Hash, GetType().Name, immediatelyCall);

        }

        public void LinkMethod<T>(Action<T> method, T param, bool immediatelyCall = false) where T : INetSerializeable
        {

            NetworkManager.Instance.LinkMethod<T>(method, NetObject.Hash, param, GetType().Name, immediatelyCall);

        }

    }

}
