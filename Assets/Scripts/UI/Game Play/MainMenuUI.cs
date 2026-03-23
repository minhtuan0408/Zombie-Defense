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

		// TODO: bạn tự xử lý load level ở đây
		// Ví dụ:
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		PlayGame();
	}

	void PlayGame()
	{
		
		GameManager.Instance.LoadScene("GamePlay");
	}

	// ===== LEVEL =====
	public void OnClickLevel()
	{
		Debug.Log("Open Level Panel");

		if (levelPanel != null)
			levelPanel.SetActive(true);
	}

	// ===== SETTING =====
	public void OnClickSetting()
	{
		Debug.Log("Open Setting Panel");

		if (settingPanel != null)
			settingPanel.SetActive(true);
	}

	// ===== CLOSE (dùng chung cho panel) =====
	public void ClosePanel(GameObject panel)
	{
		if (panel != null)
			panel.SetActive(false);
	}
}