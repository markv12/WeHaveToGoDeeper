using UnityEngine;

public class CameraShaker : MonoBehaviour {

	public static CameraShaker instance;
	private void Awake() {
		instance = this;
	}

	public Transform shakeTransform;

	public float maxOffset;
	public float shakeSpeed;
	[Range(0.01f, 5f)]
	public float shakeLength = 0.3f;
	private float shakeAmount;
	private float time = 0;

	private void Update() {
		ApplyShake();
	}
	private void ApplyShake() {
		if (shakeAmount > 0) {
			time += (shakeSpeed * Time.deltaTime);
			float x = Mathf.PerlinNoise(0, time) * shakeAmount * maxOffset;
			float y = Mathf.PerlinNoise(0, time + 1) * shakeAmount * maxOffset;
			shakeTransform.localPosition = new Vector3(x, y, 0);
		}
	}

	private Coroutine shakeRoutine;
	public void HitCameraShake() {
		this.EnsureCoroutineStopped(ref shakeRoutine);
		this.CreateAnimationRoutine(
			shakeLength,
			delegate (float progress) {
				shakeAmount = Mathf.Lerp(1, 0, progress);
			}
		);
	}
}
