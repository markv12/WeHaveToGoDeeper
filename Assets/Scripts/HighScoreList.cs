using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreList : MonoBehaviour {

    public GameObject highScoreEntry;
    public float spacing = 25.0f;

    void Start() {
        StartCoroutine(ScoresLoader.Instance.LoadBestScoresFromServer(DisplayHighScores));

        void DisplayHighScores(List<HighScore> highScores) {
            float offset = 0.0f;

            highScores.ForEach(delegate (HighScore hs) {
                GameObject entry = Instantiate(highScoreEntry, gameObject.transform);
                entry.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + offset, 0.0f);
                offset -= spacing;

                entry.transform.Find("Player Name").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = hs.name;
                entry.transform.Find("Index").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = (hs.index + 1).ToString();
                entry.transform.Find("Time").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = hs.score.ToString("N2") + "s";
            });
        }
    }
}
