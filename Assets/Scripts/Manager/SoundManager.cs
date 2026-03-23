using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance;

	[Header("Audio Sources")]
	[SerializeField] private AudioSource bgmSource;
	[SerializeField] private AudioSource sfxSource;

	[Header("BGM Themes")]
	[SerializeField] private SoundData[] bgmThemes;

	[Header("SFX Sounds")]
	[SerializeField] private SoundData[] sfxSounds;

	private Dictionary<string, AudioClip> bgmMap;
	private Dictionary<string, AudioClip> sfxMap;

	[Header("Settings")]
	public bool muteAll;
	public bool muteBGM;
	public bool muteSFX;

	const string MUTE_ALL_KEY = "MUTE_ALL";
	const string MUTE_BGM_KEY = "MUTE_BGM";
	const string MUTE_SFX_KEY = "MUTE_SFX";


	[Range(0f, 1f)] public float bgmVolume = 1f;
	[Range(0f, 1f)] public float sfxVolume = 1f;

	const string BGM_VOLUME_KEY = "BGM_VOLUME";
	const string SFX_VOLUME_KEY = "SFX_VOLUME";

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		LoadSettings();
		ApplyMuteState();
		BuildSFXMap();
		PlayBGM(bgmThemes[0].clip);

		Debug.Log(bgmSource);
		Debug.Log(sfxSource);
		Debug.Log(bgmThemes.Length);
	}

	void BuildSFXMap()
	{
		sfxMap = new Dictionary<string, AudioClip>();

		foreach (var sfx in sfxSounds)
		{
			if (string.IsNullOrEmpty(sfx.name) || sfx.clip == null)
				continue;

			if (!sfxMap.ContainsKey(sfx.name))
				sfxMap.Add(sfx.name, sfx.clip);
			else
				Debug.LogWarning($"Duplicate SFX name: {sfx.name}");
		}
	}
	// ================= BGM =================
	public void PlayBGM(AudioClip clip, bool loop = true)
	{
		if (clip == null) return;

		bgmSource.clip = clip;
		bgmSource.loop = loop;

		if (!muteAll && !muteBGM)
			bgmSource.Play();
	}

	public void StopBGM()
	{
		bgmSource.Stop();
	}

	// ================= SFX =================
	public void PlaySFX(AudioClip clip)
	{
		if (clip == null) return;
		if (muteAll || muteSFX) return;

		sfxSource.PlayOneShot(clip);
	}
	public void PlaySFX(string soundName)
	{
		if (muteAll || muteSFX) return;

		if (sfxMap.TryGetValue(soundName, out var clip))
		{
			sfxSource.PlayOneShot(clip);
		}
		else
		{
			Debug.LogWarning($"SFX not found: {soundName}");
		}
	}


	// ================= MUTE =================
	public void SetMuteAll(bool value)
	{
		muteAll = value;
		SaveSettings();
		ApplyMuteState();
	}

	public void SetMuteBGM(bool value)
	{
		muteBGM = value;
		SaveSettings();
		ApplyMuteState();
	}

	public void SetMuteSFX(bool value)
	{
		muteSFX = value;
		SaveSettings();
	}

	void ApplyMuteState()
	{
		bgmSource.mute = muteAll || muteBGM;
		sfxSource.mute = muteAll || muteSFX;
	}

	public void SetBGMVolume(float value)
	{
		bgmVolume = value;
		bgmSource.volume = value;

		PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
		PlayerPrefs.Save();
	}

	public void SetSFXVolume(float value)
	{
		sfxVolume = value;
		sfxSource.volume = value;

		PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
		PlayerPrefs.Save();
	}

	// ================= SAVE / LOAD =================
	void SaveSettings()
	{
		PlayerPrefs.SetInt(MUTE_ALL_KEY, muteAll ? 1 : 0);
		PlayerPrefs.SetInt(MUTE_BGM_KEY, muteBGM ? 1 : 0);
		PlayerPrefs.SetInt(MUTE_SFX_KEY, muteSFX ? 1 : 0);
		PlayerPrefs.Save();
	}

	void LoadSettings()
	{
		muteAll = PlayerPrefs.GetInt(MUTE_ALL_KEY, 0) == 1;
		muteBGM = PlayerPrefs.GetInt(MUTE_BGM_KEY, 0) == 1;
		muteSFX = PlayerPrefs.GetInt(MUTE_SFX_KEY, 0) == 1;

		bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
		sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

		bgmSource.volume = bgmVolume;
		sfxSource.volume = sfxVolume;
	}
	// ================= SFX (Index) =================
	public void PlaySFX(int index)
	{
		if (muteAll || muteSFX) return;

		if (index < 0 || index >= sfxSounds.Length)
		{
			Debug.LogWarning($"SFX index out of range: {index}");
			return;
		}

		var clip = sfxSounds[index].clip;
		if (clip != null)
		{
			sfxSource.PlayOneShot(clip);
		}
	}

	// ================= BGM (Index) =================
	public void PlayBGM(int index, bool loop = true)
	{
		if (index < 0 || index >= bgmThemes.Length)
		{
			Debug.LogWarning($"BGM index out of range: {index}");
			return;
		}

		var clip = bgmThemes[index].clip;
		if (clip == null) return;

		bgmSource.clip = clip;
		bgmSource.loop = loop;

		if (!muteAll && !muteBGM)
			bgmSource.Play();
	}
}

[System.Serializable]
public class SoundData
{
	public string name;
	public AudioClip clip;
}
