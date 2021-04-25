using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StraightFish : MonoBehaviour {

    public Transform fishT;

    public Vector2 endPosition = new Vector2(1, 0);
    public float speed = 5;
    private Vector2 startPosition;
    private Vector3 startScale;

    private void Awake() {
        startPosition = fishT.localPosition;
        startScale = fishT.localScale;
    }

    private static readonly WaitForSeconds pauseWait = new WaitForSeconds(0.2f);
    private void OnEnable() {
        StartCoroutine(MoveRoutine());

        IEnumerator MoveRoutine() {
            Vector2 posDiff = endPosition - startPosition;
            float distance = posDiff.magnitude;
            float duration = distance / speed;
            while (true) {
                SetFishAngle(startPosition, endPosition);
                yield return this.CreateAnimationRoutine(duration, delegate (float progress) {
                    float easedProgress = Easing.easeInOutSine(0, 1, progress);
                    fishT.localPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);
                });
                yield return pauseWait;
                SetFishAngle(endPosition, startPosition);
                yield return this.CreateAnimationRoutine(duration, delegate (float progress) {
                    float easedProgress = Easing.easeInOutSine(0, 1, progress);
                    fishT.localPosition = Vector2.Lerp(endPosition, startPosition, easedProgress);
                });
                yield return pauseWait;
            }
        }
    }

    private void SetFishAngle(Vector2 start, Vector2 end) {
        float angle = (float)GetAngle(start, end);
        bool flip = angle < 0;
        if(startPosition.y > endPosition.y) {
            flip = !flip;
        }
        fishT.localScale = flip ? startScale.SetY(-startScale.y) : startScale;
        fishT.localEulerAngles = new Vector3(0, 0, angle);
    }

    public static double GetAngle(Vector2 me, Vector2 target) {
        return System.Math.Atan2(target.y - me.y, target.x - me.x) * (180 / System.Math.PI);
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(StraightFish))]
public class StraightFishEditor : Editor {
    public void OnSceneGUI() {
        StraightFish myTarget = (StraightFish)target;

        myTarget.endPosition = Handles.FreeMoveHandle(
            myTarget.transform.position + (Vector3)myTarget.endPosition,
            Quaternion.identity,
            0.08f * HandleUtility.GetHandleSize(myTarget.transform.position),
            Vector3.zero,
            Handles.DotHandleCap
        ) - myTarget.transform.position;

        Handles.DrawLine(myTarget.transform.position + myTarget.fishT.localPosition, myTarget.transform.position + (Vector3)myTarget.endPosition);
        if (GUI.changed) {
            Undo.RecordObject(target, "Updated Sorting Offset");
            EditorUtility.SetDirty(target);
        }
    }

    public override void OnInspectorGUI() {
        StraightFish myTarget = (StraightFish)target;
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.LabelField("Length: " + Vector2.Distance(myTarget.fishT.localPosition, myTarget.endPosition));
    }
}
#endif

