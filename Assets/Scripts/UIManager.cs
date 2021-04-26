using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public Image healthBar;
    public RectTransform depthBar;
    public RectTransform depthIcon;
    public CanvasGroup canvasGroup;
    public Transform goalTransform;
    private float goalDepth;
    private float depthElementHeight;

    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public GameObject nameBox;
    public Image mainPortrait;
    public Sprite defaultPortrait;
    public Sprite professorPortrait;
    public Sprite playerPortrait;
    public RectTransform dialogueBox;
    public CanvasGroup dialogueGroup;
    public AudioClip typingClip;
    public AudioSource[] typingSounds;
    public float typingVolume = 1;
    private int nextTypeSource = 0;
    private DialogueVertexAnimator utility;

    private void Awake() {
        goalDepth = -goalTransform.position.y;
        depthElementHeight = depthBar.rect.height;
        instance = this;
        DeathPointsLoader.Instance.EnsureDeathPoints();
        utility = new DialogueVertexAnimator(dialogueText, null, PlayFromNextSource);
    }

    private Coroutine healthBarRoutine = null;
    public void ShowHealthAmount(float amount) {
        this.EnsureCoroutineStopped(ref healthBarRoutine);
        float startAmount = healthBar.fillAmount;
        this.CreateAnimationRoutine(0.3f, delegate (float progress) {
            healthBar.fillAmount = Easing.easeInOutSine(startAmount, amount, progress);
        });
    }

    private static readonly WaitForSeconds typeWait = new WaitForSeconds(3f);
    private static readonly string[] separators = new string[] { Environment.NewLine, "\r\n", "\r", "\n" };
    private static readonly char[] nameSeparator = new char[] { ':' };
    private Coroutine showLineRoutine = null;
    private Coroutine showLineRoutineInner = null;
    public void ShowMessage(string message, bool overrideCurrentMessage = false) {
        if (IsDialoguePlaying && !overrideCurrentMessage) return;

        StopTyping();
        dialogueText.text = "";
        dialogueBox.gameObject.SetActive(true);
        this.EnsureCoroutineStopped(ref showLineRoutine);
        this.EnsureCoroutineStopped(ref showLineRoutineInner);
        showLineRoutine = StartCoroutine(ShowLineRoutine(message));

        IEnumerator ShowLineRoutine(string message) {
            float startAlpha = dialogueGroup.alpha;
            showLineRoutineInner = this.CreateAnimationRoutine(0.333f, delegate (float progress) {
                dialogueGroup.alpha = Mathf.Lerp(startAlpha, 1, progress);
            });
            yield return showLineRoutineInner;

            string[] lines = message.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];

                string[] lineParts = line.Split(nameSeparator, 2, StringSplitOptions.RemoveEmptyEntries);
                string theName = "";
                string theText = lineParts[0];
                Color theColor = new Color(195f/255f, 215f / 255f, 223f / 255f);
                if (lineParts.Length >= 2) {
                    theName = lineParts[0];
                    theText = lineParts[1];
                }
      
                Sprite charPortrait;
                if(theName.ToLower() == "player") {
                    theName = SessionData.playerName;
                    charPortrait = playerPortrait;
                    theColor = new Color(255f / 255f, 246f / 255f, 229f / 255f);
                } else {
                    theName = theName.Capitalized();
                    charPortrait = professorPortrait;
                }
                theText = theText.Trim();
                nameBox.SetActive(!string.IsNullOrWhiteSpace(theName));
                nameText.text = theName;
                mainPortrait.sprite = charPortrait;
                dialogueBox.gameObject.GetComponent<Image>().color = theColor;
                bool lineComplete = false;
                Type(theText, delegate {
                    lineComplete = true;
                });
                while (!lineComplete) {
                    yield return null;
                }
                yield return new WaitForSeconds(GetWaitTimeForText(theText));
            }
            mainPortrait.sprite = defaultPortrait;
            nameBox.SetActive(false);

            showLineRoutineInner = this.CreateAnimationRoutine(0.3f, delegate (float progress) {
                dialogueGroup.alpha = 1-progress;
            });
            yield return showLineRoutineInner;
            StopTyping();
            showLineRoutine = null;
            showLineRoutineInner = null;
        }
    }

    private float GetWaitTimeForText(string theText) {
        int charCount = theText.Length;
        float amount = Mathf.InverseLerp(0f, 80f, charCount);
        return 1.0f + Mathf.Lerp(0f, 2.3f, amount);
    }

    public bool IsDialoguePlaying {
        get {
            return showLineRoutine != null;
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

    public void StopTyping() {
        this.EnsureCoroutineStopped(ref typeRoutine);
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
        depthIcon.anchoredPosition = new Vector2(depthIcon.anchoredPosition.x, -1f * depthElementHeight * (depth/goalDepth));
    }
}
