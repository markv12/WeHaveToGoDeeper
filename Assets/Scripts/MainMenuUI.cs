using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public Button startButton;

    private void Awake() {
        startButton.onClick.AddListener(delegate { StartGame(); });
    }

    private void StartGame() {
        SceneLoader.Instance.LoadScene("GameScene");
    }

    void Start() {
        DeathPointsLoader.Instance.EnsureDeathPoints();
        SessionData.playerName = "A Player";
    }
}
