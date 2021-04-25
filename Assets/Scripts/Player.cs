using UnityEngine;

public class Player : MonoBehaviour {

    public static Player mainPlayer;

    public Transform mainT;
    public Rigidbody2D rgd;
    public Thruster thruster;
    public Collider2D mainCollider;
    public AudioSource engineSound;

    public ParticleSystem thrustParticles;

    public float brakeStrength = 3;
    private const int MAX_HEALTH = 100;
    private int health = MAX_HEALTH;
    private int Health {
        get {
            return health;
        }
        set {
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
        SessionData.startTime = Time.time;
    }

    private const float SEA_LEVEL_Y = 10f;
    void Update() {
        if (DeathUIManager.instance.shown) return;

        engineSound.pitch = 1.0f + rgd.velocity.magnitude / 300;
        engineSound.volume = 0.009f + rgd.velocity.magnitude / 1000;

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) {
            thruster.ChargeUp(Time.deltaTime * Thruster.THRUST_PER_SECOND);
            ApplyBrake();
        }
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) {
            Thrust();
        }
        if (Input.GetKeyUp(KeyCode.Backspace)) {
            Die();
        }
        bool brakePressed = Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (brakePressed) {
            ApplyBrake();
        }
        UIManager.instance.ShowDepth(-mainT.position.y);
        rgd.gravityScale = (mainT.position.y > SEA_LEVEL_Y) ? 3.333f : 0;
    }

    private void Thrust() {
        thrustParticles.Play();
        float thrustAmount = thruster.Release();
        //Debug.Log(thrustAmount);
        rgd.AddForce(thruster.ThrustDirection * thrustAmount);
        AudioManager.Instance.PlayBoostSound(thrustAmount);
    }

    private void ApplyBrake() {
        rgd.velocity *= 1 - (Time.deltaTime * brakeStrength);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        float magnitude = Mathf.InverseLerp(0, 25, rgd.velocity.magnitude);
        int damage = (int)Mathf.Lerp(4.2f, 10.1f, magnitude);
        CameraShaker.instance.HitCameraShake(magnitude);
        Health -= damage;
        AudioManager.Instance.PlayHitSound(damage);
    }

    private void Die() {
        string nameToUse = string.IsNullOrWhiteSpace(SessionData.playerName) ? "Some Poor Soul" : SessionData.playerName;
        DeathUIManager.instance.Show();
        DeathPointsLoader.Instance.AddDeathPoint(transform.position.x, transform.position.y, nameToUse);
        // todo play death animation
    }

    public void Respawn() {
        Health = MAX_HEALTH;
        mainT.position = new Vector2(0.0f, 0.0f);

        // todo flash alpha as in "i-frames"
        //CanvasGroup cg = mainPlayer.GetComponent<CanvasGroup>();

        //this.CreateAnimationRoutine(1.5f, delegate (float progress) {
        //    cg.alpha = progress * 5.0f % 1.0f > 0.5f ? 0 : 1;
        //}, delegate {
        //    cg.alpha = 1;
        //});
    }
}
