using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player mainPlayer;

    public Transform mainT;
    public Rigidbody2D rgd;
    public Thruster thruster;
    public Collider2D mainCollider;

    public int brakeStrength = 3;
    private const int MAX_HEALTH = 100;
    private int health = MAX_HEALTH;
    private int Health {
        get {
            return health;
        }
        set {
            if(value < health) {
                CameraShaker.instance.HitCameraShake();
            }
            health = value;
            UIManager.instance.ShowHealthAmount(((float)health) / ((float)MAX_HEALTH));
            if(health <= 0) {
                Die();
            }
        }
    }

    void Start() {
        Health = MAX_HEALTH;
        mainPlayer = this;
    }

    void Update() {
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) {
            thruster.ChargeUp(Time.deltaTime * Thruster.THRUST_PER_SECOND);
        }
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) {
            float thrustAmount = thruster.Release();
            //Debug.Log(thrustAmount);
            rgd.AddForce(thruster.ThrustDirection * thrustAmount);
        }
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            rgd.velocity *= 1-(Time.deltaTime * brakeStrength);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Hurt")){
            Health -= 25;
        }
    }

    private void Die() {
        Debug.Log("You Died");
    }
}
