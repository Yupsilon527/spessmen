using UnityEngine;

    public abstract class Initializable : MonoBehaviour
    {
    protected bool initialized = false;
       public bool inspect = false;
        protected virtual void Initialize()
        {
            initialized = true;
        }
        protected virtual void Awake()
        {
            if (!initialized) Initialize();
        }
        protected virtual void OnEnable()
        {
            if (!initialized) Initialize();
        }
        public void FindComponent<T>(ref T att) where T : Component
    {
            if (att == null)
                att = GetComponentInChildren<T>();
        }
        public void Inspect(string str)
        {
            if (inspect)
                Debug.Log($"[{name}] {str}");
        }
    }
