using System;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour {

    public static UIManager instance;
    
    public Image chargeBar;
    public Image healthBar;

    private void Awake() {
        instance = this;
    }

    public void SetChargeBarAmount(float amount) {
        chargeBar.fillAmount = amount;
    }

    public void ShowHealthAmount(float amount) {
        healthBar.fillAmount = amount;
    }
}
