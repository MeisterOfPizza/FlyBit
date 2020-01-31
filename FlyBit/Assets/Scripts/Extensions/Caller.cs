using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0649

namespace FlyBit.Extensions
{

    [DisallowMultipleComponent]
    sealed class Caller : MonoBehaviour
    {

        #region Editor

        public UnityEvent trigger;

        #endregion

        public void Trigger()
        {
            trigger?.Invoke();
        }

    }

}
