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

    public float brakeStrength = 3;
    private const int MAX_HEALTH = 100;
    private int health = MAX_HEALTH;
    private int Health {
        get {
            return health;
        }
        set {
            if (value < health) {
                CameraShaker.instance.HitCameraShake();
            }
            health = value;
            UIManager.instance.ShowHealthAmount(((float)health) / ((float)MAX_HEALTH));
            if (health <= 0) {
                Die();
            }
        }
    }

    void Start() {
        Health = MAX_HEALTH;
        mainPlayer = this;
    }

    void Update() {
        if (DeathUIManager.instance.shown) return;

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) {
            thruster.ChargeUp(Time.deltaTime * Thruster.THRUST_PER_SECOND);
            ApplyBrake();
        }
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) {
            float thrustAmount = thruster.Release();
            //Debug.Log(thrustAmount);
            rgd.AddForce(thruster.ThrustDirection * thrustAmount);
        }
        if (Input.GetKeyUp(KeyCode.Backspace)) {
            Die();
        }
        bool brakePressed = Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (brakePressed) {
            ApplyBrake();
        }
    }

    private void ApplyBrake() {
        rgd.velocity *= 1 - (Time.deltaTime * brakeStrength);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Hurt")) {
            Health -= 25;
        } else {
            Health -= 5;
        }
    }

    private void Die() {
        string nameToUse = string.IsNullOrWhiteSpace(SessionData.playerName) ? "Some Poor Soul" : SessionData.playerName;
        DeathPointsLoader.Instance.AddDeathPoint(transform.position.x, transform.position.y, nameToUse);
        DeathUIManager.instance.Show();

        Health = MAX_HEALTH; // for now
    }
}
