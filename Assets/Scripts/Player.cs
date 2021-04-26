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
    private bool isFlying = false;
    private int fishLayer;

    void Start() {
        if (SessionData.playerName == null) SessionData.playerName = "Assistant";
        fishLayer = LayerMask.NameToLayer("Fish");
        Health = MAX_HEALTH;
        mainPlayer = this;
        SessionData.InitGame();
        UIManager.instance.ShowMessage(@"professor: Come, " + SessionData.playerName +  @"! We must journey <anim:shake>into the depths!!</anim>
player: Why are we even doing this?
professor: Copernicus! Magellan! Soon <b><anim:shake>I</anim></b> will join their ranks!! The bottom of the sea awaits!
player: ...This is stupid.
professor: Silence, my lowly assistant! <p:normal>We must go <anim:wave><b><cspace=5>Deeper!!!</cspace></b></anim>");
    }

    public const float SEA_LEVEL_Y = 10f;
    void Update() {
        if (DeathUIManager.instance.shown) return;

        engineSound.pitch = Mathf.Lerp(engineSound.pitch,
            Mathf.Max(
                0.0f + thruster.ThrustAmount * 2.0f,
                0.2f + rgd.velocity.magnitude / 500
            ),
            thruster.ThrustAmount * 2.0f >
            rgd.velocity.magnitude / 500 ?
                1 : Time.deltaTime * 2.0f
          );
        engineSound.volume = Mathf.Lerp(engineSound.volume,
            Mathf.Max(
                0.0f + thruster.ThrustAmount * 0.05f,
                0.009f + rgd.velocity.magnitude / 500
                ), Time.deltaTime * 2.0f
          );

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
        if (!isFlying && mainT.position.y > SEA_LEVEL_Y) {
            isFlying = true;
            AudioManager.Instance.PlayExitWaterSound(rgd.velocity.magnitude / 1000);
            UIManager.instance.ShowMessage(@"professor: No, no no! <p:normal>We have to go <u><b>deeper</b></u>, not higher!!
player: Is it too late to go back???
professor: <size=30>...Why are kids these days so shallow?</size>");
        }
        else if (isFlying && mainT.position.y > SEA_LEVEL_Y + 100.0f) {
            UIManager.instance.ShowMessage(@"professor: Wait, where are you going!?! How are you doing that?
player: ""<i>Flyyy~ me to the <anim:wave>mooooon</anim>, <p:normal>let me <anim:wave>plaaaaaay</anim> among the stars...</i>""");
        }
        else if (isFlying && mainT.position.y <= SEA_LEVEL_Y) {
            isFlying = false;
            AudioManager.Instance.PlayEnterWaterSound(rgd.velocity.magnitude / 1000);
        }
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
        if (Random.value > 0.5f) {
            if (collision.gameObject.layer == fishLayer) {
                string[] options = {@"professor: Did you see the <anim:shake>size</anim> of that beast?? <b><i>Terrifying!</i></b>",
@"professor: <b><anim:shake><size=50>Gracious!</size></anim></b> What a brute!
player: You just spit on me.",
@"professor: I could have dodged that with my eyes closed!
player: <size=30>But you're legally blind...</size>",
@"professor: Oof!",
@"professor: Ouch!",
@"professor: Yowch!",
@"professor: You <anim:shake>clumsy cod!</anim> And I don't mean the fish!",
            };
                string choice = options[Random.Range(0, options.Length)];
                UIManager.instance.ShowMessage(choice);
            }
            else {
                string[] options = {@"professor: Oof! Careful, fool!
player: <size=30>Takes one to know one...</size>",
@"professor: Watch the stern! <p:normal>The <anim:shake>STERN!!</anim>",
@"professor: That's a <b>wall!</b> Have you ever even driven this submarine before?
player: <size=30>Maybe if you stopped talking over my shoulder...</size>",
@"professor: Just because I said I want to go <b><anim:wave>deeper</anim></b>, I don't mean that I want us to sink!!
player: <size=30>You're welcome to take the wheel anytime now...</size>",
@"professor: Pay more attention!",
@"professor: Oof!",
@"professor: Ouch!",
@"professor: You oaf!
player: <size=30>Learn some insults from this century, maybe?</size>",
@"professor: Still gaining your sea legs, are you?",
@"professor: I designed this sub so that even an operator of below-average intelligence could pilot it. What's <b>your</b> excuse!?",
@"professor: You <anim:shake>clumsy cod!</anim>",
@"professor: Oof! You nearly knocked me off my feet with that one!
player: <i><size=30>Nearly? Well, there's always next time...</size></i>"};
                string choice = options[Random.Range(0, options.Length)];
                UIManager.instance.ShowMessage(choice);
            }
        }
    }

    private void Die() {
        rgd.velocity = new Vector2(0.0f, 0.0f);
        engineSound.volume = 0.0f;
        string nameToUse = string.IsNullOrWhiteSpace(SessionData.playerName) ? "Some Poor Soul" : SessionData.playerName;
        DeathUIManager.instance.Show();
        DeathPointsLoader.Instance.AddDeathPoint(transform.position.x, transform.position.y, nameToUse);
        // todo play death animation
    }

    public void Respawn() {
        Health = MAX_HEALTH;
        mainT.position = SessionData.lastCheckpoint;
        rgd.velocity = new Vector2(0.0f, 0.0f);

        CameraShaker.instance.ResetShake();
        // todo flash alpha as in "i-frames"
        //CanvasGroup cg = mainPlayer.GetComponent<CanvasGroup>();

        //this.CreateAnimationRoutine(1.5f, delegate (float progress) {
        //    cg.alpha = progress * 5.0f % 1.0f > 0.5f ? 0 : 1;
        //}, delegate {
        //    cg.alpha = 1;
        //});
    }
}
