using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
	public static PlayerProgress Instance;

	private const string LEVEL_KEY = "CURRENT_LEVEL";
	public int CurrentLevel { get; private set; }
	public int PlayingLevel { get; private set; }
	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		LoadLevel();

	}
	void LoadLevel()
	{
		CurrentLevel = PlayerPrefs.GetInt(LEVEL_KEY, 0);

		PlayingLevel = CurrentLevel;
	}
	public void OnLevelWin()
	{
		// Chỉ unlock khi thắng level cao nhất đã mở
		if (PlayingLevel >= CurrentLevel)
		{
			CurrentLevel++;
			SaveLevel();
		}
		PlayingLevel = CurrentLevel;
	}
	public void SetLevel(int level)
	{
		if (level > CurrentLevel) return;

		PlayingLevel = level;
	}
	void SaveLevel()
	{
		PlayerPrefs.SetInt(LEVEL_KEY, CurrentLevel);
		PlayerPrefs.Save();
	}
	public void JumpToHighestUnlockedLevel()
	{
		SetLevel(CurrentLevel);
	}

}