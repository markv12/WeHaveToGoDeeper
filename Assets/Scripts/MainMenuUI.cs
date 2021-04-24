using UnityEngine;

public class MainMenuUI : MonoBehaviour {
    void Start() {
        DeathPointsLoader.Instance.GetDeathPoints();
    }

    void Update() {

    }
}
