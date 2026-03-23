using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
	[Header("Sliders")]
	public Slider bgmSlider;
	public Slider sfxSlider;

	[Header("Toggles")]
	public Toggle muteAllToggle;
	public Toggle muteBGMToggle;
	public Toggle muteSFXToggle;

	void Start()
	{
		Debug.Log("bgmSlider: " + bgmSlider);
		Debug.Log("sfxSlider: " + sfxSlider);
		//Debug.Log("muteAllToggle: " + muteAllToggle);
		// Load giá trị từ SoundManager
		bgmSlider.value = SoundManager.Instance.bgmVolume;
		sfxSlider.value = SoundManager.Instance.sfxVolume;

		//muteAllToggle.isOn = SoundManager.Instance.muteAll;
		//muteBGMToggle.isOn = SoundManager.Instance.muteBGM;
		//muteSFXToggle.isOn = SoundManager.Instance.muteSFX;

		// Add listener
		bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
		sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

		//muteAllToggle.onValueChanged.AddListener(OnMuteAllChanged);
		//muteBGMToggle.onValueChanged.AddListener(OnMuteBGMChanged);
		//muteSFXToggle.onValueChanged.AddListener(OnMuteSFXChanged);

	}

	// ================= VOLUME =================
	void OnBGMVolumeChanged(float value)
	{
		SoundManager.Instance.SetBGMVolume(value);
	}

	void OnSFXVolumeChanged(float value)
	{
		SoundManager.Instance.SetSFXVolume(value);
	}

	// ================= MUTE =================
	void OnMuteAllChanged(bool value)
	{
		SoundManager.Instance.SetMuteAll(value);
	}

	void OnMuteBGMChanged(bool value)
	{
		SoundManager.Instance.SetMuteBGM(value);
	}

	void OnMuteSFXChanged(bool value)
	{
		SoundManager.Instance.SetMuteSFX(value);
	}
}