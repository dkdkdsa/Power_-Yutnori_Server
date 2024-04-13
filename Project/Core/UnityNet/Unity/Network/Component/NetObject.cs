using System;
using UnityEngine;

namespace UnityNet
{
    public class NetObject : MonoBehaviour
    {

        [HideInInspector] public int objectHash;

        public void Spawn(int objectHash)
        {

            this.objectHash = objectHash;

        }
        

    }

}
