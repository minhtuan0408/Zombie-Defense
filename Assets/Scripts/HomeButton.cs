using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
	public GameObject confirmPanel; // Panel xác nhận

	// Bấm nút Home
	public void OnClickHome()
	{
		confirmPanel.SetActive(true);
		Time.timeScale = 0f; // Pause game
	}

	// Bấm YES (đồng ý về menu)
	public void OnConfirmYes()
	{
		SoundManager.Instance.PlaySFX("Click");
		Time.timeScale = 1f;
		GameManager.Instance.LoadScene("Home");
	}

	// Bấm NO (ở lại game)
	public void OnConfirmNo()
	{
		SoundManager.Instance.PlaySFX("Click");
		confirmPanel.SetActive(false);
		Time.timeScale = 1f; // Resume game
	}
}