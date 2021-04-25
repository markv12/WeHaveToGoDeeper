using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishManager : MonoBehaviour {
    public Button restartButton;
    public HighScoreList highScoreList;

    void Awake() {
        restartButton.onClick.AddListener(RestartGame);
        float time = Time.time - SessionData.startTime;

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
