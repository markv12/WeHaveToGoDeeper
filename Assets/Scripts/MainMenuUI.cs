using UnityEngine;

public class MainMenuUI : MonoBehaviour {
    void Start() {
        DeathPointsLoader.Instance.EnsureDeathPoints();
    }

    void Update() {

    }
}
