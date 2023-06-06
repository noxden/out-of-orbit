using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace StableDiffusion
{
    [CreateAssetMenu(fileName = "SD Config", menuName = "Stable Diffusion/Config File", order = 1)]
    public class StableDiffusionConfig : ScriptableSingleton<StableDiffusionConfig>
    {
        [Header("Login")]
        [Tooltip("URL address, without http:// at the start and without the backslash at the end")]
        [SerializeField] public string address;
        [SerializeField] public string username;
        [SerializeField] public string password;

        /*
        #region Installed Extensions
        
        [Header("Extensions")]
        public bool ABG_extension = false;
        public bool stable_diffusion_webui_rembg = false;
        #endregion
        */
    }
}