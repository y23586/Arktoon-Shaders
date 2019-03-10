using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEditor.Callbacks;
using System.Linq;
using System.Text.RegularExpressions;

namespace ArktoonShaders
{
    public class ArktoonManager : MonoBehaviour
    {
        static string url = "https://api.github.com/repos/synqark/Arktoon-Shaders/releases/latest";
        static UnityWebRequest www;
        static string version = "1.0.2.0";

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

            #if UNITY_2017_OR_NEWER
            www.SendWebRequest();
            #else
            #pragma warning disable 0618
            www.Send();
            #pragma warning restore 0618
            #endif

            EditorApplication.update += EditorUpdate;
        }

        static void EditorUpdate()
        {
            while (!www.isDone) return;

            #if UNITY_2017_OR_NEWER
                if (www.isNetworkError || www.isHttpError) {
                    Debug.Log(www.error);
                } else {
                    UpdateHandler(www.downloadHandler.text);
                }
            #else
                #pragma warning disable 0618
                if (www.isError) {
                    Debug.Log(www.error);
                } else {
                    UpdateHandler(www.downloadHandler.text);
                }
                #pragma warning restore 0618
            #endif

            EditorApplication.update -= EditorUpdate;
        }

        // TODO: 落ち着いたらmigrationにする
        [MenuItem("Arktoon/Clear Shader Keywords")]
        private static void ClearArktoonKeywords()
        {
            var variation = new List<string>() {
                "arktoon/Opaque",
                "arktoon/Fade",
                "arktoon/Cutout",
                "arktoon/FadeRefracted",
                "arktoon/Stencil/Reader/Cutout",
                "arktoon/Stencil/Reader/Double/FadeFade",
                "arktoon/Stencil/Reader/Fade",
                "arktoon/Stencil/Writer/Cutout",
                "arktoon/Stencil/WriterMask/Cutout"
            };
            variation.ForEach(s => ClearKeywordsByShader(s));
        }

        private static void ClearKeywordsByShader(string shaderName) {
            int count = 0;
            string stArea;
            stArea = "Materials using shader " + shaderName+":\n\n";

            List<Material> armat = new List<Material>();

                Renderer[] arrend = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
            foreach (Renderer rend in arrend) {
                foreach (Material mat in rend.sharedMaterials) {
                    if (!armat.Contains (mat)) {
                        armat.Add (mat);
                    }
                }
            }

            foreach (Material mat in armat) {
                if (mat != null && mat.shader != null && mat.shader.name != null && mat.shader.name == shaderName) {
                    stArea += ">"+mat.name + ":" + string.Join(" ", mat.shaderKeywords) + "\n";
                    var keywords = new List<string>(mat.shaderKeywords);
                    keywords.ForEach(keyword => mat.DisableKeyword(keyword));
                    stArea += ">"+mat.name + ":" + string.Join(" ", mat.shaderKeywords) + "\n";
                    count++;
                }
            }

            stArea += "\n"+count + " materials using shader " + shaderName + " found.";
            Debug.Log(stArea);
        }
        static void Migrate()
        {
            /*
            any → 1.0.2
                remove keyword:
                DOUBLE_SIDED
                USE_EMISSION_PARALLLAX
                USE_GLOSS
                USE_OUTLINE_COLOR_SHIFT
                USE_REFLECTION
                USE_REFLECTION_PROBE
                USE_RIM
                USE_POSITION_RELATED_CALC
                USE_SHADE_TEXTURE
                USE_OUTLINE
                USE_CUSTOM_SHADOW_TEXTURE
                USE_CUSTOM_SHADOW_2ND
                USE_CUSTOM_SHADOW_TEXTURE_2ND
                USE_VERTEX_LIGHT
                _LIGHTSAMPLING_ARKTOON
                _LIGHTSAMPLING_CUBED
                _MATCAPBLENDMODE_UNUSED
                _MATCAPBLENDMODE_ADD
                _MATCAPBLENDMODE_LIGHTEN
                _MATCAPBLENDMODE_SCREEN
                _SHADOWCAPBLENDMODE_UNUSED
                _SHADOWCAPBLENDMODE_DARKEN
                _SHADOWCAPBLENDMODE_MULTIPLY
                _SHADOWCAPBLENDMODE_LIGHT_SHUTTER
                _ALPHATEST_ON
                _ALPHABLEND_ON
                _ALPHAPREMULTIPLY_ON

                その他Toggleで定義されていた奴とか、過去につかってたキーワード
            */
        }

        static void UpdateHandler(string apiResult)
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