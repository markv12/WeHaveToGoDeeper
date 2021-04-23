using System.Collections;
using UnityEngine;
public class LoadingScreen : MonoBehaviour {
    public CanvasGroup mainGroup;

    public void Show(IEnumerator load, float fadeTime) {
        gameObject.SetActive(true);
        this.EnsureCoroutineStopped(ref showRoutine);
        showRoutine = StartCoroutine(CO_Show(load, fadeTime));
    }

    private Coroutine showRoutine = null;
    private IEnumerator CO_Show(IEnumerator load, float fadeTime) {
        float elapsedTime = 0;
        float progress = 0;
        float startAlpha = mainGroup.alpha;
        while (progress <= 1) {
            progress = elapsedTime / fadeTime;
            elapsedTime += Time.unscaledDeltaTime;
            mainGroup.alpha = Easing.easeInOutSine(startAlpha, 1, progress);
            yield return null;
        }
        mainGroup.alpha = 1;
        yield return null;

        if (load != null) {
            yield return StartCoroutine(load);
        }
        yield return null;
        Time.timeScale = 1;

        elapsedTime = 0;
        progress = 0;

        while (progress <= 1) {
            progress = elapsedTime / fadeTime;
            float dTime = Time.unscaledDeltaTime;
            if (dTime > 0.1f) {
                dTime = 0.01666f; //Don't jump the fade too far on long frames.
            }
            elapsedTime += dTime;
            mainGroup.alpha = Easing.easeInOutSine(1, 0, progress);
            yield return null;
        }
        mainGroup.alpha = 0;
        gameObject.SetActive(false);
        showRoutine = null;
    }
}
