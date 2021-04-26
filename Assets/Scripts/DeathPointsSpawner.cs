using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPointsSpawner : MonoBehaviour {

    public GameObject deathPoint;
    bool okayToSpawn = false;

    public void DisplayDeathPoints() {
        okayToSpawn = true;
        StartCoroutine(spawnAllPointsRoutine(DeathPointsLoader.Instance.deathPoints));

        IEnumerator spawnAllPointsRoutine(List<DeathPoint> deathPoints) {
            if (!okayToSpawn) yield break;
            for (int i = 0; i < deathPoints.Count; i++) {
                DeathPoint dp = deathPoints[i];
                bool didSpawn = SpawnOnePoint(dp);
                if (didSpawn)
                    yield return new WaitForSecondsRealtime(0.035f);
            }
        }

        bool SpawnOnePoint(DeathPoint dp) {
            Vector2 viewportPoint = CameraManager.mainCamera.WorldToViewportPoint(new Vector3(dp.x, dp.y, 0));

            if (
                viewportPoint.x < 0 ||
                viewportPoint.y < 0 ||
                viewportPoint.x > 1 ||
                viewportPoint.y > 1
            )
                return false;

            GameObject point = Instantiate(deathPoint, gameObject.transform);
            Vector2 screenPoint = CameraManager.mainCamera.WorldToScreenPoint(new Vector3(dp.x, dp.y, 0));
            point.transform.position = new Vector3(
                screenPoint.x,
                screenPoint.y,
                0.0f);

            point.transform.Find("Positioner").Find("Name").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = dp.name;
            return true;
        }
    }

    public void ClearDeathPoints() {
        okayToSpawn = false;

        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
