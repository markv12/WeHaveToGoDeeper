using UnityEditor;
namespace KingdomOfNight
{
    public partial class SceneLoader
    {
#if UNITY_EDITOR
        [MenuItem("Scenes/MainScene")]
        public static void LoadMainScene() { OpenScene("Assets/scenes/MainScene.unity"); }
#endif
    }
}