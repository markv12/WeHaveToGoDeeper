using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager> {

    private static Dictionary<string, Object> resources = new Dictionary<string, Object>();

    public static Object LoadResource(string path) {
        return LoadResource<Object>(path);
    }

    public static T LoadResource<T>(string path) where T : Object {
        return LoadResource<T>(path, false);
    }

    public static T LoadResource<T>(string path, bool cacheNullResources) where T : Object {
        Object obj = null;
        if (resources.TryGetValue(path, out obj)) {
            if (obj == null && !cacheNullResources) {
                T resource = Resources.Load<T>(path);
                resources[path] = resource;
                return resource;
            }
        } else {
            T resource = Resources.Load<T>(path);
            resources.Add(path, resource);
            return resource;
        }
        return obj as T;
    }

    public static GameObject InstantiatePrefab(string path, Vector3 position) {
        return InstantiatePrefab(path, position, Quaternion.identity);
    }

    public static GameObject InstantiatePrefab(string path, Vector3 position, Quaternion rotation) {
        var resource = LoadResource<GameObject>(path);
        if (resource != null) {
            return Instantiate(resource, position, rotation);
        }
        return null;
    }

    public static T InstantiatePrefab<T>(string path, Vector3 position)
    where T : Component {
        return InstantiatePrefab<T>(path, position, Quaternion.identity);
    }

    public static T InstantiatePrefab<T>(string path, Vector3 postion, Quaternion rotation)
    where T : Component {
        GameObject gameObject = InstantiatePrefab(path, postion, rotation);
        if (gameObject != null) {
            T component = gameObject.GetComponent<T>();
            if (component != null) {
                return component;
            } else {
                Destroy(gameObject);
            }
        }
        return null;
    }

    public static T InstantiatePrefab<T>(GameObject prefab, Vector3 position)
    where T : Component {
        return InstantiatePrefab<T>(prefab, position, Quaternion.identity);
    }

    public static T InstantiatePrefab<T>(GameObject prefab, Vector3 postion, Quaternion rotation)
    where T : Component {
        var gameObject = Instantiate(prefab, postion, rotation);
        if (gameObject != null) {
            var component = gameObject.GetComponent<T>();
            if (component != null) {
                return component;
            } else {
                Destroy(gameObject);
            }
        }
        return null;
    }

    private static readonly System.Action<Object> nothing = delegate (Object pogObj) { };
    public void LoadResourceAsync(string path) {
        StartCoroutine(_LoadResourceAsync(path, nothing));
    }
    public void LoadResourceAsync(string path, System.Action<Object> action) {
        StartCoroutine(_LoadResourceAsync(path, action));
    }

    private static Dictionary<string, List<System.Action<Object>>> actionsForPath = new Dictionary<string, List<System.Action<Object>>>();
    private static IEnumerator _LoadResourceAsync(string path, System.Action<Object> action) {
        if (resources.TryGetValue(path, out Object obj)) {
            action(obj);
        } else {
            if (actionsForPath.TryGetValue(path, out List<System.Action<Object>> queuedActions)) {
                queuedActions.Add(action);
            } else {
                actionsForPath.Add(path, new List<System.Action<Object>> { action });
                ResourceRequest request = Resources.LoadAsync(path);
                if (request.asset == null)
                    Debug.LogError("Couldn't find: " + path);
                while (!request.isDone) {
                    yield return null;
                }
                resources.Add(path, request.asset);
                queuedActions = actionsForPath[path];
                for (int i = 0; i < queuedActions.Count; i++) {
                    queuedActions[i](request.asset);
                }
                actionsForPath.Remove(path);
            }
        }
    }
}
