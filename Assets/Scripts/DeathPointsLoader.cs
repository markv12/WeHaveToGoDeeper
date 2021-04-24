using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DeathPointsLoader : Singleton<DeathPointsLoader>
{
  public static DeathPoint[] deathPoints;
  string levelName = "test";

  public void GetDeathPoints () {
    StartCoroutine(LoadDeathPointsFromServer());
  }

  IEnumerator LoadDeathPointsFromServer() {
    Debug.Log("Loading death points");
    string url = "http://ld48-server.herokuapp.com/deaths/get/" + levelName;

    using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
    {
      yield return webRequest.SendWebRequest();

      if (webRequest.result == UnityWebRequest.Result.ConnectionError)
      {
        Debug.Log("Network Error: " + webRequest.error);
      }
      else if (webRequest.downloadHandler.text.Substring(0, 1) == "<")
      {
        Debug.Log("Network Error: 404 " + webRequest.downloadHandler.text);
        yield break;
      }
      else if (webRequest.downloadHandler.text == "Forbidden" || webRequest.downloadHandler.text == "Internal Server Error")
      {
        Debug.Log("Network Error: " + webRequest.downloadHandler.text);
        deathPoints = new DeathPoint[0];
      }
      else
      {
        Debug.Log("Received high scores: " + webRequest.downloadHandler.text);
        //DeathPoints dp = JsonUtility.FromJson<DeathPoints>(webRequest.downloadHandler.text);
        //Debug.Log(dp);

        //string[] highScoreStrings;
        //string[] stringSplitter = new string[] { "\\n" };
        //string highScoreString = webRequest.downloadHandler.text.Substring(1, webRequest.downloadHandler.text.Length - 2);
        //if (webRequest.downloadHandler.text.IndexOf("\\n") == -1)
        //{
        //  Debug.Log("Only one score: " + highScoreString);
        //  highScoreStrings = new string[] { highScoreString };
        //}
        //else
        //{
        //  highScoreStrings = highScoreString.Split(stringSplitter, System.StringSplitOptions.None);
        //}
        //Debug.Log(highScoreStrings);
      }
    }
  }
}

public class DeathPoints
{
    public DeathPoint[] deathPoints;
}

[Serializable]
public class DeathPoint {
  public float x, y;
  public string name;
}