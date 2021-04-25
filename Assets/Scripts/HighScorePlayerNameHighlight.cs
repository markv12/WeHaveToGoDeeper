using UnityEngine;

public class HighScorePlayerNameHighlight : MonoBehaviour {
    public TMPro.TextMeshProUGUI text;

    void Start() {
        if (text.text == SessionData.playerName)
            text.color = new Color32(246, 148, 94, 255);
    }
}