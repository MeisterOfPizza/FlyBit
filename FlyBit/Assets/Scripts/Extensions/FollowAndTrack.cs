using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Extensions
{

    [DisallowMultipleComponent]
    class FollowAndTrack : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Transform target;

        [Header("Values")]
        [SerializeField] private Vector3 followOffset = Vector3.zero;
        [SerializeField] private Vector3 trackOffset  = Vector3.zero;

        [Space]
        [SerializeField] private float followSpeed = 2.5f;
        [SerializeField] private float trackSpeed  = 2.5f;

        [Space]
        [SerializeField] private CameraFollowType followType = CameraFollowType.Lerp;
        [SerializeField] private CameraTrackType  trackType  = CameraTrackType.Slerp;

        #endregion

        #region Enums

        private enum CameraFollowType
        {
            NoFollow,
            MoveTowards,
            Lerp,
            Slerp,
            Constant
        }

        private enum CameraTrackType
        {
            NoTrack,
            Lerp,
            Slerp,
            Instant
        }

        #endregion

        private void Awake()
        {
            if (target != null)
            {
                SetPosition(target.position);
                SetLookAt(target.position);
            }
        }

        public void Initialize(Transform target)
        {
            this.target = target;

            SetPosition(target.position);
            SetLookAt(target.position);
        }

        private void LateUpdate()
        {
            if (target != null)
            {
                switch (followType)
                {
                    case CameraFollowType.MoveTowards:
                        transform.position = Vector3.MoveTowards(transform.position, target.position + followOffset, followSpeed * Time.deltaTime);
                        break;
                    case CameraFollowType.Lerp:
                        transform.position = Vector3.Lerp(transform.position, target.position + followOffset, followSpeed * Time.deltaTime);
                        break;
                    case CameraFollowType.Slerp:
                        transform.position = Vector3.Slerp(transform.position, target.position + followOffset, followSpeed * Time.deltaTime);
                        break;
                    case CameraFollowType.Constant:
                        transform.position = target.position + followOffset;
                        break;
                    case CameraFollowType.NoFollow:
                    default:
                        break;
                }
                
                switch (trackType)
                {
                    case CameraTrackType.Lerp:
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position + trackOffset - transform.position), trackSpeed * Time.deltaTime);
                        break;
                    case CameraTrackType.Slerp:
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position + trackOffset - transform.position), trackSpeed * Time.deltaTime);
                        break;
                    case CameraTrackType.Instant:
                        transform.LookAt(target.position + trackOffset);
                        break;
                    case CameraTrackType.NoTrack:
                    default:
                        break;
                }
            }
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position + followOffset;
        }

        public void SetLookAt(Vector3 position)
        {
            transform.LookAt(position + trackOffset);
        }

    }

}
