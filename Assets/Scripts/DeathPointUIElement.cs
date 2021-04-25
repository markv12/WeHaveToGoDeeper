using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPointUIElement : MonoBehaviour {
    public CanvasGroup cg;
    public RectTransform rt;

    void Start() {
        this.CreateAnimationRoutine(0.15f, delegate (float progress) {
            float easedProgress = Easing.easeOutSine(0.0f, 1.0f, progress);
            cg.alpha = progress * 1.0f;
            rt.anchoredPosition = rt.anchoredPosition.SetY(easedProgress * -30.0f);
        }, delegate { });
    }

}
