using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public static class MonobehaviourExtensions
{
    public static void EnsureCoroutineStopped(this MonoBehaviour value, ref Coroutine routine)
    {
        if (routine != null)
        {
            value.StopCoroutine(routine);
            routine = null;
        }
    }

    public static Coroutine CreateAnimationRoutine(this MonoBehaviour value, float duration, Action<float> changeFunction, Action onComplete = null) {
        return value.StartCoroutine(GenericAnimationRoutine(duration, changeFunction, onComplete));
    }

    private static IEnumerator GenericAnimationRoutine(float duration, Action<float> changeFunction, Action onComplete) {
        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1) {
            changeFunction(progress);
            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / duration;
            yield return null;
        }
        changeFunction(1);
        onComplete?.Invoke();
    }

    public static T EnsureScriptableObjectPresent<T>(T scriptableObject, string scriptName, string controllerName) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError("No " + scriptName + " assigned to " + controllerName + ", using default.");
            return ScriptableObject.CreateInstance<T>();
        }
        return scriptableObject;
    }

    public static T EnsureComponentPresent<T>(this MonoBehaviour value, T component, string componentName) where T : Component
    {
        if (component == null)
        {
            component = value.GetComponent<T>();
            Assert.IsNotNull(component, "No " + componentName + " component found on Enemy.");
        }
        return component;
    }
}
