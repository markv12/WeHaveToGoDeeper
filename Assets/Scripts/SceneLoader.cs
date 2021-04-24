using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;
using System.IO;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoader : Singleton<SceneLoader> {
    private const string PATH_TO_SCENES_FOLDER = "/scenes/";
    private const string PATH_TO_OUTPUT_SCRIPT_FILE = "/Scripts/SceneLoaderDropdowns.cs";

    private LoadingScreen m_loadScreen = null;
    private LoadingScreen LoadScreen {
        get {
            if (m_loadScreen == null) {
                GameObject loadingScreenObject = Instantiate(Resources.Load(LOADING_SCREEN_PATH)) as GameObject;
                DontDestroyOnLoad(loadingScreenObject);
                m_loadScreen = loadingScreenObject.GetComponent<LoadingScreen>();
            }
            return m_loadScreen;
        }
    }

    public const float FADE_TIME = 0.666f;
    private const string LOADING_SCREEN_PATH = "LoadingScreen";
    public void ShowLoadingScreen(IEnumerator load, float _fadeTime = FADE_TIME) {
        LoadScreen.Show(load, _fadeTime);
    }

    public void LoadScene(string sceneName, float _fadeTime = FADE_TIME, Action onComplete = null) {
        float musicFadeTime = _fadeTime * 0.9f;
        ShowLoadingScreen(Co_PreloadThenLoad(sceneName, onComplete), _fadeTime);
    }

    private IEnumerator Co_PreloadThenLoad(string sceneName, Action onComplete) {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return null;
        onComplete?.Invoke();
    }

#if UNITY_EDITOR

    [MenuItem("Tools/Generate Scene Load Menu Code")]
    public static void GenerateSceneLoadMenuCode() {
        StringBuilder result = new StringBuilder();
        string basePath = Application.dataPath + PATH_TO_SCENES_FOLDER;
        AddClassHeader(result);
        AddCodeForDirectory(new DirectoryInfo(basePath), result);
        AddClassFooter(result);

        string scriptPath = Application.dataPath + PATH_TO_OUTPUT_SCRIPT_FILE;
        File.WriteAllText(scriptPath, result.ToString());

        void AddCodeForDirectory(DirectoryInfo directoryInfo, StringBuilder result) {
            FileInfo[] fileInfoList = directoryInfo.GetFiles();
            for (int i = 0; i < fileInfoList.Length; i++) {
                FileInfo fileInfo = fileInfoList[i];
                if (fileInfo.Extension == ".unity") {
                    AddCodeForFile(fileInfo, result);
                }
            }
            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
            for (int i = 0; i < subDirectories.Length; i++) {
                AddCodeForDirectory(subDirectories[i], result);
            }

            void AddCodeForFile(FileInfo fileInfo, StringBuilder result) {
                string subPath = fileInfo.FullName.Replace('\\', '/').Replace(basePath, "");
                string assetPath = ASSETS_SCENE_PATH + subPath;

                string functionName = fileInfo.Name.Replace(".unity", "").Replace(" ", "").Replace("-", "");

                result.Append("    [MenuItem(\"Scenes/").Append(subPath.Replace(".unity", "")).Append("\")]").Append(Environment.NewLine);
                result.Append("    public static void Load").Append(functionName).Append("() { OpenScene(\"").Append(assetPath).Append("\"); }").Append(Environment.NewLine); ;
            }
        }
    }

    private static void AddClassHeader(StringBuilder result) {
        result.Append(@"using UnityEditor;
public partial class SceneLoader
{
");
        result.Append(@"#if UNITY_EDITOR
");
    }

    private static void AddClassFooter(StringBuilder result) {
        result.Append(@"#endif
}");
    }

    private const string ASSETS_SCENE_PATH = "Assets/scenes/";
    private static void OpenScene(string scenePath) {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
    }
#endif
}
