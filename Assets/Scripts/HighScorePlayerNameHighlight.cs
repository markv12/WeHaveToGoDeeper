using UnityEngine;

public class HighScorePlayerNameHighlight : MonoBehaviour {
    public TMPro.TextMeshProUGUI text;
    public GameObject panel;

    void Start() {
        if (text.text == SessionData.playerName)
            panel.SetActive(true);
    }
}