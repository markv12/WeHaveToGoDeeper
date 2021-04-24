using UnityEngine;

public class CircleFish : MonoBehaviour {

    public Transform centerT;

    public float speed = 50;

    private void Update() {
        centerT.Rotate(0, 0, speed * Time.deltaTime);
    }
}
