//Communicates to an installation of AUTOMATIC1111's Stable Diffusion WebUI API to generate images via text in Unity.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using NaughtyAttributes;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace StableDiffusion
{
    public class Txt2Img : SDBase
    {
        [SerializeField, Label("Txt2img Input"), Space(10)]
        protected Txt2ImgPayload txt2imgInput = new Txt2ImgPayload();

        [Space(10)]
        public bool useRenderers = true;
        [ShowIf("useRenderers"), SerializeField]
        public Renderer[] targetRenderers = new Renderer[1];

        public bool useEvents = false;
        [ShowIf("useEvents"), SerializeField]
        public UnityEventTexture2D[] ResponseEvents;

        

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
            if (configInstance == null)
            {
                Debug.LogError("Stable Diffusion Config doesn't exist! Please create one.");
                yield break;
            }

            WWWForm loginForm = new WWWForm();

            string url = $"http://{configInstance.username}:{configInstance.password}@{configInstance.address}";

            loginForm.AddField("username", configInstance.username);
            loginForm.AddField("password", configInstance.password);
            using (UnityWebRequest loginRequest = UnityWebRequest.Post($"http://{configInstance.address}/login/", loginForm))
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
                            //Task<Texture2D[]> task = GetTexturesFromtxt2imgAsync(getReq.downloadHandler.text, txt2imgInput.rotate180);
                            //yield return new WaitUntil(() => task.IsCompleted);
                            //textures = task.Result;
                            textures = GetTexturesFromtxt2img(getReq.downloadHandler.text, txt2imgInput);

                            if (!txt2imgInput.useExtra || (txt2imgInput.showExtra && txt2imgInput.useExtra && txt2imgInput.showSteps)) //set texture to output if we are not using extra, or if we are using extra and showing the progress steps
                                SDFunctions.ApplyTexture2dToOutputs(textures, renderers, responseEvents);
                        }
                    }

                    if (txt2imgInput.showExtra && txt2imgInput.useExtra)
                    {
                        yield return Img2Extras.ProcessExtraCoroutine(txt2imgInput.extraInput, textures, renderers, responseEvents);
                    }
                }
            }
            yield return null;
        }

        public async static Task<Texture2D[]> GenerateImagesTask(Txt2ImgPayload txt2imgInput)
        {
            if (configInstance == null)
            {
                Debug.LogError("Stable Diffusion Config doesn't exist! Please create one.");
                return null;
            }

            WWWForm loginForm = new WWWForm();

            string url = $"http://{configInstance.username}:{configInstance.password}@{configInstance.address}";

            loginForm.AddField("username", configInstance.username);
            loginForm.AddField("password", configInstance.password);
            using (UnityWebRequest loginRequest = UnityWebRequest.Post($"http://{configInstance.address}/login/", loginForm))
            {
                UnityWebRequestAsyncOperation loginTask = loginRequest.SendWebRequest();

                while (!loginTask.isDone)
                    await Task.Yield();

                if (loginRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(loginRequest.error);
                    return null;
                }
                else
                {
                    Debug.Log("SD: Login successful!");


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
                        UnityWebRequestAsyncOperation requestTask = getReq.SendWebRequest();

                        while (!requestTask.isDone)
                            await Task.Yield();

                        //Handle HTTP error
                        if (getReq.result != UnityWebRequest.Result.Success)
                        {
                            Debug.Log($"SD: txt2img request Failed: {getReq.result} {getReq.error}");
                            return null;
                        }
                        //Handle successful HTTP request
                        else
                        {
                            Debug.Log("SD: txt2img request Complete!");
                            // Access the response data from getReq.downloadHandler
                            //Task<Texture2D[]> task = GetTexturesFromtxt2imgAsync(getReq.downloadHandler.text, txt2imgInput.rotate180);
                            //yield return new WaitUntil(() => task.IsCompleted);
                            //textures = task.Result;
                            textures = GetTexturesFromtxt2img(getReq.downloadHandler.text, txt2imgInput);
                        }
                    }

                    if (txt2imgInput.showExtra && txt2imgInput.useExtra)
                    {
                        Texture2D[] processedTextures = new Texture2D[textures.Length];

                        for(int i = 0; i < processedTextures.Length; i++)
                        {
                            processedTextures[i] = await Img2Extras.ProcessExtraTask(txt2imgInput.extraInput, textures[i]);
                        }
                        return processedTextures;
                    }
                    else
                    {
                        return textures;
                    }
                }
            }
        }




        //Convert a json string to Sprite
        private static Texture2D[] GetTexturesFromtxt2img(string json, Txt2ImgPayload input)
        {
            List<Texture2D> texture2Ds = new List<Texture2D>();
            Txt2ImgContainer container = JsonUtility.FromJson<Txt2ImgContainer>(json);

            for (int i = 0; i < container.images.Length; i++)
            {
                byte[] b64_bytes = Convert.FromBase64String(container.images[i]); //convert the image's strings to bytes.

                if (input.saveImageToFile)
                {
                    string path = $"{Application.persistentDataPath}/StableDiffusion/";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    path += $"{DateTime.Now.ToString("yyyy-dd-M-HHmmss")}_{i}.png";
                    Debug.Log($"SD: Saving image to {path}");
                    File.WriteAllBytes(path, b64_bytes);
                }

                //load bytes into a new texture
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(b64_bytes);

                if (input.rotate180)
                {
                    //reverse the array to rotate the image 180° as it is otherwise imported upside down
                    Color[] pix = tex.GetPixels();
                    Array.Reverse(pix, 0, pix.Length);
                    tex.SetPixels(pix);
                }
                tex.Apply();
                texture2Ds.Add(tex);
            }

            return texture2Ds.ToArray();
        }

        //Convert a json string to Sprite
        private static async Task<Texture2D[]> GetTexturesFromtxt2imgAsync(string json, bool rotate180)
        {
            Txt2ImgContainer container = JsonUtility.FromJson<Txt2ImgContainer>(json);
            Texture2D[] texture2Ds = new Texture2D[container.images.Length];

            for (int i = 0; i < texture2Ds.Length; i++)
                texture2Ds[i] = new Texture2D(1, 1);

            byte[][] bytesArray = new byte[container.images.Length][];
            await Task.Run(() =>
            {
                for (int i = 0; i < bytesArray.Length; i++)
                {
                    bytesArray[i] = Convert.FromBase64String(container.images[i]); //convert theimage's strings to bytes.
                }
            });

            for (int i = 0; i < bytesArray.Length; i++)
            {
                texture2Ds[i].LoadImage(bytesArray[i]);

                if (rotate180)
                {
                    //reverse the array to rotate the image 180° as it is otherwise imported upside down
                    Color[] pix = texture2Ds[i].GetPixels();
                    Array.Reverse(pix, 0, pix.Length);
                    texture2Ds[i].SetPixels(pix);
                }
            }
            return texture2Ds;
        }


        #endregion

        #region Return Containers
        [System.Serializable]
        private class Txt2ImgContainer
        {
            public string[] images;
            public string parameters;
            public string info;
        }

        #endregion

        protected override void OnValidate()
        {
            base.OnValidate();

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
                    {
                        if (ResponseEvents.Length > i)
                        {
                            if (ResponseEvents[i] != null)
                                newEventsArray[i] = ResponseEvents[i]; //reassign the previous events to the adjusted event length so the stored information isn't lost
                        }

                        //If the array is growing, assign the previous event's settings to the newly extended event.
                        if (i > 0 && ResponseEvents[i - 1] != null && newEventsArray[i] == null)
                        {
                            newEventsArray[i] = ResponseEvents[i - 1];
                        }
                    }
                    ResponseEvents = newEventsArray;
                }
            }
        }
    }

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

        [Label("Rotate by 180°"), AllowNesting, Tooltip("Should we rotate the resulting image(s) by 180°? This is a bugfix since the images are loaded upside down into unity.")]
        public bool rotate180 = false;

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
        public bool showExtra
        {
            get
            {
                if (Txt2Img.configInstance != null)
                    return Txt2Img.configInstance.stable_diffusion_webui_rembg;
                else
                    return true;
            }
        }
        [Label("Remove Background"), AllowNesting, ShowIf("showExtra")]
        public bool useExtra = false;
        [Label("Show Progress"), AllowNesting, ShowIf("useExtra")]
        public bool showSteps = true;
        //[SerializeField, ShowIf("useExtra"), Label("Extra Input"), AllowNesting]
        [HideInInspector] //hidden for now until API bugs are fixed
        public ExtrasPayload extraInput = new ExtrasPayload();
        #endregion

        public bool saveImageToFile = false;

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

        //Returns a copy of this class
        public Txt2ImgPayload Copy()
        {
            return (Txt2ImgPayload)this.MemberwiseClone();
        }
    }
}