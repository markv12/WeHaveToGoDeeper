using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathUIManager : MonoBehaviour {

    public static DeathUIManager instance;

    public DeathPointsSpawner pointsSpawner;
    public Canvas canvas;
    public CanvasGroup cg;
    public bool shown = false;

    void Awake() {
        instance = this;
    }

    void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            Hide();
        }
    }

    private Coroutine fadeRoutine;

    public void Show() {
        Time.timeScale = 0.0f;
        canvas.enabled = true;
        shown = true;
        pointsSpawner.DisplayDeathPoints();

        this.EnsureCoroutineStopped(ref fadeRoutine);
        fadeRoutine = this.CreateAnimationRoutine(0.3f, delegate (float progress) {
            cg.alpha = progress;
        }, delegate {});
    }

    public void Hide() {
        this.EnsureCoroutineStopped(ref fadeRoutine);
        Time.timeScale = 1.0f;
        canvas.enabled = false;
        shown = false;
        pointsSpawner.ClearDeathPoints();
    }
}