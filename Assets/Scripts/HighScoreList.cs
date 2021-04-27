using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreList : MonoBehaviour {

    public GameObject highScoreEntry;
    private float spacing = 50.0f;
    public CanvasGroup around;
    public Transform aroundTransform;

    public void LoadBestScores() {
        StartCoroutine(ScoresLoader.Instance.LoadBestScoresFromServer(DisplayHighScores));

        void DisplayHighScores(List<HighScore> highScores) {
            float offset = 0.0f;

            highScores.ForEach(delegate (HighScore hs) {
                GameObject entry = Instantiate(highScoreEntry, gameObject.transform);
                entry.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, offset, 0.0f);
                offset -= spacing;

                entry.transform.Find("Player Name").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = hs.name;
                entry.transform.Find("Index").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = (hs.index + 1).ToString();
                entry.transform.Find("Time").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = hs.score.ToString("N2") + "s";
            });
        }
    }

    public void LoadScoresAround(string name) {
        StartCoroutine(ScoresLoader.Instance.LoadNearbyScoresFromServer(name, DisplayNearbyScores));

        void DisplayNearbyScores(List<HighScore> scores) {
            if (scores[(int)Mathf.Round(scores.Count / 2)].index < 5) return;

            around.alpha = 1;
            float offset = 0.0f;

            scores.ForEach(delegate (HighScore hs) {
                GameObject entry = Instantiate(highScoreEntry, aroundTransform);
                entry.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, offset, 0.0f);
                offset -= spacing;

                entry.transform.Find("Player Name").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = hs.name;
                entry.transform.Find("Index").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = (hs.index + 1).ToString();
                entry.transform.Find("Time").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = hs.score.ToString("N2") + "s";
            });
        }
    }
}
