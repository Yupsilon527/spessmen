using UnityEngine;

namespace VikingParty
{
    public abstract class Initializable : MonoBehaviour
    {
        bool initialized = false;
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
        public void FindComponent<T>(ref T att) where T : MonoBehaviour
        {
            if (att == null)
                att = GetComponent<T>();
        }
        public void Inspect(string str)
        {
            if (inspect)
                Debug.Log($"[{name}] {str}");
        }
    }
}
