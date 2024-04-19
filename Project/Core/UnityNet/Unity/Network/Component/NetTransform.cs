using Core;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityNet
{

    [RequireComponent(typeof(NetObject))]
    public class NetTransform : MonoBehaviour
    {

        [SerializeField] private float syncTime = 0.01f;

        public NetObject NetObj { get; private set; }

        private Vector3 oldPosition;
        private bool isSyncing;

        private void Awake()
        {

            oldPosition = transform.position;
            NetObj = GetComponent<NetObject>();
            StartCoroutine(SyncTransformCo());

        }

        public void Sync(Vector3 positon)
        {

            StartCoroutine(LerpTranformCo(positon));

        }

        private IEnumerator LerpTranformCo(Vector3 endPos)
        {


            isSyncing = true;

            float per = 0;
            var startPos = transform.position;

            while(per < 1)
            {

                yield return null;

                per += Time.deltaTime / syncTime;

                transform.position = Vector3.Lerp(startPos, endPos, per);

            }

            oldPosition = transform.position;
            isSyncing = false;

        }

        private IEnumerator SyncTransformCo()
        {

            var sec = new WaitForSeconds(syncTime);

            while (true)
            {

                yield return sec;

                if (isSyncing) continue;

                if (Vector3.Distance(oldPosition, transform.position) > 0.01f)
                {

                    //동기화 패킷
                    TransformLinkPacket packet = new TransformLinkPacket();
                    packet.position = transform.position;
                    packet.objectHash = NetObj.Hash;

                    NetworkManager.Instance.SendPacket(packet);

                }

            }

        }

    }

}
