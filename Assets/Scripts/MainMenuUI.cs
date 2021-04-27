using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public Button startButton;
    public CanvasGroup startButtonCG;
    public GameObject errorText;
    public TMPro.TMP_InputField nameInput;
    public HighScoreList hsl;

    public GameObject fullScreenToggle;
    public Button fullScreenButton;
    public Image fullScreenImage;
    public Sprite disabledSprite;
    public Sprite enabledSprite;

    bool canStart = false;

    private void Awake() {

#if UNITY_WEBGL
        fullScreenToggle.SetActive(false);
#endif
        fullScreenButton.onClick.AddListener(ToggleFullScreenSetting);
        fullScreenImage.sprite = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? enabledSprite : disabledSprite;

        startButton.onClick.AddListener(StartGame);
        DeathPointsLoader.Instance.EnsureDeathPoints();
        hsl.LoadBestScores();

        nameInput.Select();
        nameInput.onValueChanged.AddListener(delegate {
            if (nameInput.text != "") {
                canStart = true;
                startButtonCG.alpha = 1.0f;
            }
            else {
                canStart = false;
                startButtonCG.alpha = 0.3f;
            }
        });
        AudioManager preload = AudioManager.Instance;
    }

    void Update() {
        if (Input.GetKey(KeyCode.Return)) {
            StartGame();
        }
    }

    void StartGame() {
        if (!canStart) {
            errorText.SetActive(true);
            return;
        }

        SessionData.playerName = nameInput.text.Replace('?', '¿').Replace('/', '-').Replace('&', '+').ToLower();
        SceneLoader.Instance.LoadScene("GameScene");
    }

    private void ToggleFullScreenSetting() {
        bool isFullscreen = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen || Screen.fullScreenMode == FullScreenMode.FullScreenWindow || Screen.fullScreenMode == FullScreenMode.MaximizedWindow;
        isFullscreen = !isFullscreen;
        if (isFullscreen) {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        fullScreenImage.sprite = isFullscreen ? enabledSprite : disabledSprite;
    }
}
