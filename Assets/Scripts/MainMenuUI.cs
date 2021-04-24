using UnityEngine;

public class MainMenuUI : MonoBehaviour {
    void Start() {
        DeathPointsLoader.Instance.EnsureDeathPoints();
        SessionData.playerName = "A Player";
    }
}
