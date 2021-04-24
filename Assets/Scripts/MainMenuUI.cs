using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public Button startButton;

    private void Awake() {
        startButton.onClick.AddListener(StartGame);
        DeathPointsLoader.Instance.EnsureDeathPoints();
        SessionData.playerName = "A Player";

        void StartGame() {
            SceneLoader.Instance.LoadScene("GameScene");
        }
    }
}
