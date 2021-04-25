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
	public void PlayExitWaterSound() {
		PlaySFX(exitWater, 0.75f);
	}

	public AudioClip enterWater;
	public void PlayEnterWaterSound() {
		PlaySFX(enterWater, 0.75f);
	}

	public AudioClip hit;
	public void PlayHitSound(float intensity) {
		PlaySFX(hit, 0.75f * intensity);
	}

	public AudioClip boost;
	public void PlayBoostSound(float intensity) {
		PlaySFX(boost, 0.75f * intensity);
	}

	public void PlaySFX(AudioClip clip, float volume)
	{
		AudioSource source = GetNextAudioSource();
		source.volume = volume;
		source.PlayOneShot(clip);
	}

	private AudioSource GetNextAudioSource()
	{
		AudioSource result = audioSources[audioSourceIndex];
		audioSourceIndex = (audioSourceIndex + 1) % audioSources.Length;
		return result;
	}
}
