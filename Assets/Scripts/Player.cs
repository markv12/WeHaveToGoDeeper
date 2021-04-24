using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player mainPlayer;

    public Transform mainT;
    public Rigidbody2D rgd;
    public Thruster thruster;
    public Collider2D mainCollider;

    private const int MAX_HEALTH = 100;
    private int health = MAX_HEALTH;
    private int Health {
        get {
            return health;
        }
        set {
            health = value;
            UIManager.instance.ShowHealthAmount(((float)health) / ((float)MAX_HEALTH));
        }
    }


    void Awake() {
        Health = MAX_HEALTH;
        mainPlayer = this;
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            thruster.ChargeUp(Time.deltaTime * Thruster.THRUST_PER_SECOND);
        }
        if (Input.GetMouseButtonUp(0)) {
            float thrustAmount = thruster.Release();
            Debug.Log(thrustAmount);
            rgd.AddForce(thruster.ThrustDirection * thrustAmount);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Hurt")){
            Health -= 10;
        }
    }
}
