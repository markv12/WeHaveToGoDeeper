using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScoresLoader : Singleton<ScoresLoader> {
    readonly string levelName = "test";

    public void AddHighScore(float score, string name) {
        StartCoroutine(AddHighScoreToServer(score, name));
    }

    public IEnumerator LoadBestScoresFromServer(Action<List<HighScore>> callback) {
        string url = "https://ld48-server.herokuapp.com/score/best/" + levelName + "/bottom/5";

        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        bool success = WebRequestErrorHandler(webRequest);
        if (!success) yield break;

        string highScoreString = "{\"highScores\":" + webRequest.downloadHandler.text + "}";
        HighScores hs = JsonUtility.FromJson<HighScores>(highScoreString);

        callback?.Invoke(hs.list());

        //Debug.Log("Received " + hs.list().Count + " high scores");
    }

    IEnumerator AddHighScoreToServer(float score, string name) {
        Debug.Log("Adding score for user " + name);

        string url = "https://ld48-server.herokuapp.com/score/add/" + levelName + "/" + name + "/" + score.ToString();

        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        bool success = WebRequestErrorHandler(webRequest);
        if (!success) yield break;

        Debug.Log("Added new score");
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
public struct HighScores {
    public HighScore[] highScores;

    public List<HighScore> list() {
        return new List<HighScore>(highScores);
    }
}

[Serializable]
public class HighScore {
    public float score;
    public string name;
    public int index;

    public HighScore(float _score, string _name, int _index) {
        score = _score;
        name = _name;
        index = _index;
    }

    public void print() {
        Debug.Log(index.ToString() + ": " + score.ToString() + " " + name);
    }
}