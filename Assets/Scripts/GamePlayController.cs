using UnityEngine;

public class GamePlayController : MonoBehaviour
{
	public static GamePlayController Instance;

	public bool isPlaying = true;
	public bool isInStory = false;
	public GameObject CanvasGame;
	public GameObject Player;
	void Awake()
	{
		Instance = this;
	}
	private void Start()
	{

		SoundManager.Instance.PlayBGM(1);
		if (PlayerProgress.Instance.PlayingLevel == 0)
		{
			StoryManager.instance.PlayStory(0);
		}
		else if (PlayerProgress.Instance.PlayingLevel == 1)
		{

		}
		else
		{
			if (CanvasGame != null)
				CanvasGame.SetActive(true);
			if (Player != null)
				Player.SetActive(true);
		}

	}
	public void OnStoryStart()
	{
		isInStory = true;
		isPlaying = false;
	}

	public void OnStoryFinished()
	{
		Debug.Log("Resume game");

		isInStory = false;
		isPlaying = true;
	}
}