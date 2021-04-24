using UnityEditor;
public partial class SceneLoader
{
#if UNITY_EDITOR
    [MenuItem("Scenes/GameScene")]
    public static void LoadGameScene() { OpenScene("Assets/scenes/GameScene.unity"); }
    [MenuItem("Scenes/MainMenu")]
    public static void LoadMainMenu() { OpenScene("Assets/scenes/MainMenu.unity"); }
#endif
}