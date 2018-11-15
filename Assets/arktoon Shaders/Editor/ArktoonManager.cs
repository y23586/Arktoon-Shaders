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
        static string version = "0.9.4.0";

        [DidReloadScripts(0)]
        static void CheckVersion ()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode) return;
            Debug.Log ("[Arktoon] Checking local version.");
            string localVersion = EditorUserSettings.GetConfigValue("arktoon_version_local") ?? "";

            if (!localVersion.Equals(version)) {
                // 直前のバージョンと異なるか新規インポートなので、とりあえずReimportを走らせる
                Debug.Log ("[Arktoon] Version change detected : Force reimport.");
                string guidArktoonManager   = AssetDatabase.FindAssets("ArktoonManager t:script")[0];
                string pathToArktoonManager = AssetDatabase.GUIDToAssetPath(guidArktoonManager);
                string pathToShaderDir      = Directory.GetParent(Path.GetDirectoryName(pathToArktoonManager)) + "/Shaders";
                AssetDatabase.ImportAsset(pathToShaderDir, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            }

            // 更新後ローカルバージョンをセット
            EditorUserSettings.SetConfigValue("arktoon_version_local", version);
            Debug.Log ("[Arktoon] Checking remote version.");
            www = UnityWebRequest.Get(url);
            www.Send();
            EditorApplication.update += EditorUpdate;
        }

        static void EditorUpdate()
        {
            while (!www.isDone) return;
            if (www.isError) {
                Debug.Log(www.error);
            } else {
                updateHandler(www.downloadHandler.text);
            }
            EditorApplication.update -= EditorUpdate;
        }

        static void updateHandler(string apiResult)
        {
            gitJson git = JsonUtility.FromJson<gitJson>(apiResult);
            string version = git.tag_name;
            EditorUserSettings.SetConfigValue ("arktoon_version_remote", version);
            Debug.Log("[Arktoon] Remote version : " + version);
        }

        public class gitJson
        {
            public string tag_name;
        }
    }
}