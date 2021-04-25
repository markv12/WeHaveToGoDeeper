using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    [TextArea]
    public string testMessage;

    public Image chargeBar;
    public Image healthBar;
    public Image depthBar;
    public CanvasGroup canvasGroup;
    public Transform goalTransform;
    private float goalDepth;

    public TMP_Text dialogueText;
    public RectTransform dialogueBox;
    public AudioClip typingClip;
    public AudioSource[] typingSounds;
    public float typingVolume = 1;
    private int nextTypeSource = 0;
    private DialogueVertexAnimator utility;

    private void Awake() {
        goalDepth = -goalTransform.position.y;
        instance = this;
        DeathPointsLoader.Instance.EnsureDeathPoints();
        utility = new DialogueVertexAnimator(dialogueText, null, PlayFromNextSource);
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

    private Coroutine closeRoutine = null;
    public void ShowMessage(string message) {
        dialogueBox.gameObject.SetActive(true);
        Type(message, delegate {
            this.EnsureCoroutineStopped(ref closeRoutine);
            closeRoutine = StartCoroutine(WaitAndCloseBox());
        });

        IEnumerator WaitAndCloseBox() {
            yield return new WaitForSeconds(5f);
            dialogueBox.gameObject.SetActive(false);
        }
    }

    private Coroutine typeRoutine = null;
    private const float MIN_HEIGHT = 80;
    private bool Type(string message, Action onFinish) {
        this.EnsureCoroutineStopped(ref typeRoutine);
        utility.textAnimating = false;
        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(message, out string totalTextMessage);
        SetTextBoxHeight(totalTextMessage);
        bool hasText = totalTextMessage.Length > 0;
        if (hasText) {
            typeRoutine = StartCoroutine(utility.AnimateTextIn(commands, totalTextMessage, typingClip, onFinish));
        } else {
            utility.HandleNonTextCommandsOnly(commands);
        }
        return hasText;
    }

    private void SetTextBoxHeight(string totalTextMessage) {
        dialogueText.text = totalTextMessage;
        float height = Mathf.Max(MIN_HEIGHT, dialogueText.preferredHeight + 32);
        dialogueBox.sizeDelta = new Vector2(dialogueBox.sizeDelta.x, height);
        dialogueText.text = "";
    }

    public void PlayFromNextSource(AudioClip clip) {
        AudioSource nextSource = typingSounds[nextTypeSource];
        nextSource.clip = clip;
        nextSource.volume = typingVolume;
        nextSource.Play();
        nextTypeSource = (nextTypeSource + 1) % typingSounds.Length;
    }

    public void ShowDepth(float depth) {
        depthBar.fillAmount = Mathf.Min(1, Mathf.Max(0, depth / goalDepth));
    }
}
