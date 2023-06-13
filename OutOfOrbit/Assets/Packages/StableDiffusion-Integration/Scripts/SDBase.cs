using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StableDiffusion
{
    public class SDBase : MonoBehaviour
    {
        [SerializeField]
        private StableDiffusionConfig _config;

        public static StableDiffusionConfig configInstance;

        protected virtual void Awake()
        {
            AssignInstance();
        }

        protected virtual void OnValidate()
        {
            AssignInstance();
        }

        private void AssignInstance()
        {
            if (configInstance != null)
            {
                _config = configInstance;
            }

            if (_config != null && _config != configInstance)
            {
                configInstance = _config;
            }
            if (configInstance == null)
            {
                Debug.LogWarning("Config Instance not assigned to the stable diffusion scripts!");
            }
        }
    }
}
