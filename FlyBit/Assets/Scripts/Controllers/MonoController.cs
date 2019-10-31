using UnityEngine;

namespace FlyBit.Controllers
{

    /// <summary>
    /// Base class for the <see cref="Controller{T}"/> class.
    /// This acts as a wrapper class when referencing <see cref="Controller{T}"/> classes in the Unity editor.
    /// </summary>
    abstract class MonoController : MonoBehaviour
    {

        protected bool AwakeCalled { get; set; }

        public abstract void Awake();
        public abstract void OnAwake();
        public abstract void LateStart();

    }

}
