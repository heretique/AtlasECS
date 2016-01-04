using UnityEngine;


namespace Atlas
{
    public class AtlasComponent : MonoBehaviour
    {

        // Set this to false during Initialize or Awake to prevent the component
        // from being registered with RobotArms. This is useful for situations
        // where the object is created but not yet active such as with an object pool
        protected bool autoRegister = true;
        protected bool reRegisterOnSceneLoad;

        public void Awake()
        {
            this.RegisterComponent();
        }

        public void OnEnable()
        {
            this.RegisterComponent();
        }

        // This is needed for the very first update since Unity will call
        // Awake and OnEnable of a GameObject during the Instantiate call
        // before other objects can run their Awake, therefore the RobotArmsCoordinator
        // will not be set yet and we need to run the registration later
        // when we can guarantee that reference is set. On all subsequent
        // enable/disables OnEnable will function correctly.
        public void Start()
        {
            Initialize();
        }

        public void OnDisable()
        {
            this.UnregisterComponent();
        }

        public void OnLevelWasLoaded(int levelId)
        {
            if (reRegisterOnSceneLoad)
            {
                this.RegisterComponent();
            }
        }

        protected virtual void Initialize() { }

    }
}


