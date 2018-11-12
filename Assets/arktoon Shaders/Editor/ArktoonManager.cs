using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEditor.Callbacks;
using System.Text.RegularExpressions;

namespace ArktoonShaders
{
    public class ArktoonManager : MonoBehaviour
    {
        static string url = "https://api.github.com/repos/synqark/Arktoon-Shaders/releases/latest";
        static UnityWebRequest www;

        [DidReloadScripts(0)]
        static void BeginCheckUpdate ()
        {
            Debug.Log ("アップデートをチェック中");

            www = UnityWebRequest.Get(url);
            www.Send();
            EditorApplication.update += EditorUpdate;
        }

        static void EditorUpdate()
        {
            while (!www.isDone)
                return;

            if (www.isError)
                Debug.Log(www.error);
            else
                updateHandler(www.downloadHandler.text);

            EditorApplication.update -= EditorUpdate;
        }


        static void updateHandler(string apiResult)
        {
            gitAPI git = JsonUtility.FromJson<gitAPI>(apiResult);
            string version = git.tag_name;

            Debug.Log("version: " + version);

        }

        public class gitAPI
        {
            public string name;
            public string tag_name;
            public string assets_url;
            public string html_url;
            public string published_at;
            public string zipball_url;
            public string body;
            public string assets;
        }
    }
}