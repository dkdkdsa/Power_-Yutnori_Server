using System;
using UnityEngine;

namespace UnityNet
{

    [DisallowMultipleComponent]
    public class NetObject : MonoBehaviour
    {

        public int objectHash { get; private set; }

        public void Spawn(int objectHash)
        {

            this.objectHash = objectHash;

        }
        

    }

}
