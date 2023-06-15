using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using NaughtyAttributes;
using System.Threading.Tasks;

namespace StableDiffusion
{
    public class Img2Extras : SDBase
    {
        public static IEnumerator ProcessExtraCoroutine(ExtrasPayload extraInput, Texture2D[] textures, Renderer[] renderers, UnityEventTexture2D[] responseEvents)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                string url = $"http://{configInstance.username}:{configInstance.password}@{configInstance.address}";

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

                        SDFunctions.ApplyTexture2dToOutputs(textures[i], renderers == null ? null : renderers[i], responseEvents == null ? null : responseEvents[i]);
                    }
                }
            }
            yield return null;
        }

        public async static Task<Texture2D> ProcessExtraTask(ExtrasPayload extraInput, Texture2D texture)
        {
            string url = $"http://{configInstance.username}:{configInstance.password}@{configInstance.address}";

            extraInput.image = Convert.ToBase64String(texture.EncodeToPNG());
            string json = JsonUtility.ToJson(extraInput);
            using UnityWebRequest getExtras = UnityWebRequest.Put($"{url}/sdapi/v1/extra-single-image", json);
            {
                getExtras.method = "POST";
                getExtras.SetRequestHeader("Content-Type", "application/json");

                Debug.Log("SD: img2extra request Sent!");
                UnityWebRequestAsyncOperation requestTask = getExtras.SendWebRequest();
                while (!requestTask.isDone)
                    await Task.Yield();

                //Handle HTTP error
                if (getExtras.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"SD: img2extra request Failed: {getExtras.result} {getExtras.error}");
                    return null;
                }
                //Handle successful HTTP request
                else
                {
                    Debug.Log("SD: img2extra request Complete!");
                    // Access the response data from getReq.downloadHandler
                    string responseJsonData = getExtras.downloadHandler.text;

                    return GetTextureFromExtra(getExtras.downloadHandler.text);
                }
            }
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


        [System.Serializable]
        private class ExtraInputImage
        {
            public string data;
            public string name;
        }

        [System.Serializable]
        private class ExtraContainer
        {
            public string html_info;
            public string image;
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
}
