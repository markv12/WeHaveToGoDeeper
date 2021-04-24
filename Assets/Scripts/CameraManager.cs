using UnityEngine;
public class CameraManager : MonoBehaviour {

    public static Camera mainCamera;

    public Transform mainT;
    public Player target;
    public Camera mainCam;

    private float cameraZ;

    private void Awake() {
        mainCamera = mainCam;
        cameraZ = mainT.position.z;
    }

    private const float FOLLOW_SPEED = 7.5f;
    private void FixedUpdate() {
        mainT.position = Vector3.Lerp(mainT.position, target.rgd.position, FOLLOW_SPEED * Time.fixedDeltaTime).SetZ(cameraZ);
    }
}
