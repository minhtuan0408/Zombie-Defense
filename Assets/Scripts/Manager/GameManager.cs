using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	[Header("Quest List")]
	public List<bool> questStates = new List<bool>();

	[Header("Game State")]
	public bool isWin = false;
	public bool doneMission = false;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		// 👇 vào game lần đầu → load Home
		LoadScene("Home");
	}

	public void LoadScene(string sceneName)
	{
		ResetGame(); // 👈 reset trước khi load
		LoadingPanel.Instance.LoadScene(sceneName);
	}

	public void InitQuest(int count)
	{
		questStates.Clear();
		isWin = false;
		for (int i = 0; i < count; i++)
		{
			questStates.Add(false);
		}
	}

	// 🔥 set quest hoàn thành
	public void CompleteQuest(int index)
	{
		if (index >= 0 && index < questStates.Count)
		{
			questStates[index] = true;
		}
	}

	// 🔥 check tất cả đã xong chưa
	public bool AreAllQuestsCompleted()
	{
		foreach (bool q in questStates)
		{
			if (!q) return false;
		}
		isWin = true;
		return true;
	}

	public void ResetGame()
	{
		isWin = false;
		doneMission = false;
		questStates.Clear();
	}
}