using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviour
{
	[Header("Scene Name")]
	public string menuSceneName = "MainMenu";

	[Header("UI")]
	public GameObject winImage;
	public GameObject loseImage;

	public GameObject btnHome;
	public GameObject btnReplay;
	public GameObject btnNextLevel;

	[Header("Combo UI")]
	public TextMeshProUGUI bestComboText;

	public Animator bgAnimator;
	public GameObject panelContent; 
	
	public void OnClick_BackToMenu()
	{
		SoundManager.Instance.PlaySFX("Click");
		Debug.Log("CLICK");
		if (GameManager.Instance.isWin)
		{
			PlayerProgress.Instance.OnLevelWin();
		}
		Time.timeScale = 1f;
		SceneManager.LoadScene(menuSceneName);
	}

	// 👉 Replay
	public void OnClick_Replay()
	{
		SoundManager.Instance.PlaySFX("Click");
		Time.timeScale = 1f;
		SceneManager.LoadScene("GamePlay");
	}

	// 👉 Next Level
	public void OnClick_NextLevel()
	{
		SoundManager.Instance.PlaySFX("Click");
		if (GameManager.Instance.isWin)
		{
			PlayerProgress.Instance.OnLevelWin();		
		}
		Time.timeScale = 1f;
		SceneManager.LoadScene("GamePlay");
	}

	// 👉 Hiển thị Ending
	//public void ShowEnding()
	//{
	//	gameObject.SetActive(true);
	//	Time.timeScale = 0f;

	//	bool isWin = GameManager.Instance.isWin;

	//	// reset UI
	//	winImage.SetActive(false);
	//	loseImage.SetActive(false);
	//	btnHome.SetActive(false);
	//	btnReplay.SetActive(false);
	//	btnNextLevel.SetActive(false);

	//	bestComboText.gameObject.SetActive(false);

	//	// chạy sequence
	//	StartCoroutine(ShowSequence(isWin));
	//}
	public void ShowEnding()
	{
		StartCoroutine(EndingFlow());
	}
	IEnumerator ShowSequence(bool isWin)
	{
		// 🎯 1. Hiện Win/Lose
		if (isWin)
		{
			SoundManager.Instance.PlaySFX("Win");
			winImage.SetActive(true);
		}
		else
		{
			SoundManager.Instance.PlaySFX("Lose");
			loseImage.SetActive(true);
		}

		yield return new WaitForSecondsRealtime(0.5f);

		// 🎯 2. Hiện combo
		bestComboText.gameObject.SetActive(true);
		SoundManager.Instance.PlaySFX("ComboShow");
		int best = ComboManager.Instance.bestCombo;
		bestComboText.text = "BEST COMBO x" + best;

		yield return StartCoroutine(ComboScaleEffect());

		yield return new WaitForSecondsRealtime(0.3f);

		// 🎯 3. Hiện buttons
		btnHome.SetActive(true);
		btnReplay.SetActive(true);
		btnNextLevel.SetActive(isWin);
	}

	IEnumerator ComboScaleEffect()
	{
		float duration = 0.4f;
		float time = 0f;

		Vector3 start = Vector3.zero;
		Vector3 peak = Vector3.one * 2.5f; // phóng to mạnh
		Vector3 end = Vector3.one;

		while (time < duration)
		{
			time += Time.unscaledDeltaTime;
			float t = time / duration;

			t = Mathf.SmoothStep(0, 1, t);

			// scale 2 phase: to lên rồi về
			Vector3 scale;
			if (t < 0.5f)
			{
				scale = Vector3.Lerp(start, peak, t * 2);
			}
			else
			{
				scale = Vector3.Lerp(peak, end, (t - 0.5f) * 2);
			}

			bestComboText.transform.localScale = scale;
			yield return null;
		}

		bestComboText.transform.localScale = Vector3.one;
	}
	// 👉 Ẩn panel
	public void HideEnding()
	{
		gameObject.SetActive(false);
		Time.timeScale = 1f;
	}

	IEnumerator EndingFlow()
	{
		//// 🎬 1. BG bắt đầu fade tối
		//bgAnimator.SetTrigger("Fade"); // hoặc Play("Fade")

		// 🎬 2. Bật panel nhưng ẨN content
		gameObject.SetActive(true);
		panelContent.SetActive(false);



		// ⏳ 3. Đợi màn hình tối dần
		yield return new WaitForSecondsRealtime(1f);

		// 🎬 4. Play anim End của BG
		bgAnimator.Play("End");
		panelContent.SetActive(true);
		// ⏳ đợi nhẹ cho mượt (tuỳ chỉnh)
		yield return new WaitForSecondsRealtime(0.2f);

		// 🎯 5. Bật UI content
		

		// 🎯 6. reset UI (y chang code cũ của bạn)
		winImage.SetActive(false);
		loseImage.SetActive(false);
		btnHome.SetActive(false);
		btnReplay.SetActive(false);
		btnNextLevel.SetActive(false);
		bestComboText.gameObject.SetActive(false);

		// 🎯 7. chạy sequence cũ
		bool isWin = GameManager.Instance.isWin;
		StartCoroutine(ShowSequence(isWin));
	}
}