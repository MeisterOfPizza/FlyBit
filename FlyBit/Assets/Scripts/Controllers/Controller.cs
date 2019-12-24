namespace FlyBit.Controllers
{

    /// <summary>
    /// Base class for all controllers.
    /// Do not override Unity's default Awake, use <see cref="OnAwake"/> instead.
    /// </summary>
    public abstract class Controller<T> : MonoController where T : Controller<T>
    {

        #region Static properties

        public static T Singleton { get; protected set; }

        #endregion

        #region MonoBehaviour methods

        public override void Awake()
        {
            Singleton = (T)this;

            if (!AwakeCalled)
            {
                OnAwake();
                AwakeCalled = true;
            }
        }

        public override void OnAwake()
        {
            // Do nothing here.
            // Not all classes that inherit from this will use the awake methods.
        }

        public override void LateStart()
        {
            // Do nothing here.
            // Not all classes that inherit from this will use the start methods.
        }

        #endregion

    }

}
