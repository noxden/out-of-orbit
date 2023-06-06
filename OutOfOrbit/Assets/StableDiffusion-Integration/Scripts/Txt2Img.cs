// Written by Sebastian Kostur, 30.04.2023 //
// H-DA Expanded Realities 6th Semester //

//Communicates to an installation of AUTOMATIC1111's Stable Diffusion WebUI API to generate images in Unity.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using NaughtyAttributes;
using UnityEngine.Events;
using UnityEditor.Events;

using System.Reflection;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace StableDiffusion
{
    public class Txt2Img : MonoBehaviour
    {
        [SerializeField, Label("Txt2img Input")] 
        private Txt2ImgPayload txt2imgInput = new Txt2ImgPayload();

        [Space(10)]
        public bool useRenderers = true;
        [ShowIf("useRenderers"), SerializeField]
        private Renderer[] targetRenderers = new Renderer[1];

        public bool useEvents = false;
        [ShowIf("useEvents"), SerializeField]
        private UnityEventTexture2D[] ResponseEvents;

        [System.Serializable]
        public class UnityEventTexture2D : UnityEvent<Texture2D>
        {
        }

        #region Functions
        //Generate images via the instance's inspector settings
        public void GenerateImages()
        {
            GenerateImages(txt2imgInput);
        }

        //overloaded method to allow generating images with different inputs
        public void GenerateImages(Txt2ImgPayload txt2imgInput)
        {
            StartCoroutine(GenerateImagesCoroutine(txt2imgInput, useRenderers? targetRenderers : null, useEvents? ResponseEvents : null));
        }

        //Overload for UnityEventTexture2d's only
        public static IEnumerator GenerateImagesCoroutine(Txt2ImgPayload txt2imgInput, UnityEventTexture2D[] responseEvents)
        {
            GenerateImagesCoroutine(txt2imgInput, null, responseEvents);
            yield return null;
        }

        //Overload for renderers only
        public static IEnumerator GenerateImagesCoroutine(Txt2ImgPayload txt2imgInput, Renderer[] renderers)
        {
            GenerateImagesCoroutine(txt2imgInput, renderers, null);
            yield return null;
        }

        public static IEnumerator GenerateImagesCoroutine(Txt2ImgPayload txt2imgInput, Renderer[] renderers, UnityEventTexture2D[] responseEvents)
        {
            WWWForm loginForm = new WWWForm();

            string url = $"http://{StableDiffusionConfig.instance.username}:{StableDiffusionConfig.instance.password}@{StableDiffusionConfig.instance.address}";

            loginForm.AddField("username", StableDiffusionConfig.instance.username);
            loginForm.AddField("password", StableDiffusionConfig.instance.password);
            using (UnityWebRequest loginRequest = UnityWebRequest.Post($"http://{StableDiffusionConfig.instance.address}/login/", loginForm))
            {
                yield return loginRequest.SendWebRequest();

                if (loginRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(loginRequest.error);
                }
                else
                {
                    Debug.Log("SD: Login successful!");

                    if (renderers == null && responseEvents == null)
                    {
                        Debug.LogError("SD: Tried to generate images without providing an output to apply the textures to!");
                        yield return null;
                    }
                        

                    txt2imgInput.Initialize(); //update hidden values to their true values
                    Texture2D[] textures = new Texture2D[0];

                    //Send request to server to generate a stable diffusion image
                    //Note:
                    //Normally we would be using Unity's UnityWebRequest.Post command like this:
                    //using (UnityWebRequest getReq = UnityWebRequest.Post($"http://{username}:{password}@141.100.233.171:4000/sdapi/v1/txt2img", postData))
                    //however, "For some reason UnityWebRequest applies URL encoding to POST message payloads." as seen here: https://forum.unity.com/threads/unitywebrequest-post-url-jsondata-sending-broken-json.414708/
                    //The solution is to first create it as a UnityWebRequest.Put request and to then change it to Post We then specify that it's a json and magically, it now works!

                    using UnityWebRequest getReq = UnityWebRequest.Put($"{url}/sdapi/v1/txt2img", JsonUtility.ToJson(txt2imgInput));
                    {
                        getReq.method = "POST";
                        getReq.SetRequestHeader("Content-Type", "application/json");

                        Debug.Log("SD: txt2img request Sent!");
                        yield return getReq.SendWebRequest();

                        //Handle HTTP error
                        if (getReq.result != UnityWebRequest.Result.Success)
                        {
                            Debug.Log($"SD: txt2img request Failed: {getReq.result} {getReq.error}");
                        }
                        //Handle successful HTTP request
                        else
                        {
                            Debug.Log("SD: txt2img request Complete!");
                            // Access the response data from getReq.downloadHandler
                            string responseJsonData = getReq.downloadHandler.text;

                            textures = GetTexturesFromtxt2img(getReq.downloadHandler.text);

                            ApplyTexture2dToOutputs(textures, renderers, responseEvents);
                        }
                    }

                    if (txt2imgInput.useExtra)
                    {
                        yield return ProcessExtraCoroutine(txt2imgInput.extraInput, textures, renderers, responseEvents);
                    }
                }
            }
            yield return null;
        }

        private static IEnumerator ProcessExtraCoroutine(ExtrasPayload extraInput, Texture2D[] textures, Renderer[] renderers, UnityEventTexture2D[] responseEvents)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                string url = $"http://{StableDiffusionConfig.instance.username}:{StableDiffusionConfig.instance.password}@{StableDiffusionConfig.instance.address}";

                extraInput.image = Convert.ToBase64String(textures[i].EncodeToPNG());
                string json = JsonUtility.ToJson(extraInput);
                using UnityWebRequest getExtras = UnityWebRequest.Put($"{url}/sdapi/v1/extra-single-image", json);
                {
                    getExtras.method = "POST";
                    getExtras.SetRequestHeader("Content-Type", "application/json");

                    Debug.Log("SD: img2extra request Sent!");
                    yield return getExtras.SendWebRequest();

                    //Handle HTTP error
                    if (getExtras.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"SD: img2extra request Failed: {getExtras.result} {getExtras.error}");
                    }
                    //Handle successful HTTP request
                    else
                    {
                        Debug.Log("SD: img2extra request Complete!");
                        // Access the response data from getReq.downloadHandler
                        string responseJsonData = getExtras.downloadHandler.text;

                        textures[i] = GetTextureFromExtra(getExtras.downloadHandler.text);

                        ApplyTexture2dToOutputs(textures[i], renderers[i], responseEvents[i]);
                    }
                }
            }
            yield return null;
        }

        private static void ApplyTexture2dToOutputs(Texture2D[] textures, Renderer[] renderers, UnityEventTexture2D[] responseEvents)
        {
            if (renderers != null)
                for (int i = 0; i < textures.Length; i++)
                    if (renderers[i] != null)
                        renderers[i].material.mainTexture = textures[i];

            if (responseEvents != null)
                for (int i = 0; i < responseEvents.Length; i++)
                    responseEvents[i]?.Invoke(textures[i]);
        }

        private static void ApplyTexture2dToOutputs(Texture2D texture, Renderer renderer, UnityEventTexture2D responseEvent)
        {
            if (renderer != null)
                renderer.material.mainTexture = texture;

            if (responseEvent != null)
                responseEvent?.Invoke(texture);
        }

        //Convert a json string to Sprite
        private static Texture2D[] GetTexturesFromtxt2img(string json)
        {
            List<Texture2D> texture2Ds = new List<Texture2D>();
            Txt2ImgContainer container = JsonUtility.FromJson<Txt2ImgContainer>(json);

            for (int i = 0; i < container.images.Length; i++)
            {
                byte[] b64_bytes = Convert.FromBase64String(container.images[i]); //convert theimage's strings to bytes.

                //load bytes into a new texture
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(b64_bytes);
                tex.Apply();

                texture2Ds.Add(tex);
            }
            
            return texture2Ds.ToArray();
        }

        private static Texture2D GetTextureFromExtra(string json)
        {
            ExtraContainer container = JsonUtility.FromJson<ExtraContainer>(json);
            byte[] b64_bytes = Convert.FromBase64String(container.image); //convert theimage's strings to bytes.

            //load bytes into a new texture
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(b64_bytes);
            tex.Apply();

            return tex;
        }
        #endregion

        #region Payloads
        [System.Serializable]
        public class Txt2ImgPayload
        {
            #region Default Settings
            [TextArea(1, 50)]
            public string prompt;
            [Label("Negative Prompt"), AllowNesting, TextArea(1, 50), Tooltip("exclude this prompt from the generation")]
            public string negative_prompt;

            [Label("Sampling Method"), AllowNesting, Tooltip("Which algorithm to use to produce the image")]
            public SamplerMethods samplerMethod = SamplerMethods.Euler_a;
            [HideInInspector, SerializeField]
            private string sampler_name;

            [Label("Sampling steps"), AllowNesting, Range(1, 150), Tooltip("How many times to improve the generated image iteratively;higher values take longer; very low values can produce bad results")]
            public int steps = 20;

            [Label("Batch Count"), AllowNesting, Range(1, 100), Tooltip("How many batches of images to create (has no impact on generation performance or VRAM usage)")]
            public int n_iter = 1;
            
            [Label("Batch Size"), AllowNesting, Range(1, 6), Tooltip("How many images to create in a single batch (increases generation performance at cost of higher VRAM usage)")]
            public int batch_size = 1;

            [Label("Restore Faces"), AllowNesting] 
            public bool restore_faces = false;
            [Tooltip("Produces an image that can be tiled")]
            public bool tiling = false;

            [Range(64, 2048)]
            public int width = 512;
            [Range(64, 2048)]
            public int height = 512;
            [Label("CFG Scale"), AllowNesting, Range(1, 30), Tooltip("Classifier Free Guidance Scale - how strongly the image should conform to prompt - lower values produce more creative results")]
            public float cfg_scale = 7;

            [Tooltip("-1 for random seed every time")]
            public int seed = -1;
            #endregion

            [Space(20)]

            #region High Resolution
            [Label("Use Upscaling"), AllowNesting, Tooltip("Use a two step process to partially create an image at smaller resolution, upscale, and then improve details in it without changing composition)")] 
            public bool enable_hr = false;

            [HideInInspector, SerializeField]
            private string hr_upscaler = "None";
            [ShowIf("enable_hr"), Label("Upscaler"), AllowNesting]
            public UpscalerModels upscalerModel = UpscalerModels.Latent;

            [ShowIf("enable_hr"), Label("Hires steps"), AllowNesting, Range(0, 150), Tooltip("Number of sampling steps for upscaled picture. If 0, use same as for original")]
            public float hr_second_pass_steps = 0;

            [ShowIf("enable_hr"), Label("Denoising Strength"), AllowNesting, Range(0, 1), Tooltip("Determines how little respect the algorithm should have for image's content. at 0, nothing will change, and at 1 you'll get an unrelated image. With values below 1.0, processing will take less steps than the sampling steps slider specifies")]
            public float denoising_strength = 0.7f;

            [ShowIf("enable_hr"), Label("Upscale by"), AllowNesting, Range(1, 4), Tooltip("Adjusts the size of the image by multiplying the original width and height by the selected value. Ignored if either Resize width to or Resize height to are non-zero")]
            public float hr_scale = 2;
            #endregion

            #region Extras
            //this is currently kinda bugged in the API and requires the extension https://github.com/AUTOMATIC1111/stable-diffusion-webui-rembg to be installed. When sending a request to the extras API, it removes the background even if disabled. Needs to be fixed with updates in the future
            [Label("Remove Background"), AllowNesting]
            public bool useExtra = false;
            //[SerializeField, ShowIf("useExtra"), Label("Extra Input"), AllowNesting]
            [HideInInspector] //hidden for now until API bugs are fixed
            public ExtrasPayload extraInput = new ExtrasPayload();
            #endregion

            public Txt2ImgPayload()
            {
                Initialize();
            }

            public void Initialize()
            {
                //set the upscaler string to the dropdown enum
                hr_upscaler = upscalerModel.GetStringValue();
                sampler_name = samplerMethod.GetStringValue();
            }
        }

        [System.Serializable]
        public class ExtrasPayload
        {
            [HideInInspector]
            public string image;

            public int resize_mode = 1;
            [Label("Resize")]
            public float upscaling_resize = 2;
            public string upscaler_1 = "None";


            #region rembg extension (Remove Background)
            //Currently not possible, commented out.
            /*
            //[HideInInspector]
            //public string model { get { return modelEnum.GetStringValue(); } }
            public BackgroundModels modelEnum = BackgroundModels.u2net;
            public string model = "None";
            public bool return_mask = false;
            public bool alpha_matting = false;
            public float alpha_matting_foreground_threshold = 240;
            public float alpha_matting_background_threshold = 10;
            public float alpha_matting_erode_size = 10;
            */
            #endregion
        }
        #endregion

        #region Enums
        public enum SamplerMethods
        {
            [StringValue("Euler a")]
            Euler_a,
            [StringValue("Euler")]
            Euler,
            [StringValue("LMS")]
            LMS,
            [StringValue("Heun")]
            Heun,
            [StringValue("DPM2")]
            DPM2,
            [StringValue("DPM2 a")]
            DPM2_a,
            [StringValue("DPM++ 2S a")]
            DPMpp_2S_a,
            [StringValue("DPM++ 2M")]
            DPMpp_2M,
            [StringValue("DPM++ SDE")]
            DPMpp_SDE,
            [StringValue("DPM fast")]
            DPM_fast,
            [StringValue("DPM adaptive")]
            DPM_adaptive,
            [StringValue("LMS Karras")]
            LMS_Karras,
            [StringValue("DPM2 Karras")]
            DPM2_Karras,
            [StringValue("DPM2 a Karras")]
            DPM2_a_Karras,
            [StringValue("DPM++ 2S a Karras")]
            DPMpp_2S_a_Karras,
            [StringValue("DPM++ 2M Karras")]
            DPMpp_2M_Karras,
            [StringValue("DPM++ SDE Karras")]
            DPMpp_SDE_Karras,
            [StringValue("DDIM")]
            DDIM,
            [StringValue("PLMS")]
            PLMS,
            [StringValue("UniPC")]
            UniPC
        }
        public enum UpscalerModels
        {
            [StringValue("None")]
            None,
            [StringValue("Latent")]
            Latent,
            [StringValue("Latent (antialiased)")]
            Latent_antialiased,
            [StringValue("Latent (bicubic)")]
            Latent_bicubic,
            [StringValue("Latent (bicubic antialiased)")]
            Latent_bicubic_antialiased,
            [StringValue("Latent (nearest)")]
            Latent_nearest,
            [StringValue("Latent (nearest-exact)")]
            Latent_nearest_exact,
            [StringValue("Lanczos")]
            Lanczos,
            [StringValue("Nearest")]
            Nearest,
            [StringValue("ESRGAN_4x")]
            ESRGAN_4x,
            [StringValue("LDSR")]
            LDSR,
            [StringValue("R-ESRGAN 4x+")]
            R_ESRGAN_4x,
            [StringValue("R-ESRGAN 4x+ Anime6B")]
            R_ESRGAN_4x_Anime6B,
            [StringValue("ScuNET GAN")]
            ScuNET_GAN,
            [StringValue("ScuNET PSNR")]
            ScuNET_PSNR,
            [StringValue("SwinIR 4x")]
            SwinIR_4x,
        }
        public enum BackgroundModels
        { 
            [StringValue("None")] 
            None,
            [StringValue("u2net")] 
            u2net,
            [StringValue("u2netp")] 
            u2netp,
            [StringValue("u2net_human_seg")] 
            u2net_human_seg,
            [StringValue("u2net_cloth_seg")] 
            u2net_cloth_seg,
            [StringValue("silueta")] 
            silueta
        }
        #endregion

        #region Return Containers

        [System.Serializable]
        private class ExtraInputImage
        {
            public string data;
            public string name;
        }

        [System.Serializable]
        private class Txt2ImgContainer
        {
            public string[] images;
            public string parameters;
            public string info;
        }

        [System.Serializable]
        private class ExtraContainer
        {
            public string html_info;
            public string image;
        }

        #endregion

        private void OnValidate()
        {
            if (txt2imgInput != null)
            {
                //Adjust the size of the renderer array to the new batch count and size.
                int imgCount = txt2imgInput.n_iter * txt2imgInput.batch_size;
                if (targetRenderers.Length != imgCount)
                {
                    Renderer[] newRendererArray = new Renderer[imgCount];
                    for (int i = 0; i < imgCount; i++)
                        if (targetRenderers.Length > i)
                            if (targetRenderers[i] != null)
                                newRendererArray[i] = targetRenderers[i]; //reassign the previous renderers to the adjusted array length so the stored information isn't lost
                    targetRenderers = newRendererArray;
                }

                //Adjust the size of the Unity Event array to the new batch count and size
                if (ResponseEvents.Length != imgCount)
                {
                    UnityEventTexture2D[] newEventsArray = new UnityEventTexture2D[imgCount];
                    for (int i = 0; i < imgCount; i++)
                        if (ResponseEvents.Length > i)
                            if (ResponseEvents[i] != null)
                                newEventsArray[i] = ResponseEvents[i]; //reassign the previous events to the adjusted event length so the stored information isn't lost
                    ResponseEvents = newEventsArray;
                }
            }
        }
    }
}