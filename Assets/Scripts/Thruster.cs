using UnityEngine;

public class Thruster : MonoBehaviour {

    public Transform thrusterT;
    public Transform ownerT;
    public Camera theCamera;


    private float thrustAmount = 0;
    private float ThrustAmount {
        get {
            return thrustAmount;
        }
        set {
            thrustAmount = value;
            UIManager.instance.SetChargeBarAmount(thrustAmount);
        }
    }
    public float maxThrust = 1000;
    public const float THRUST_PER_SECOND = 1f;

    public float cameraDistance;
    private void Awake() {
        cameraDistance = -theCamera.transform.position.z;
    }

    void Update() {
        Vector3 v3 = Input.mousePosition;
        v3.z = cameraDistance;
        Vector3 aimPosition = theCamera.ScreenToWorldPoint(v3);

        Vector3 handPos = GetThrusterPosition(aimPosition);
        float angleFromPlayer = GetThrusterAngle(handPos);
        thrusterT.rotation = Quaternion.Euler(0, 0, angleFromPlayer);

        Vector3 GetThrusterPosition(Vector3 aimPosition) {
            Vector3 result;
            Vector3 relPosFromPlayer = aimPosition - ownerT.position;
            relPosFromPlayer.z = 0;
            Vector3 normalizedAngle = relPosFromPlayer.normalized * 5;
            result = ownerT.position + normalizedAngle;

            return new Vector3(result.x, result.y, 0);
        }

        float GetThrusterAngle(Vector3 gunPos) {
            Vector3 relPosFromPlayer = gunPos - ownerT.position;
            return Mathf.Atan2(relPosFromPlayer.y, relPosFromPlayer.x) * 57.2958f;
        }
    }

    public Vector3 ThrustDirection {
        get {
            return -thrusterT.right;
        }
    }

    public void ChargeUp(float chargeAmount) {
        if(ThrustAmount == 0) {
            ThrustAmount = 0.05f;
        }
        ThrustAmount = Mathf.Min(1f, ThrustAmount + chargeAmount);
    }

    public float Release() {
        float result = ThrustAmount;
        ThrustAmount = 0;
        return result * maxThrust;
    }
}
