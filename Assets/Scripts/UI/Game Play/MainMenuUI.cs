using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
	[Header("Panels")]
	public GameObject levelPanel;
	public GameObject settingPanel;
	public TextMeshProUGUI currentLevel;
	// ===== PLAY =====
	private void Start()
	{
		int playingLevel = PlayerProgress.Instance.PlayingLevel;
		currentLevel.text = "DAY : " + (playingLevel+1).ToString();
	}
	public void OnClickPlay()
	{
		Debug.Log("Play clicked");
		SoundManager.Instance.PlaySFX("Click");
		PlayGame();
	}

	void PlayGame()
	{
		
		GameManager.Instance.LoadScene("GamePlay");
	}

	// ===== LEVEL =====
	public void OnClickLevel()
	{

		SoundManager.Instance.PlaySFX("Click");
		if (levelPanel != null)
			levelPanel.SetActive(true);
	}

	// ===== SETTING =====
	public void OnClickSetting()
	{

		SoundManager.Instance.PlaySFX("Click");
		if (settingPanel != null)
			settingPanel.SetActive(true);
	}

	// ===== CLOSE (dùng chung cho panel) =====
	public void ClosePanel(GameObject panel)
	{
		SoundManager.Instance.PlaySFX("Click");
		if (panel != null)
			panel.SetActive(false);
	}
}