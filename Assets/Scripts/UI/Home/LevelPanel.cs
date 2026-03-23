using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour, IPointerClickHandler
{
	[Header("UI")]
	public Image levelIcon;
	public GameObject lockObj;

	//public LevelData levelData;
	public int index;

	public TextMeshPro currentLevel;

	void UpdateUI()
	{
		bool isUnlocked = index <= PlayerProgress.Instance.CurrentLevel;

		if (lockObj != null)
			lockObj.SetActive(!isUnlocked);

		if (levelIcon != null)
			levelIcon.color = isUnlocked ? Color.white : Color.gray;

		UpdateCurrentLevelText();
	}

	void UpdateCurrentLevelText()
	{
		if (currentLevel == null) return;

		// Hiển thị level hiện tại player đang chọn / đang chơi
		int playingLevel = PlayerProgress.Instance.PlayingLevel;

		currentLevel.text = "DAY : " + playingLevel.ToString();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		bool isUnlocked = index <= PlayerProgress.Instance.CurrentLevel;

		if (!isUnlocked)
		{
			Debug.Log("Level bị khóa");
			return;
		}

		// set level đang chơi
		PlayerProgress.Instance.SetLevel(index);


		GameManager.Instance.LoadScene("GamePlay");
	}

	void OnEnable()
	{
		UpdateUI();
	}
}