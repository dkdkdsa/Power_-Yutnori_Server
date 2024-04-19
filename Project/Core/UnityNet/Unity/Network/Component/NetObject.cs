using Core;
using System;
using UnityEngine;

namespace UnityNet
{

    [DisallowMultipleComponent]
    public class NetObject : MonoBehaviour
    {

        public bool IsOwner => NetworkManager.Instance.ClientId == OwnerCliendId;
        public bool IsHaveOwner => OwnerCliendId != -1;

        public int Hash { get; private set; }
        public int OwnerCliendId { get; private set; } = -1;

        public void Spawn(int objectHash, int ownerClientId)
        {

            Hash = objectHash;
            OwnerCliendId = ownerClientId;

        }
        
        public void Despawn()
        {

            Destroy(gameObject);

            var p = new DespawnObjectPacket();
            p.objectHash = Hash;

            NetworkManager.Instance.SendPacket(p);

        }

    }

}
