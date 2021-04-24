using UnityEngine;

public class HandController : MonoBehaviour {

    public Transform ownerT;

    public bool belongsToPlayer;
    public float heightAdjustment = 1f;
    public float armLength = 3.5f;

    public Transform shootPoint;
    public Transform handMainTransform;
    public Transform imageT;

    private static Camera theCamera;
    public static Camera TheCamera {
        get {
            if (theCamera == null) {
                theCamera = CameraShaker.instance.shakeTransform.GetComponent<Camera>();
            }
            return theCamera;
        }
    }
    public static float cameraDistance;

    private void Start() {
        if (belongsToPlayer) {
            cameraDistance = -TheCamera.transform.position.z;
        }
    }

    private static readonly Quaternion theIQ = Quaternion.identity;
    void Update() {
        if (Time.timeScale > 0) {
            Vector3 aimPosition;
            if (belongsToPlayer) {
                Vector3 v3 = Input.mousePosition;
                v3.z = cameraDistance;
                aimPosition = TheCamera.ScreenToWorldPoint(v3);
            } else {
                aimPosition = OldPlayer.mainPlayer.t.position;
            }

            Vector3 handPos = GetHandPosition(aimPosition);
            float angleFromPlayer = GetHandAngle(handPos);
            AdjustImageForAngle(angleFromPlayer);
            handMainTransform.rotation = Quaternion.Euler(0, 0, angleFromPlayer);
            handMainTransform.position = handPos;
            if (belongsToPlayer && Input.GetMouseButtonDown(0)) {

            }
            imageT.localRotation = Quaternion.Lerp(imageT.localRotation, theIQ, Time.deltaTime * 3);
        }
    }

    private DirectionState dState = DirectionState.initial;
    private void AdjustImageForAngle(float angleFromPlayer) {
        if (angleFromPlayer > -90 && angleFromPlayer < 90) {
            if (dState != DirectionState.right) {
                imageT.localScale = new Vector3(-1, 1, 1);
                imageT.localPosition = new Vector3(0, -heightAdjustment, 0);
                imageT.localRotation = theIQ;
                dState = DirectionState.right;
            }
        } else {
            if (dState != DirectionState.left) {
                imageT.localScale = new Vector3(-1, -1, 1);
                imageT.localPosition = new Vector3(0, heightAdjustment, 0);
                imageT.localRotation = theIQ;
                dState = DirectionState.left;
            }
        }
    }

    private float GetHandAngle(Vector3 gunPos) {
        Vector3 relPosFromPlayer = gunPos - ownerT.position;
        return Mathf.Atan2(relPosFromPlayer.y, relPosFromPlayer.x) * 57.2958f;
    }

    private Vector3 GetHandPosition(Vector3 aimPosition) {
        Vector3 result;
        Vector3 relPosFromPlayer = aimPosition - ownerT.position;
        relPosFromPlayer.z = 0;
        if (relPosFromPlayer.magnitude > armLength) {
            Vector3 normalizedAngle = relPosFromPlayer.normalized * armLength;
            result = ownerT.position + normalizedAngle;
        } else {
            result = aimPosition;
        }
        return new Vector3(result.x, result.y, 0);
    }
}

public enum DirectionState {
    initial,
    left,
    right
}
