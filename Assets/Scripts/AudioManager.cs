using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private const string AUDIO_MANAGER_PATH = "AudioManager";
	private static AudioManager instance;
	public static AudioManager Instance
	{
		get
		{
			if (instance == null) {
				GameObject gameOverScreenObject = (GameObject)Resources.Load(AUDIO_MANAGER_PATH);
				GameObject instantiated = Instantiate(gameOverScreenObject);
				DontDestroyOnLoad(instantiated);
				instance = instantiated.GetComponent<AudioManager>();
			}
			return instance;
		}
	}

	[Header("Sound Effects")]
	public AudioSource[] audioSources;

	private int audioSourceIndex = 0;

	public AudioClip exitWater;
	public void PlayExitWaterSound(float intensity) {
		PlaySFX(exitWater, 1.4f * intensity);
	}

	public AudioClip enterWater;
	public void PlayEnterWaterSound(float intensity) {
		PlaySFX(enterWater, 8.0f * intensity);
	}

	public AudioClip hit;
	public void PlayHitSound(float intensity) {
		PlaySFX(hit, 0.065f * intensity, intensity * 0.2f);
	}

	public AudioClip boost;
	public void PlayBoostSound(float intensity) {
		PlaySFX(boost, 0.3f * intensity / 2500, 1 + intensity/10000);
	}

	public void PlaySFX(AudioClip clip, float volume)
	{
		AudioSource source = GetNextAudioSource();
		source.volume = volume;
		source.PlayOneShot(clip);
	}
	public void PlaySFX(AudioClip clip, float volume, float pitch) {
		AudioSource source = GetNextAudioSource();
		source.volume = volume;
		source.PlayOneShot(clip);
		source.pitch = pitch;
	}

	private AudioSource GetNextAudioSource()
	{
		AudioSource result = audioSources[audioSourceIndex];
		audioSourceIndex = (audioSourceIndex + 1) % audioSources.Length;
		return result;
	}
}
