using UnityEngine;
using UnityEngine.Events;

namespace StableDiffusion
{
    public class UnityMainEvents : MonoBehaviour
    {
        public UnityEvent OnStart;
        public UnityEvent OnUpdate;

        // Start is called before the first frame update
        void Start()
        {
            OnStart?.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}