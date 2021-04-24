using UnityEngine;
public class CameraManager : MonoBehaviour {

    public Transform mainT;
    public Player target;

    private float cameraZ;

    private void Awake() {
        cameraZ = mainT.position.z;
    }

    private const float FOLLOW_SPEED = 10f;
    private void FixedUpdate() {
        mainT.position = Vector3.Lerp(mainT.position, target.rgd.position, FOLLOW_SPEED * Time.fixedDeltaTime).SetZ(cameraZ);
    }
}
