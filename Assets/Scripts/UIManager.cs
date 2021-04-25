using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public Image healthBar;
    public Image depthBar;
    public CanvasGroup canvasGroup;
    public Transform goalTransform;
    private float goalDepth;

    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public GameObject nameBox;
    public RectTransform dialogueBox;
    public CanvasGroup dialogueGroup;
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
    private static readonly string[] nameSeparator = new string[] { ":" };
    private Coroutine showLineRoutine = null;
    private Coroutine showLineRoutineInner = null;
    public void ShowMessage(string message) {
        dialogueText.text = "";
        dialogueGroup.alpha = 0;
        dialogueBox.gameObject.SetActive(true);
        this.EnsureCoroutineStopped(ref showLineRoutine);
        this.EnsureCoroutineStopped(ref showLineRoutineInner);
        showLineRoutine = StartCoroutine(ShowLineRoutine(message));

        IEnumerator ShowLineRoutine(string message) {
            showLineRoutineInner = this.CreateAnimationRoutine(0.3f, delegate (float progress) {
                dialogueGroup.alpha = progress;
            });
            yield return showLineRoutineInner;

            string[] lines = message.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];

                string[] lineParts = line.Split(nameSeparator, StringSplitOptions.RemoveEmptyEntries);
                string theName = lineParts.Length >= 2 ? lineParts[0] : "";
                if(theName.ToLower() == "player") {
                    theName = SessionData.playerName;
                }
                nameBox.SetActive(!string.IsNullOrWhiteSpace(theName));
                nameText.text = theName;
                string theText = lineParts.Length >= 2 ? lineParts[1] : lineParts[0];
                bool lineComplete = false;
                Type(theText, delegate {
                    lineComplete = true;
                });
                while (!lineComplete) {
                    yield return null;
                }
                yield return typeWait;
            }

            showLineRoutineInner = this.CreateAnimationRoutine(0.3f, delegate (float progress) {
                dialogueGroup.alpha = 1-progress;
            });
            yield return showLineRoutineInner;
            showLineRoutine = null;
            showLineRoutineInner = null;
        }
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
