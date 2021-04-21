using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    public Camera mainCamera;
    public float currentZoom = -9;
    private const float MAX_ZOOM = -9;

    private const float ZOOM_THRESHOLD = 0.92f;

    private Transform zoomTarget;
    private void Awake() {
        zoomTarget = FindObjectOfType<Player>().transform;
    }

    private static readonly Vector3 adjustmentVector = new Vector3(-0.5f, -0.5f, 0);
    private void LateUpdate() {
        Vector3 viewportPoint = (mainCamera.WorldToViewportPoint(zoomTarget.position) + adjustmentVector) * 2f;
        float maxViewportPos = Mathf.Max(Mathf.Abs(viewportPoint.x), Mathf.Abs(viewportPoint.y));
        if (maxViewportPos > ZOOM_THRESHOLD) {
            float maxDiff = (maxViewportPos - ZOOM_THRESHOLD) * 20;
            float speed = Mathf.Lerp(0, 5 , maxDiff);
            currentZoom -= Time.deltaTime* speed;
        } else if (currentZoom < MAX_ZOOM){
            currentZoom += Time.deltaTime * 4f;
        }
    }
}
