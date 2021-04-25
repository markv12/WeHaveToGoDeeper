using UnityEditor;
public partial class SceneLoader
{
#if UNITY_EDITOR
    [MenuItem("Scenes/FinishScreen")]
    public static void LoadFinishScreen() { OpenScene("Assets/scenes/FinishScreen.unity"); }
    [MenuItem("Scenes/GameScene")]
    public static void LoadGameScene() { OpenScene("Assets/scenes/GameScene.unity"); }
    [MenuItem("Scenes/MainMenu")]
    public static void LoadMainMenu() { OpenScene("Assets/scenes/MainMenu.unity"); }
#endif
}