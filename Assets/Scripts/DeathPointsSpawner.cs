using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPointSpawner : MonoBehaviour {

    public GameObject deathPoint;

    public void DisplayDeathPoints() {
        DeathPointsLoader.Instance.deathPoints.ForEach(delegate (DeathPoint dp) {
            GameObject point = Instantiate(deathPoint, gameObject.transform);

            point.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0.0f);

            point.transform.Find("Name").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = dp.name;
        });
    }

    public void ClearDeathPoints() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
