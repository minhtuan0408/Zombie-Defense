using UnityEngine;

public class ClosePanelButton : MonoBehaviour
{
	public GameObject panel; // Kéo Panel vào ðây trong Inspector

	public void TurnOffPanel()
	{
		SoundManager.Instance.PlaySFX("Click");
		if (panel != null)
		{
			panel.SetActive(false);
		}
	}
}