using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour {

    public static UIManager instance;
    
    public Image chargeBar;
    public Image healthBar;

    private void Awake() {
        instance = this;
        DeathPointsLoader.Instance.EnsureDeathPoints();
    }

    public void SetChargeBarAmount(float amount) {
        chargeBar.fillAmount = Easing.easeInSine(0, 1, amount);
    }

    private Coroutine healthBarRoutine = null;
    public void ShowHealthAmount(float amount) {
        this.EnsureCoroutineStopped(ref healthBarRoutine);
        float startAmount = healthBar.fillAmount;
        this.CreateAnimationRoutine(0.3f, delegate (float progress) {
            healthBar.fillAmount = Easing.easeInOutSine(startAmount, amount, progress);
        });
    }
}
