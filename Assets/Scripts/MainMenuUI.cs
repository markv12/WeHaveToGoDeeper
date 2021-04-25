using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public Button startButton;
    public CanvasGroup startButtonCG;
    public TMPro.TMP_InputField nameInput;
    bool canStart = false;

    private void Awake() {
        startButton.onClick.AddListener(StartGame);
        DeathPointsLoader.Instance.EnsureDeathPoints();
        SessionData.playerName = "Some Poor Soul";

        nameInput.Select();
        nameInput.onValueChanged.AddListener(delegate {
            if (nameInput.text != "") {
                canStart = true;
                startButtonCG.alpha = 1.0f;
            }
            else {
                startButtonCG.alpha = 0.3f;
            }
        });
    }

    void Update() {
        if (Input.GetKey(KeyCode.Return)) {
            StartGame();
        }
    }

    void StartGame() {
        if (!canStart) return;

        SessionData.playerName = nameInput.text;
        SceneLoader.Instance.LoadScene("GameScene");
    }
}
