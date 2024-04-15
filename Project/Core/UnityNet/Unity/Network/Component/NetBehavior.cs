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

        public void LinkMethod(Action method)
        {

            NetworkManager.Instance.LinkMethod(method, NetObject.objectHash);

        }

    }

}
