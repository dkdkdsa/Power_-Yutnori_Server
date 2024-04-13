using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityNet
{

    [CreateAssetMenu(menuName = "SO/Network/NetworkPrefabs")]
    public class NetworkPrefabs : ScriptableObject
    {

        [field: SerializeField] public List<NetObject> prefabs { get; private set; } = new List<NetObject>(); 

    }

}
