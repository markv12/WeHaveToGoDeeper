using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathPointsLoader : Singleton<DeathPointsLoader> {
    public List<DeathPoint> deathPoints;

    readonly string levelName = "test";
    bool hasLoaded = false;

    public void AddDeathPoint(float x, float y, string name) {
        StartCoroutine(AddDeathPointToServer(x, y, name));
    }

    public void EnsureDeathPoints() {
        if (!hasLoaded)
            StartCoroutine(LoadDeathPointsFromServer());
    }

    IEnumerator LoadDeathPointsFromServer() {
        hasLoaded = true;
        //Debug.Log("Loading death points for level " + levelName);
        string url = "http://ld48-server.herokuapp.com/deaths/get/" + levelName;

        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        bool success = WebRequestErrorHandler(webRequest);
        if (!success) yield break;

        string highScoreString = "{\"deathPoints\":" + webRequest.downloadHandler.text + "}";
        DeathPoints dp = JsonUtility.FromJson<DeathPoints>(highScoreString);
        deathPoints = dp.list();

        //Debug.Log("Received " + deathPoints.Count + " death points");
    }

    IEnumerator AddDeathPointToServer(float x, float y, string name) {
        //Debug.Log("Adding death point for level " + levelName);

        string url = "http://ld48-server.herokuapp.com/deaths/add/" + levelName + "/" + x.ToString() + "/" + y.ToString() + "/" + name;
        //Debug.Log(url);

        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        bool success = WebRequestErrorHandler(webRequest);
        if (!success) yield break;

        deathPoints.Add(new DeathPoint(x, y, name));

        //Debug.Log("Added new death point");
        //deathPoints.ForEach(delegate (DeathPoint p) {
        //  p.print();
        //});
    }

    bool WebRequestErrorHandler(UnityWebRequest webRequest) {
        if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log("Network Error: " + webRequest.error);
            return false;
        }

        else if (webRequest.downloadHandler.text.Substring(0, 1) == "<") {
            Debug.Log("Network Error: 404");
            return false;
        }

        else if (webRequest.downloadHandler.text == "Forbidden"
          || webRequest.downloadHandler.text == "Internal Server Error") {
            Debug.Log("Network Error: " + webRequest.downloadHandler.text);
            return false;
        }
        return true;
    }
}

[Serializable]
public struct DeathPoints {
    public DeathPoint[] deathPoints;

    public List<DeathPoint> list() {
        return new List<DeathPoint>(deathPoints);
    }
}

[Serializable]
public class DeathPoint {
    public float x, y;
    public string name;

    public DeathPoint(float _x, float _y, string _name) {
        x = _x;
        y = _y;
        name = _name;
    }

    public void print() {
        Debug.Log(x.ToString() + " " + y.ToString() + " " + name);
    }
}