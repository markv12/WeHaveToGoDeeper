using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishManager : MonoBehaviour {
    public Button restartButton;
    public HighScoreList highScoreList;
    public TMPro.TextMeshProUGUI timeLabel;

    void Awake() {
        restartButton.onClick.AddListener(RestartGame);
        float time = Time.time - SessionData.startTime;

        timeLabel.text = time.ToString("N2") + "s";

        if (time < 0.1f) return;

        StartCoroutine(
            ScoresLoader.Instance.AddHighScoreToServer(
                time, SessionData.playerName, delegate {
                    highScoreList.LoadScoresAround(SessionData.playerName);
                }
            )
        );
    }

    void RestartGame() {
        SceneLoader.Instance.LoadScene("GameScene");
    }
}
